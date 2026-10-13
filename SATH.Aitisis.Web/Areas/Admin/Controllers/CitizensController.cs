using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Citizens;
using SATH.Aitisis.Web.Areas.Admin.ViewModels.Citizens;

namespace SATH.Aitisis.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public sealed class CitizensController : Controller
    {
        private readonly ICitizenManagementService _citizenManagementService;

        public CitizensController(ICitizenManagementService citizenManagementService)
        {
            _citizenManagementService = citizenManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var citizens = await _citizenManagementService.GetCitizensAsync();

            var model = citizens
                .Select(x => new CitizenListViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var succeeded = await _citizenManagementService.ReactivateCitizenAsync(id);

            if (!succeeded)
            {
                TempData["ErrorMessage"] = "Citizen account could not be reactivated.";
            }
            else
            {
                TempData["SuccessMessage"] = "Citizen account reactivated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}