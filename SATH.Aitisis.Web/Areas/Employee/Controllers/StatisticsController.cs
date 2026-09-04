using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.DashboardStatistics;
using SATH.Aitisis.Web.Areas.Employee.ViewModels.Dashboard;
using System.Security.Claims;

namespace SATH.Aitisis.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = AppRoles.Employee)]
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
            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Unauthorized();
            }

            var statistics = await _dashboardStatisticsService.GetEmployeeStatisticsAsync(employeeId);

            var model = new EmployeeDashboardViewModel
            {
                PendingApplications = statistics.PendingApplications,
                ApprovedByMe = statistics.ApprovedByMe,
                RejectedByMe = statistics.RejectedByMe,
                TotalDecisionsByMe = statistics.TotalDecisionsByMe
            };

            return View(model);
        }
    }
}