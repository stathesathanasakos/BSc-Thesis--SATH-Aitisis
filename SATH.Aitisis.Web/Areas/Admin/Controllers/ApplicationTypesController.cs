using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.ApplicationTypes;
using SATH.Aitisis.Web.Areas.Admin.ViewModels.ApplicationTypes;


namespace SATH.Aitisis.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class ApplicationTypesController : Controller
    {
        private readonly IApplicationTypeService _applicationTypeService;


        public ApplicationTypesController(IApplicationTypeService applicationTypeService)
        {
            _applicationTypeService = applicationTypeService;
        }




        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applicationTypes = await _applicationTypeService.GetApplicationTypesAsync();

            var model = applicationTypes
                .Select(x => new ApplicationTypeListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    RequiresPayment = x.RequiresPayment,
                    FeeAmount = x.FeeAmount,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            return View(model);
        }






        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateApplicationTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new CreateApplicationTypeRequest
            {
                Name = model.Name,
                Description = model.Description,
                RequiresPayment = model.RequiresPayment,
                FeeAmount = model.FeeAmount
            };

            var result = await _applicationTypeService.CreateApplicationTypeAsync(request);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Application type created successfully.";

            return RedirectToAction(nameof(Index));
        }



        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var details = await _applicationTypeService.GetDetailsAsync(id);

            if (details == null)
            {
                return NotFound();
            }

            var model = new ApplicationTypeDetailsViewModel
            {
                Id = details.Id,
                Name = details.Name,
                Description = details.Description,
                RequiresPayment = details.RequiresPayment,
                FeeAmount = details.FeeAmount,
                IsActive = details.IsActive,

                Fields = details.Fields
                    .Select(x => new ApplicationTypeFieldViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        FieldType = x.FieldType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList(),

                DocumentRequirements = details.DocumentRequirements
                    .Select(x => new DocumentRequirementViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList()
            };

            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddField(AddApplicationTypeFieldViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid field information.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.ApplicationTypeId });
            }

            var request = new AddApplicationTypeFieldRequest
            {
                ApplicationTypeId = model.ApplicationTypeId,
                Name = model.Name,
                Label = model.Label,
                FieldType = model.FieldType,
                IsRequired = model.IsRequired
            };

            var succeeded = await _applicationTypeService.AddFieldAsync(request);

            if (!succeeded)
            {
                TempData["ErrorMessage"] ="The field could not be added. Check that the application type exists and that the technical name is unique.";
            }
            else
            {
                TempData["SuccessMessage"] ="Field added successfully.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = model.ApplicationTypeId });
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDocumentRequirement(AddDocumentRequirementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid document requirement information.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.ApplicationTypeId });
            }

            var request = new AddDocumentRequirementRequest
            {
                ApplicationTypeId = model.ApplicationTypeId,
                Name = model.Name,
                Description = model.Description,
                IsRequired = model.IsRequired
            };

            var succeeded =
                await _applicationTypeService.AddDocumentRequirementAsync(request);

            if (!succeeded)
            {
                TempData["ErrorMessage"] ="The document requirement could not be added.";
            }
            else
            {
                TempData["SuccessMessage"] = "Document requirement added successfully.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = model.ApplicationTypeId });
        }




        // application type deactivation and reactivation actions for admin users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var succeeded = await _applicationTypeService.DeactivateAsync(id);

            if (!succeeded)
            {
                TempData["ErrorMessage"] = "The application type could not be deactivated.";
            }
            else
            {
                TempData["SuccessMessage"] = "Application type deactivated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var succeeded = await _applicationTypeService.ReactivateAsync(id);

            if (!succeeded)
            {
                TempData["ErrorMessage"] = "The application type could not be reactivated.";
            }
            else
            {
                TempData["SuccessMessage"] = "Application type reactivated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}