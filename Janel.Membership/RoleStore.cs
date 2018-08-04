using Janel.Contract;
using Janel.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Janel.Membership {
  public class RoleStore : IRoleStore<IdentityRole> {
    private readonly IJanelUnitOfWork _unitOfWork;

    public RoleStore(IJanelUnitOfWork unitOfWork) {
      _unitOfWork = unitOfWork;
    }

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.RoleRepository.Insert(role);
        return IdentityResult.Success;
      });
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.RoleRepository.Delete(role);
        return IdentityResult.Success;
      });
    }

    public void Dispose() { }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return _unitOfWork.RoleRepository.GetById(new Guid(roleId));
      });
    }

    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return _unitOfWork.RoleRepository.GetByName(normalizedRoleName);
      });
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return role.Name;
      });
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return role.Id.ToString();
      });
    }

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        return role.Name;
      });
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        role.Name = normalizedName;
        _unitOfWork.RoleRepository.Update(role);
      });
    }

    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken) {
      return Task.Run(() => {
        role.Name = roleName;
        _unitOfWork.RoleRepository.Update(role);
      });
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken) {
      return Task.Run(() => {
        _unitOfWork.RoleRepository.Update(role);
        return IdentityResult.Success;
      });
    }
  }
}
