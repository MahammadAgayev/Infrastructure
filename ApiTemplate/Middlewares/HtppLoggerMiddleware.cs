using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace IdentityService.MIddlewares
{
    public class HttpLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<HttpLoggerMiddleware> _logger;

        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;


        public HttpLoggerMiddleware(RequestDelegate next, ILogger<HttpLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }


        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            await this.logRequest(context);

            await this.logResponse(context, sw);
        }


        private async Task logRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                string requestBody = HttpLoggerMiddleware.readStreamInChunks(requestStream);

                _logger.LogInformation("request received. {RequestUrl} {RequestBody} {QueryString}", context.Request.Path, requestBody, context.Request.QueryString);

                context.Request.Body.Position = 0;
            }
        }

        private async Task logResponse(HttpContext context, Stopwatch sw)
        {
            var originalBodyStream = context.Response.Body;

            await using (var responseBodyStream = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation("response replied {ResponseBody} {TotalMilliseconds}", responseBody, sw.Elapsed.TotalMilliseconds);

                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
        }

        private static string readStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using (var textWriter = new StringWriter())
            {
                using (var reader = new StreamReader(stream))
                {
                    var readChunk = new char[readChunkBufferLength];
                    int readChunkLength;

                    do
                    {
                        readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                        textWriter.Write(readChunk, 0, readChunkLength);
                    } while (readChunkLength > 0);

                    return textWriter.ToString();
                }
            }
        }
    }
}