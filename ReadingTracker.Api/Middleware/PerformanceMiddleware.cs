using System.Diagnostics;

namespace ReadingTracker.Api.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    // Constructor accepts the next middleware delegate and logger (Singleton lifetime)
    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Start stopwatch on the way IN
        var stopwatch = Stopwatch.StartNew();

        // 2. Hook into the response headers before the rest of the pipeline executes
        // We use OnStarting to modify headers *before* the response status code is sent to the client
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            context.Response.Headers["X-Response-Time-Ms"] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        try
        {
            // 3. Pass control to the next middleware in the pipeline (Controllers, Auth, etc.)
            await _next(context);
        }
        finally
        {
            // Even if an exception occurs, ensure stopwatch stops and we log execution time
            if (!stopwatch.IsRunning)
            {
                // Already stopped in OnStarting
            }
            else
            {
                stopwatch.Stop();
            }

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}