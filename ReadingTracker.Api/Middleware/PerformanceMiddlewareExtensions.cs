namespace ReadingTracker.Api.Middleware;

public static class PerformanceMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMiddleware>();
    }
}