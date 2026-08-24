using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.Infrastructure.Identity;


namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Deactivate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Your account could not be deactivated. Please try again.";

                return RedirectToAction(nameof(Deactivate));
            }

            await _signInManager.SignOutAsync();

            TempData["SuccessMessage"] ="Your account has been deactivated.";

            return RedirectToAction(
                actionName: "Index",
                controllerName: "Home",
                routeValues: new { area = "" });
        }
    }
}