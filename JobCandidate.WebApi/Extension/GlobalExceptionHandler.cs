using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace JobCandidate.WebApi.Extension
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IServiceProvider _serviceProvider;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            _logger.LogError("Exception occurred: {@Exception}, Type: {@Type}, UserName: {@UserName}, UserId: {@UserId}, Ip: {@Ip}",
                exception,
                "API");

            httpContext.Response.ContentType = "application/json";
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            string errorMessage;
            int statusCode;

            statusCode = (int)HttpStatusCode.InternalServerError;
            errorMessage = "An internal server error occurred.";

            httpContext.Response.StatusCode = statusCode;
            var response = new
            {
                statusCode,
                errorMessage
            };
            var json = JsonSerializer.Serialize(response, options);
            await httpContext.Response.WriteAsync(json, cancellationToken);
            return true;
        }
    }
}
