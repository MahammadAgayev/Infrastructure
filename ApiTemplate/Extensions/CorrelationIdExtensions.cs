using CorrelationId.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static void AddCorrelationId(this IServiceCollection services)
        {
            services.AddDefaultCorrelationId(o =>
            {
                o.IgnoreRequestHeader = false;
                o.AddToLoggingScope = true;
                o.IncludeInResponse = true;
                o.ResponseHeader = "x-correlation-id";
                o.RequestHeader = "x-correlation-id";
            });
        }
    }
}
