using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReadingTracker.Api.Filters;

public class PerformanceAuditFilter : IActionFilter
{
    private readonly ILogger<PerformanceAuditFilter> _logger;
    private Stopwatch? _timer;

    // We can inject dependencies here because we will register this in the DI container
    public PerformanceAuditFilter(ILogger<PerformanceAuditFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // 1. THIS RUNS BEFORE THE CONTROLLER ACTION
        _timer = Stopwatch.StartNew();

        // Because this is a Filter (not Middleware), it knows EXACTLY which controller and method is running
        var controller = context.RouteData.Values["controller"];
        var action = context.RouteData.Values["action"];

        _logger.LogInformation($"[Audit] Starting execution of {controller}.{action}");

        // You can even inspect the incoming arguments!
        if (context.ActionArguments.ContainsKey("userBookId"))
        {
            _logger.LogInformation($"[Audit] Target UserBookId: {context.ActionArguments["userBookId"]}");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // 2. THIS RUNS AFTER THE CONTROLLER ACTION FINISHES
        _timer?.Stop();

        var action = context.RouteData.Values["action"];

        // We can inspect the outgoing result before it reaches the user
        var statusCode = context.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult
            ? objectResult.StatusCode
            : 200;

        _logger.LogInformation($"[Audit] Finished {action} in {_timer?.ElapsedMilliseconds}ms with status {statusCode}");
    }
}