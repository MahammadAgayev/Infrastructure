using System.Text.Json.Serialization;

namespace IdentityServer.Models
{
    public class AuthenticateResponse
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}