using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;

namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    //just a redirect
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}