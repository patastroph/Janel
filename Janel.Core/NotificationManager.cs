using Janel.Contract;
using Janel.Data;
using Janel.Data.Event;
using Observer.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Janel.Core {
  public class NotificationManager : INotificationManager, IEventListener {
    private readonly IScheduleManager _scheduleManager;
    private readonly IJanelUnitOfWork _janelUnitOfWork;
    private List<Notification> _ongoingNotifications;

    public NotificationManager(IScheduleManager scheduleManager, IJanelUnitOfWork janelUnitOfWork) {
      _scheduleManager = scheduleManager;
      _janelUnitOfWork = janelUnitOfWork;
      _ongoingNotifications = new List<Notification>();
    }

    public void RegisterEvents(IEventManager eventManager) {
      eventManager.Register<AlertReceived>().To(OnAlertReceived);
      eventManager.Register<TaskTimerElapsed>().To(ValidateSentNotifications).When(t => t.MinuteElapsed == 1);
    }

    private IEnumerable<Message> ValidateSentNotifications(TaskTimerElapsed arg) {
      var notificationsToRemove = new List<Notification>();

      foreach (var notification in _ongoingNotifications) {
        if (notification.MessagesSent.Last().SentOn.AddMinutes(3) >= DateTime.Now) {
          //3 minutes elapsed. Need to do something
          if (notification.MessagesSent.Count >= 3) {
            //Responsible never responded
            notificationsToRemove.Add(notification);
            JanelObserver.EventManager.Dispatch(new NotificationNotResponded(notification));
          } else {
            //Retry sending a message
            var sendItTo = notification.MessagesSent.Last().SentTo;
            var nbMessageSentToThisGuy = notification.MessagesSent.Where(m => m.SentTo == sendItTo).Count();

            var communicationType = nbMessageSentToThisGuy <= sendItTo.PreferedCommunications.Count ?
                                      sendItTo.PreferedCommunications.ElementAt(nbMessageSentToThisGuy) :
                                      sendItTo.PreferedCommunications.Last();

            SendNotificationWithAcknowledge(DateTime.Now, sendItTo, communicationType, notification.Message, notification, null);

            _janelUnitOfWork.NotificationRepository.Update(notification);
          }
        }
      }

      if (notificationsToRemove.Any()) {
        notificationsToRemove.ForEach(n => _ongoingNotifications.Remove(n));
      }

      return JanelObserver.Success();
    }

    public void AcknowledgeNotification(Guid notificationId, Person by) {
      if (_ongoingNotifications.Any(n => n.Id.Equals(notificationId))) {
        var notification = _ongoingNotifications.First(n => n.Id.Equals(notificationId));
        
        notification.IsAcknowledge = true;
        notification.AcknowledgeBy = by;

        _janelUnitOfWork.NotificationRepository.Update(notification);

        _ongoingNotifications.Remove(notification);

        JanelObserver.EventManager.Dispatch(new NotificationAcknowledge(notification, by));
      }     
    }

    private IEnumerable<Message> OnAlertReceived(AlertReceived arg) {
      var responsible = arg.OverridePerson ?? _scheduleManager.GetPersonInCharge();
      var sentDate = DateTime.Now;

      if (responsible == null) {
        JanelObserver.EventManager.Dispatch(new ErrorOccurred("No Responsible found"));
        return new List<Message> { new Message { Description = "No Responsible found", Succeeded = false } };
      }

      arg.Alert.Responsible = responsible;
      arg.Alert.NotificationsSent.Add(sentDate);
      arg.Alert.Actors.Add(responsible);
      arg.Alert.Status = StatusType.New;

      _janelUnitOfWork.AlertRepository.Update(arg.Alert);

      var message = $"Alert received from {arg.Alert.Service.Name}, on {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}. Message : {arg.Alert.Description}";

      return SendNotificationWithAcknowledge(sentDate, responsible, (responsible.PreferedCommunications?.First() ?? CommunicationType.Email), message, null, arg.Alert);         
    }

    private IEnumerable<Message> SendNotificationWithAcknowledge(DateTime sentDate, Person to, CommunicationType communicationType, string message, Notification notification, object source) {
      var communication = new Communication {
        SentOn = sentDate,
        SentTo = to,
        Type = communicationType
      };

      if (notification == null) {
        notification = new Notification {
          Message = $"{message}\n\nClick here to take action http:\\mysuperlink.com",
          Source = source
        };

        _ongoingNotifications.Add(notification);
      }

      notification.MessagesSent.Add(communication);

      communication.IsSentSuccessfully = SendNotification(to, notification.Message, communication.Type);

      _janelUnitOfWork.NotificationRepository.Insert(notification);

      if (communication.IsSentSuccessfully) {
        return JanelObserver.Success();
      } else {
        var error = $"Unable to send notification {notification.Id}";

        JanelObserver.EventManager.Dispatch(new ErrorOccurred(error));

        return new List<Message> { new Message { Description = error, Succeeded = false } };
      }
    }

    public bool SendNotification(Person to, string message, CommunicationType type) {
      var twilioAccountSID = ConfigurationManager.AppSettings["Twilio_AccountSID"] ?? "AC4e6a4cc549dc577e83ab5ed943b7aa4d";
      var twilioCredentials = ConfigurationManager.AppSettings["Twilio_Credentials"] ?? "c8643b5c95dd80b3e33487ae03d2664e";
      var phoneNumber = ConfigurationManager.AppSettings["Twilio_PhoneNumber"] ?? "+14388340890";
      
      try {
        switch (type) {
          case CommunicationType.SMS:
            TwilioClient.Init(twilioAccountSID, twilioCredentials);

            var messageResource = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(phoneNumber),
                to: new Twilio.Types.PhoneNumber(to.PhoneNumber)
            );

            break;
          case CommunicationType.PhoneCall:
            TwilioClient.Init(twilioAccountSID, twilioCredentials);

            var call = CallResource.Create(
                url: new Uri("http://demo.twilio.com/docs/voice.xml"),
                to: new Twilio.Types.PhoneNumber(phoneNumber),
                from: new Twilio.Types.PhoneNumber(to.PhoneNumber)
            );

            break;
          case CommunicationType.Email:
          default:
            var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpIP"] ?? "relais.videotron.ca");
            
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["NotificationFromEmail"] ?? "notification@robotshop.com");
            mailMessage.To.Add(to.Email);
            mailMessage.Body = message;
            mailMessage.Subject = "RobotShop Notification";
            client.Send(mailMessage);

            break;
        }
      } catch (Exception exc) {
        JanelObserver.EventManager.Dispatch(new ErrorOccurred(exc));
        return false;
      }      

      return true;
    }
  }
}
