using System.Threading.Tasks;
using IdentityServer.Models;

namespace IdentityServer.Abstarct
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(EmailAuthenticateRequest request);
        Task<AuthenticateResponse> Authenticate(PhoneNumberAuthenticateRequest request);

        Task CreateAccountRequest(CreateAccountRequest request);
    }
}