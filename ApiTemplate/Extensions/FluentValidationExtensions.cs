using FluentValidation.AspNetCore;

using Microsoft.Extensions.DependencyInjection;


namespace ApiTemplate.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddControllers().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining(typeof(Startup));
            });
        }
    }
}