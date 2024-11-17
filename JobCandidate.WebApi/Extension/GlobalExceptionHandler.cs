using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using JobCandidate.Shared.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace JobCandidate.WebApi.Extension
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError("Exception occurred: {@Exception}, Type: {@Type}, UserName: {@UserName}, UserId: {@UserId}, Ip: {@Ip}",
                exception,
                "API");

            httpContext.Response.ContentType = "application/json";

            int statusCode;
            string title;
            var errors = new Dictionary<string, string[]>(); 

            if (exception is ValidationException validationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                title = "One or more validation errors occurred.";
                errors.Add("Comments", new[] { validationException.Message });
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                title = "An unexpected error occurred.";
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Detail = errors.Count > 0 ? null : "Please refer to the error description.",
                Extensions =
                {
                    ["errors"] = errors
                }
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(problemDetails, options);
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true;
        }
    }
}
