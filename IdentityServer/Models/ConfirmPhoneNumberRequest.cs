using StorageCore.Domain.Entities;

namespace IdentityServer.Models
{
    public class ConfirmPhoneNumberRequest
    {
        public Account Account { get; set; } 
        public string Code { get; set; }
    }
}
