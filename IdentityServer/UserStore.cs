using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace IdentityServer
{
    public class UserStore : IUserAuthenticatorKeyStore<Account>, IUserStore<Account>, IUserEmailStore<Account>, IUserPhoneNumberStore<Account>,
        IUserTwoFactorStore<Account>, IUserPasswordStore<Account>, IUserRoleStore<Account>, IUserSecurityStampStore<Account>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IdentityResult> CreateAsync(Account user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            //because of the lack of transaction cannot perform create on store


            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(Account user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //because of the lack of transaction cannot perform create on store

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<Account> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _unitOfWork.UserRepository.GetAsync(int.Parse(userId));

            return user;
        }

        public async Task<Account> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _unitOfWork.UserRepository.GetByPhoneAsync(normalizedUserName);

            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber.ToUpperInvariant());
        }

        public Task<string> GetUserIdAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task SetNormalizedUserNameAsync(Account user, string normalizedName, CancellationToken cancellationToken)
        {
            user.PhoneNumber = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(Account user, string userName, CancellationToken cancellationToken)
        {
            user.PhoneNumber = userName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(Account user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            //because of the lack of transaction cannot perform create on store

            return Task.FromResult(IdentityResult.Success);
        }

        public Task SetEmailAsync(Account user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Account user, CancellationToken cancellationToken)
        {

            return Task.FromResult(user.EmailConfirmed); //user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(Account user, bool confirmed, CancellationToken cancellationToken)
        {

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<Account> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(normalizedEmail);

            return user;
        }

        public Task<string> GetNormalizedEmailAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(Account user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(Account user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(Account user, CancellationToken cancellationToken)
        {

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(Account user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(Account user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task SetPasswordHashAsync(Account user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task AddToRoleAsync(Account user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //var normalizedName = roleName.ToUpper();
            //var role = await _unitOfWork.RoleRepository.GetAsync(normalizedName);

            //var userRole = new AccountRole() { User = user, Role = role }; 

            //await _unitOfWork.UserRoleRepository.CreateAsync(userRole);
            //because of the lack of transaction cannot perform create on store

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(Account user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //await _unitOfWork.UserRoleRepository.DeleteAsync(user, roleName);

            //because of the lack of transaction cannot perform create on store

            return Task.CompletedTask;
        }

        public async Task<IList<string>> GetRolesAsync(Account user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var data = await _unitOfWork.UserRoleRepository.GetUserRolesAsync(user);
            return data.Select(x => x.Name).ToList();
        }

        public async Task<bool> IsInRoleAsync(Account user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var roleList = await GetRolesAsync(user, cancellationToken);
            return roleList.Contains(roleName);
        }

        public async Task<IList<Account>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var users = await _unitOfWork.UserRoleRepository.GetUsersByRoleAsync(roleName);

            return users;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public Task<string> GetAuthenticatorKeyAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetAuthenticatorKeyAsync(Account user, string key, CancellationToken cancellationToken)
        {
            user.PasswordHash = key;

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(Account user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task SetSecurityStampAsync(Account user, string stamp, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}