using Microsoft.AspNetCore.Identity;

namespace DrugPrioritizationAssistant.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        const string roleName = "SuperAdmin";

        // Create role if it doesn't exist
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new IdentityRole<int>
            {
                Name = roleName
            };

            var roleResult =
                await roleManager.CreateAsync(role);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    "خطا در ایجاد نقش SuperAdmin: " +
                    string.Join(
                        " | ",
                        roleResult.Errors.Select(x => x.Description)));
            }
        }

        const string userName = "admin";
        const string email = "admin@localhost.local";
        const string password = "Admin@12345";

        var user =
            await userManager.FindByNameAsync(userName);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,

                FirstName = "مدیر",
                LastName = "سیستم",

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var userResult =
                await userManager.CreateAsync(
                    user,
                    password);

            if (!userResult.Succeeded)
            {
                throw new Exception(
                    "خطا در ایجاد کاربر مدیر: " +
                    string.Join(
                        " | ",
                        userResult.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(
                user,
                roleName))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    roleName);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    "خطا در افزودن کاربر به نقش SuperAdmin: " +
                    string.Join(
                        " | ",
                        roleResult.Errors.Select(x => x.Description)));
            }
        }
    }
}