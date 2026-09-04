using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;

namespace SATH.Aitisis.Web.Areas.Employee.Controllers
{
    // just a redirect
    [Area("Employee")]
    [Authorize(Roles = AppRoles.Employee)]
    public sealed class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}