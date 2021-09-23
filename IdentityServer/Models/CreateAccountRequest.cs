using System;

namespace IdentityServer.Models
{
    public class CreateAccountRequest
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Rolename { get; set; }
    }
}