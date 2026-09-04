using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.DashboardStatistics;
using SATH.Aitisis.Web.Areas.Citizen.ViewModels.Dashboard;
using System.Security.Claims;

namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class StatisticsController : Controller
    {
        private readonly IDashboardStatisticsService _dashboardStatisticsService;

        public StatisticsController(IDashboardStatisticsService dashboardStatisticsService)
        {
            _dashboardStatisticsService = dashboardStatisticsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var statistics = await _dashboardStatisticsService.GetCitizenStatisticsAsync(citizenId);

            var model = new CitizenDashboardViewModel
            {
                TotalApplications = statistics.TotalApplications,
                PendingApplications = statistics.PendingApplications,
                ApprovedApplications = statistics.ApprovedApplications,
                RejectedApplications = statistics.RejectedApplications
            };

            return View(model);
        }
    }
}