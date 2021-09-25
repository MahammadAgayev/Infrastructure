using ApiTemplate.Extensions;
using AspNetCoreRateLimit;
using CorrelationId;
using ApiTemplate.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace ApiTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddVersioning();

            services.AddCorrelationId();

            services.AddFluentValidation();

            services.AddRateLimit(this.Configuration);

            services.AddSwagger(this.Configuration);

            services.AddDb(this.Configuration);

            services.AddIdentity(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime,
           ILogger<Startup> logger)
        {
            applicationLifetime.ApplicationStopped.Register(() => logger.LogInformation("Application stopped."));

            app.UseIpRateLimiting();

            app.UseCorrelationId();

            app.UseHttpMetrics();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwaggerPipeline(this.Configuration);
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<HttpLoggerMiddleware>();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
