using Microsoft.AspNetCore.Identity;
using SATH.Aitisis.App.Common.Users;
using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.Infrastructure.Identity
{
    public sealed class UserLookupService : IUserLookupService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLookupService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileInfo?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserProfileInfo
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty
            };
        }
    }
}
