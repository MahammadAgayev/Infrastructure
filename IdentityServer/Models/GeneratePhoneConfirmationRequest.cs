using StorageCore.Domain.Entities;

namespace IdentityServer.Models
{
    public class GeneratePhoneConfirmationRequest
    {
        public Account Account { get; set; }
    }
}