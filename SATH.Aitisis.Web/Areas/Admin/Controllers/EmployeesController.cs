using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Employees;
using SATH.Aitisis.Web.Areas.Admin.ViewModels.Employees;


namespace SATH.Aitisis.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeManagementService _employeeService;

        public EmployeesController(IEmployeeManagementService employeeService)
        {
            _employeeService = employeeService;
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new CreateEmployeeRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };

            var result = await _employeeService.CreateEmployeeAsync(request);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Employee created successfully.";

            return RedirectToAction(nameof(Index));
        }




        // get the list of the employees
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            var model = employees
                .Select(x => new EmployeeListViewModel
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
        public async Task<IActionResult> Deactivate(string id)
        {
            var succeeded = await _employeeService.DeactivateEmployeeAsync(id);

            if (!succeeded)
            {
                TempData["ErrorMessage"] = "Employee could not be deactivated.";
            }
            else
            {
                TempData["SuccessMessage"] = "Employee deactivated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string id)
        {
            var succeeded = await _employeeService.ReactivateEmployeeAsync(id);

            if (!succeeded)
            {
                TempData["ErrorMessage"] = "Employee could not be reactivated.";
            }
            else
            {
                TempData["SuccessMessage"] = "Employee reactivated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}