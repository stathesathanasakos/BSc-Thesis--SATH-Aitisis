using SATH.Aitisis.App.Services.Applications;

namespace SATH.Aitisis.Web.BackgroundServices
{
    public sealed class DraftApplicationCleanupHostedService : BackgroundService
    {
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
        private static readonly TimeSpan DraftLifetime = TimeSpan.FromHours(24);
        // for test
        //private static readonly TimeSpan DraftLifetime = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DraftApplicationCleanupHostedService> _logger;

        public DraftApplicationCleanupHostedService(IServiceScopeFactory scopeFactory, ILogger<DraftApplicationCleanupHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunCleanupAsync(stoppingToken);
            using var timer = new PeriodicTimer(CleanupInterval);

            try{
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunCleanupAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) {
                // Application is shutting down.
            }
        }

        private async Task RunCleanupAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested){
                return;
            }

            try{
                using var scope = _scopeFactory.CreateScope();
                var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                var deletedCount = await applicationService.DeleteExpiredDraftApplicationsAsync(DraftLifetime);

                if (deletedCount > 0)
                {
                    _logger.LogInformation("Deleted {DeletedCount} expired draft application(s).", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting expired draft applications.");
            }
        }
    }
}
