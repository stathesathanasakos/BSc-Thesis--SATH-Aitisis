using Microsoft.Extensions.Caching.Memory;
using SATH.Aitisis.App.Services.DashboardStatistics;
using Microsoft.Extensions.Logging;

namespace SATH.Aitisis.Infrastructure.Caching
{
    public sealed class CachedStatisticsService : IDashboardStatisticsService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(70);
        private const string AdminStatisticsCacheKey = "statistics:admin";
        private const string EmployeeStatisticsCacheKeyPrefix = "statistics:employee:";
        private const string CitizenStatisticsCacheKeyPrefix = "statistics:citizen:";

        private readonly DashboardStatisticsService _innerService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedStatisticsService> _logger;

        public CachedStatisticsService(DashboardStatisticsService innerService, IMemoryCache cache, ILogger<CachedStatisticsService> logger)
        {
            _innerService = innerService;
            _cache = cache;
            _logger = logger;
        }


        public async Task<AdminDashboardStatistics> GetAdminStatisticsAsync()
        {
            var statistics = await _cache.GetOrCreateAsync(
                AdminStatisticsCacheKey,
                async cacheEntry =>
                {
                    // logs a cache miss for admin statistics
                    _logger.LogInformation("Statistics cache miss for key {CacheKey}. Loading statistics from database.",AdminStatisticsCacheKey);

                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await _innerService.GetAdminStatisticsAsync();
                });

            return statistics ?? new AdminDashboardStatistics();
        }

        public async Task<EmployeeDashboardStatistics> GetEmployeeStatisticsAsync(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return await _innerService.GetEmployeeStatisticsAsync(employeeId);
            }

            var cacheKey = $"{EmployeeStatisticsCacheKeyPrefix}{employeeId}";

            var statistics = await _cache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    // logs a cache miss for employee statistics
                    _logger.LogInformation("Statistics cache miss for key {CacheKey}. Loading statistics from database.", cacheKey);
                    
                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await _innerService.GetEmployeeStatisticsAsync(employeeId);
                });

            return statistics ?? new EmployeeDashboardStatistics();
        }

        public async Task<CitizenDashboardStatistics> GetCitizenStatisticsAsync(string citizenId)
        {
            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return await _innerService.GetCitizenStatisticsAsync(citizenId);
            }

            var cacheKey = $"{CitizenStatisticsCacheKeyPrefix}{citizenId}";

            var statistics = await _cache.GetOrCreateAsync(
                cacheKey,
                async cacheEntry =>
                {
                    // logs a cache miss for citizen statistics
                    _logger.LogInformation("Statistics cache miss for key {CacheKey}. Loading statistics from database.", cacheKey);

                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await _innerService.GetCitizenStatisticsAsync(citizenId);
                });

            return statistics ?? new CitizenDashboardStatistics();
        }
    }
}