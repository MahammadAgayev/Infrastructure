using System.Text;
using IdentityServer;
using IdentityServer.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StorageCore.Domain.Entities;

namespace ApiTemplate.Extensions
{
    public static class IdentityExtensions
    {
        public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AccountOptions>(configuration.GetSection("AccountOptions"));
            var options = configuration.GetSection("AccountOptions").Get<AccountOptions>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.Configure<IdentityOptions>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.Password.RequiredLength = 4;
            });

            services.AddIdentity<Account, Role>();

            services.AddTransient<IUserStore<Account>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();

            services.AddTransient<IAccountService, AccountService>();
        }
    }
}