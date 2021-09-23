using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


namespace ApiTemplate.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                var apiInfo = config.GetSection("ApiInfo").Get<ApiInfo>();

                c.SwaggerDoc($"{apiInfo.Version}", new OpenApiInfo
                {
                    Title = apiInfo.Title,
                    Version = apiInfo.Version,
                    Description = apiInfo.Description,
                });
                c.EnableAnnotations();


                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                      { jwtSecurityScheme, Array.Empty<string>() }
                });

            });
        }

        public static void UseSwaggerPipeline(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseSwagger();

            var apiInfo = config.GetSection("ApiInfo").Get<ApiInfo>();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{apiInfo.Version}/swagger.json", $"{apiInfo.Title}-{apiInfo.Version}");
                c.RoutePrefix = string.Empty;
            });
        }
    }

}
