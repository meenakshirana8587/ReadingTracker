using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ReadingTracker.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // 1. Log the full exception and stack trace internally for the developer
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing request {Method} {Path}. Message: {Message}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);

            // 2. Set the HTTP response status code to 500
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // 3. Write a safe RFC 7807 ProblemDetails JSON response using the built-in service
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred on the server. Please try again later.",
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                }
            });
        }
    }
}
