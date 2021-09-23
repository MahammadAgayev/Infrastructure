using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace IdentityServer
{
    public class RoleStore : IRoleStore<Role>
    {
        private IUnitOfWork _untiofwork;
        public RoleStore(IUnitOfWork unitOfWork)
        {
            _untiofwork = unitOfWork;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _untiofwork.RoleRepository.CreateAsync(role);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            //await _untiofwork.RoleRepository.UpdateAsync(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public  Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            //await _untiofwork.RoleRepository.DeleteAsync(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var role = await _untiofwork.RoleRepository.GetAsync(int.Parse(roleId));
            return role;
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var role = await _untiofwork.RoleRepository.GetAsync(normalizedRoleName);

            return role;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}