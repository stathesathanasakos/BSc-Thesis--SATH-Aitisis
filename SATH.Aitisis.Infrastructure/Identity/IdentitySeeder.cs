using Microsoft.AspNetCore.Identity;
using SATH.Aitisis.App.Common.Constants;

namespace SATH.Aitisis.Infrastructure.Identity
{
    // here I seed the initial ASP.NET Core Identity roles required by the app
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // I inject the RoleManager service to manage roles and save them in AspNetRoles table 
            string[] roles =
            {
                AppRoles.Admin,
                AppRoles.Employee,
                AppRoles.Citizen
            };

            foreach (string role in roles)
            {
                // I check if the role already exists and if not, then I create it
                // so this way whenever the app starts, I will be sure that the roles are created and there are no duplicates
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }
        }


        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@sath-aitisis.local";
            // this isn;t good practice for the project to be deployed to production
            // the password should be stored in a secure way like in an environment variable
            const string adminPassword = "Admin123!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin != null)
            {
                return;
            }

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    admin,
                    AppRoles.Admin);
            }
        }
    }
}