using System;
using System.Net;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;


namespace ApiTemplate.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;


        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AuthenticateException ex)
            {
                _logger.LogInformation($"authentication failed: {ex.Message}");

                await this.HandleErrorAsync(context, new ApiResponse
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Result = "login failed"
                });
            }
            catch (IdentityException ex)
            {
                _logger.LogInformation($"identity operation failed: {ex.Message}");

                await this.HandleErrorAsync(context, new ApiResponse
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Result = "login failed"
                });
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, $"failed to operate request");

                await this.HandleErrorAsync(context, new ApiResponse
                {
                    ResultCode = (int)HttpStatusCode.BadRequest,
                    Result = "login failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occured while processing request");

                await this.HandleErrorAsync(context, new ApiResponse
                {
                    ResultCode = (int)HttpStatusCode.InternalServerError,
                    Result = "Request failed."
                });
            }
        }

        private async Task HandleErrorAsync(HttpContext context, ApiResponse apiResponse)
        {
            var response = context.Response;

            response.ContentType = "application/json";

            response.StatusCode = apiResponse.ResultCode;

            string result = JsonConvert.SerializeObject(apiResponse);

            await response.WriteAsync(result);
        }
    }
}