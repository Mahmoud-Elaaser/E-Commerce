using ECommerce.Models;
using ECommerce.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ECommerce.Data
{
    public static class IdentityDataInitializer
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Manager = "Manager";
        }

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminOptions = scopedProvider.GetRequiredService<IOptions<AdminOptions>>();


            string[] roleNames = { Roles.Admin, Roles.User, Roles.Manager };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            await CreateDefaultAdminAsync(userManager, adminOptions);


        }

        private static async Task CreateDefaultAdminAsync(
            UserManager<ApplicationUser> userManager,
            IOptions<AdminOptions> adminOptions)
        {
            var options = adminOptions.Value;
            var adminEmail = options.Email;
            var adminPassword = options.Password;
            var adminName = options.FullName;
            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                return;
            }



            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FullName = adminName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);

                    await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("IsAdministrator", "true"));
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create admin user: {errors}");
                }
            }
            else
            {
                /// Ensure existing admin has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }

                /// Ensure admin claim exists
                var claims = await userManager.GetClaimsAsync(adminUser);
                if (!claims.Any(c => c.Type == "IsAdministrator"))
                {
                    await userManager.AddClaimAsync(adminUser,
                        new System.Security.Claims.Claim("IsAdministrator", "true"));
                }
            }
        }
    }
}