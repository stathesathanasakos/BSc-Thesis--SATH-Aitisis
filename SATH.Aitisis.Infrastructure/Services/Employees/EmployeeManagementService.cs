using Microsoft.AspNetCore.Identity;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Employees;
using SATH.Aitisis.Infrastructure.Identity;

namespace SATH.Aitisis.Infrastructure.Services.Employees
{
    // Implements employee account management using ASP.NET Core Identity
    // implements the IEmployeeManagementService interface to provide functionality for creating employee accounts, checking for existing users, and assigning roles
    // It uses UserManager<ApplicationUser> to manage user accounts and handle password hashing and validation.
    public sealed class EmployeeManagementService: IEmployeeManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateEmployeeResult> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new CreateEmployeeResult
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                        "An account with this email already exists."
                    }
                };
            }

            var employee = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(employee, request.Password);

            if (!createResult.Succeeded)
            {
                return new CreateEmployeeResult
                {
                    Succeeded = false,
                    Errors = createResult.Errors
                        .Select(x => x.Description)
                        .ToArray()
                };
            }

            var roleResult =
                await _userManager.AddToRoleAsync(
                    employee,
                    AppRoles.Employee);

            if (!roleResult.Succeeded)
            {
                // Roll back the account if role assignment fails
                // so that the system does not have an employee account without the proper role
                await _userManager.DeleteAsync(employee);

                return new CreateEmployeeResult
                {
                    Succeeded = false,
                    Errors = roleResult.Errors
                        .Select(x => x.Description)
                        .ToArray()
                };
            }

            return new CreateEmployeeResult
            {
                Succeeded = true
            };
        }


        public async Task<IReadOnlyCollection<EmployeeListItem>> GetEmployeesAsync()
        {
            var employees = await _userManager.GetUsersInRoleAsync(
                AppRoles.Employee);

            return employees
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new EmployeeListItem
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email ?? string.Empty,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToArray();
        }




        public async Task<bool> DeactivateEmployeeAsync(string employeeId)
        {
            var employee = await _userManager.FindByIdAsync(employeeId);

            if (employee == null)
            {
                return false;
            }

            var isEmployee = await _userManager.IsInRoleAsync(
                employee,
                AppRoles.Employee);

            if (!isEmployee)
            {
                return false;
            }

            employee.IsActive = false;

            var result = await _userManager.UpdateAsync(employee);

            return result.Succeeded;
        }



        public async Task<bool> ReactivateEmployeeAsync(string employeeId)
        {
            var employee = await _userManager.FindByIdAsync(employeeId);

            if (employee == null)
            {
                return false;
            }

            var isEmployee = await _userManager.IsInRoleAsync(
                employee,
                AppRoles.Employee);

            if (!isEmployee)
            {
                return false;
            }

            employee.IsActive = true;

            var result = await _userManager.UpdateAsync(employee);

            return result.Succeeded;
        }


    }
}