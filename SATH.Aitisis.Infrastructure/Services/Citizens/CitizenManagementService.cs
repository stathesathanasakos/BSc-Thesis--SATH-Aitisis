using Microsoft.AspNetCore.Identity;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Citizens;
using SATH.Aitisis.Infrastructure.Identity;

namespace SATH.Aitisis.Infrastructure.Services.Citizens
{
    public sealed class CitizenManagementService : ICitizenManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CitizenManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IReadOnlyCollection<CitizenListItem>> GetCitizensAsync()
        {
            var citizens = await _userManager.GetUsersInRoleAsync(AppRoles.Citizen);

            return citizens
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new CitizenListItem
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

        public async Task<bool> ReactivateCitizenAsync(string citizenId)
        {
            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return false;
            }

            var citizen = await _userManager.FindByIdAsync(citizenId);

            if (citizen == null)
            {
                return false;
            }

            var isCitizen = await _userManager.IsInRoleAsync(citizen, AppRoles.Citizen);

            if (!isCitizen)
            {
                return false;
            }

            if (citizen.IsActive)
            {
                return true;
            }

            citizen.IsActive = true;

            var result = await _userManager.UpdateAsync(citizen);

            return result.Succeeded;
        }
    }
}