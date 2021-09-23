using IdentityServer;
using IdentityServer.Abstarct;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Extensions
{
    public static  class AddIdentityExtensions
    {
        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<UserStore, RoleStore>();

            services.AddScoped<IAccountService, AccountService>();
        }
    }
}