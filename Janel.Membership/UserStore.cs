using Janel.Contract;
using Janel.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Janel.Membership {
  public class UserStore : IUserStore<Person>, IUserPasswordStore<Person>, IUserPhoneNumberStore<Person> {
    private readonly IJanelUnitOfWork _unitOfWork;

    public UserStore(IJanelUnitOfWork unitOfWork) {
      _unitOfWork = unitOfWork;
    }

    public Task<IdentityResult> CreateAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.PersonRepository.Insert(user);
        return IdentityResult.Success;
      });
    }

    public Task<IdentityResult> DeleteAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.PersonRepository.Delete(user);
        return IdentityResult.Success;
      });
    }

    public void Dispose() { }

    public Task<Person> FindByIdAsync(string userId, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return _unitOfWork.PersonRepository.GetById(new Guid(userId));        
      });
    }

    public Task<Person> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return _unitOfWork.PersonRepository.GetByUserName(normalizedUserName);
      });
    }

    public Task<string> GetNormalizedUserNameAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return user.Email.ToLower();
      });
    }

    public Task<string> GetPasswordHashAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return user.Password;
      });
    }

    public Task<string> GetPhoneNumberAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return user.PhoneNumber;
      });
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return false;
      });
    }

    public Task<string> GetUserIdAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return user.Id.ToString();
      });
    }

    public Task<string> GetUserNameAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return user.Email;
      });
    }

    public Task<bool> HasPasswordAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return !string.IsNullOrEmpty(user.Password);
      });
    }

    public Task SetNormalizedUserNameAsync(Person user, string normalizedName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        user.Email = normalizedName.ToLower();
        _unitOfWork.PersonRepository.Update(user);
        return this;
      });
    }

    public Task SetPasswordHashAsync(Person user, string passwordHash, CancellationToken cancellationToken) {
      return Task.Run(() => {
        user.Password = passwordHash;
        _unitOfWork.PersonRepository.Update(user);
        return this;
      });
    }

    public Task SetPhoneNumberAsync(Person user, string phoneNumber, CancellationToken cancellationToken) {
      return Task.Run(() => {
        user.PhoneNumber = phoneNumber;
        _unitOfWork.PersonRepository.Update(user);
        return this;
      });
    }

    public Task SetPhoneNumberConfirmedAsync(Person user, bool confirmed, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return true;
      });
    }

    public Task SetUserNameAsync(Person user, string userName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        user.Email = userName.ToLower();
        _unitOfWork.PersonRepository.Update(user);
        return this;
      });
    }

    public Task<IdentityResult> UpdateAsync(Person user, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.PersonRepository.Update(user);
        return IdentityResult.Success;
      });
    }
  }
}
