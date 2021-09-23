namespace IdentityServer.Models
{
    public class PhoneNumberAuthenticateRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}