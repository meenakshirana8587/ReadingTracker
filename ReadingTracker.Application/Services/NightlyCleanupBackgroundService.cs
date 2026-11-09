using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReadingTracker.Infrastructure;

namespace ReadingTracker.Application.Services;

public class NightlyCleanupBackgroundService : BackgroundService
{
    private readonly ILogger<NightlyCleanupBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory; // The Singleton's lifeline to Scoped services

    public NightlyCleanupBackgroundService(
        ILogger<NightlyCleanupBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Nightly Cleanup Service is starting.");

        // The while loop keeps the service running forever until the app shuts down
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Executing nightly database cleanup...");

                // 1. Create a fresh Scope (fixes the Captive Dependency problem)
                using (var scope = _scopeFactory.CreateScope())
                {
                    // 2. Safely resolve the Scoped DbContext
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 3. Do database work (e.g., deleting reading sessions that are marked as 'Trash')
                    // await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ReadingSessions WHERE IsDeleted = true", stoppingToken);
                }

                _logger.LogInformation("Cleanup finished successfully.");

                // 4. Put the background thread to sleep for 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // This exception is expected when the server is gracefully shutting down
                // and the stoppingToken is triggered during the Task.Delay
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A fatal error occurred during the background cleanup job.");
            }
        }

        _logger.LogInformation("Nightly Cleanup Service is gracefully stopping.");
    }
}