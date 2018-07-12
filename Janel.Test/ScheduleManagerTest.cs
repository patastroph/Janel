using Janel.Contract;
using Janel.Core;
using Janel.Data;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests {
  [TestFixture]
  public class ScheduleManagerTest {
    private Mock<IJanelUnitOfWork> _unitOfWork;
    private Mock<IDateTimeManager> _dateTimeManager;

    [SetUp]
    public void Setup() {
      _unitOfWork = new Mock<IJanelUnitOfWork>();
      _dateTimeManager = new Mock<IDateTimeManager>();

      _dateTimeManager.Setup(dt => dt.GetNow()).Returns(new DateTime(2017, 1, 3, 15, 30, 14));
    }

    [Test]
    public void ShouldGetExceptionWhenNoScheduleInserted() {
      var manager = new ScheduleManager(_unitOfWork.Object, _dateTimeManager.Object);

      _unitOfWork.Setup(u => u.ScheduleRepository.GetList()).Returns(new List<Schedule>().AsQueryable());

      Assert.Throws(Is.TypeOf<Exception>(), () => manager.GetPersonInCharge());
    }

    [Test]
    public void ShouldGetAResponsible() {
      var manager = new ScheduleManager(_unitOfWork.Object, _dateTimeManager.Object);
      var responsible = new Person();

      _unitOfWork.Setup(u => u.ScheduleRepository.GetList()).Returns(new List<Schedule>
      {
        new Schedule {
          Id = Guid.NewGuid(),
          StartAt = new DateTime(2017, 1 , 1 , 8, 0, 0),
          EndAt = new DateTime(2017, 1 , 7 , 8, 0, 0),
          Responsible = responsible
        }
      }.AsQueryable());

      var personInCharge = manager.GetPersonInCharge();

      Assert.AreEqual(responsible, personInCharge);
    }

    [Test]
    public void WhenResponsibleBusyShouldGetNextInSchedule() {
      var manager = new ScheduleManager(_unitOfWork.Object, _dateTimeManager.Object);
      var busyResponsible = new Person { Name = "Iam Busy" };
      var nextResponsible = new Person { Name = "Iam Next" };
      var previousResponsible = new Person { Name = "Iwas Responsible" };

      _unitOfWork.Setup(u => u.ScheduleRepository.GetList()).Returns(new List<Schedule>
      {
        new Schedule {
          Id = Guid.NewGuid(),
          StartAt = new DateTime(2017, 1 , 7, 7, 59, 0),
          EndAt = new DateTime(2017, 1 , 14, 8, 0, 0),
          Responsible = nextResponsible,
        },
        new Schedule {
          Id = Guid.NewGuid(),
          StartAt = new DateTime(2017, 1 , 1 , 7, 59, 0),
          EndAt = new DateTime(2017, 1 , 7 , 8, 0, 0),
          Responsible = busyResponsible,
          IsBusy = true,
          BusyReason = "I am out"
        },
        new Schedule {
          Id = Guid.NewGuid(),
          StartAt = new DateTime(2016, 12 , 26, 8, 0, 0),
          EndAt = new DateTime(2017, 1 , 1, 7, 59, 0),
          Responsible = previousResponsible,
        },
      }.AsQueryable());

      var personInCharge = manager.GetPersonInCharge();

      Assert.AreEqual(nextResponsible, personInCharge, $"Expected {nextResponsible.Name} but was {personInCharge.Name}");
    }

    [Test]
    public void ScheduleShouldBeInsertedWhenAdded() {
      var manager = new ScheduleManager(_unitOfWork.Object, _dateTimeManager.Object);

      _unitOfWork.Setup(u => u.ScheduleRepository.GetList()).Returns(new List<Schedule>().AsQueryable());
      
      manager.AddSchedule(null, new DateTime(), new DateTime());

      _unitOfWork.Verify(u => u.ScheduleRepository.Insert(It.IsAny<Schedule>()), Times.Once);
    }

    [Test]
    public void ScheduleShouldBeInsertedWithGoodInfos() {
      var manager = new ScheduleManager(_unitOfWork.Object, _dateTimeManager.Object);
      var responsible = new Person();
      var starDate = new DateTime(2016, 12, 26, 8, 0, 0);
      var endDate = new DateTime(2017, 1, 1, 7, 59, 0);

      _unitOfWork.Setup(u => u.ScheduleRepository.GetList()).Returns(new List<Schedule>().AsQueryable());
      
      manager.AddSchedule(responsible, starDate, endDate);

      _unitOfWork.Verify(u => u.ScheduleRepository.Insert(It.Is<Schedule>(s => s.Responsible == responsible && s.StartAt == starDate && s.EndAt == endDate)), Times.Once);
    }

  }
}