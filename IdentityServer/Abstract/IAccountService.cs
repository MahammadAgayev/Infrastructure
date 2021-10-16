using System.Data.Common;
using System.Threading.Tasks;
using IdentityServer.Models;
using StorageCore.Domain.Entities;

namespace IdentityServer.Abstract
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(PhoneNumberAuthenticateRequest request);

        Task<Account> CreateAccount(CreateAccountRequest request, DbTransaction transaction);
        Task<Account> CreateAccountSimulate(CreateAccountRequest request);

        Task<GeneratePhoneConfirmationResponse> GeneratePhoneNumerConfirmation(GeneratePhoneConfirmationRequest request);
        Task<bool> ConfirmPhoneNumber(ConfirmPhoneNumberRequest request, DbTransaction transaction);

        Task Logout();
    }
}