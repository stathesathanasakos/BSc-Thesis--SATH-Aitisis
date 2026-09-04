using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;

namespace SATH.Aitisis.Web.Areas.Admin.Controllers
{
    // just a redirect
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}