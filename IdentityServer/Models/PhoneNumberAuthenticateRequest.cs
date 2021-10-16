using StorageCore.Domain.Entities;

namespace IdentityServer.Models
{
    public class PhoneNumberAuthenticateRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }

        public bool CreateToken { get; set; }
        public bool CheckConfirmation { get; set; }
    }
}