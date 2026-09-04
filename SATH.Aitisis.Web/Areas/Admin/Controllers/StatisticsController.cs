using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.DashboardStatistics;
using SATH.Aitisis.Web.Areas.Admin.ViewModels.Dashboard;

namespace SATH.Aitisis.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
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
            var statistics = await _dashboardStatisticsService.GetAdminStatisticsAsync();

            var model = new AdminDashboardViewModel
            {
                TotalApplications = statistics.TotalApplications,
                PendingApplications = statistics.PendingApplications,
                ApprovedApplications = statistics.ApprovedApplications,
                RejectedApplications = statistics.RejectedApplications,
                ActiveApplicationTypes = statistics.ActiveApplicationTypes
            };

            return View(model);
        }
    }
}