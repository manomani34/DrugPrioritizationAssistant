using DrugPrioritizationAssistant.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using DrugPrioritizationAssistant.Infrastructure.Persistence;

namespace DrugPrioritizationAssistant.Infrastructure.Identity;

public static class AuthorizationSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        RoleManager<IdentityRole<int>> roleManager)
    {
        // =====================================================
        // Roles
        // =====================================================

        string[] roles =
        [
            "SuperAdmin",
            "Admin",
            "Researcher",
            "Viewer"
        ];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<int>
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result =
                    await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"خطا در ایجاد نقش {roleName}: " +
                        string.Join(
                            " | ",
                            result.Errors.Select(
                                x => x.Description)));
                }
            }
        }


        // =====================================================
        // Permissions
        // =====================================================

        var permissions = new[]
        {
            // Drugs
            new Permission
            {
                Name = "Drugs.View",
                DisplayName = "مشاهده داروها",
                GroupName = "داروها"
            },

            new Permission
            {
                Name = "Drugs.Create",
                DisplayName = "ایجاد دارو",
                GroupName = "داروها"
            },

            new Permission
            {
                Name = "Drugs.Edit",
                DisplayName = "ویرایش دارو",
                GroupName = "داروها"
            },

            new Permission
            {
                Name = "Drugs.Delete",
                DisplayName = "حذف دارو",
                GroupName = "داروها"
            },


            // Need Assessment
            new Permission
            {
                Name = "NeedAssessment.View",
                DisplayName = "مشاهده ارزیابی نیاز",
                GroupName = "ارزیابی نیاز"
            },

            new Permission
            {
                Name = "NeedAssessment.Create",
                DisplayName = "ایجاد ارزیابی نیاز",
                GroupName = "ارزیابی نیاز"
            },

            new Permission
            {
                Name = "NeedAssessment.Edit",
                DisplayName = "ویرایش ارزیابی نیاز",
                GroupName = "ارزیابی نیاز"
            },

            new Permission
            {
                Name = "NeedAssessment.Delete",
                DisplayName = "حذف ارزیابی نیاز",
                GroupName = "ارزیابی نیاز"
            },


            // Production Feasibility
            new Permission
            {
                Name = "ProductionFeasibility.View",
                DisplayName = "مشاهده امکان تولید",
                GroupName = "امکان تولید"
            },

            new Permission
            {
                Name = "ProductionFeasibility.Create",
                DisplayName = "ایجاد ارزیابی امکان تولید",
                GroupName = "امکان تولید"
            },

            new Permission
            {
                Name = "ProductionFeasibility.Edit",
                DisplayName = "ویرایش امکان تولید",
                GroupName = "امکان تولید"
            },

            new Permission
            {
                Name = "ProductionFeasibility.Delete",
                DisplayName = "حذف ارزیابی امکان تولید",
                GroupName = "امکان تولید"
            },


            // Scoring
            new Permission
            {
                Name = "DrugScores.View",
                DisplayName = "مشاهده امتیازها",
                GroupName = "امتیازدهی"
            },

            new Permission
            {
                Name = "DrugScores.Calculate",
                DisplayName = "محاسبه امتیازها",
                GroupName = "امتیازدهی"
            },


            // Priorities
            new Permission
            {
                Name = "DrugPriorities.View",
                DisplayName = "مشاهده اولویت‌بندی",
                GroupName = "اولویت‌بندی"
            },


            // Scoring Settings
            new Permission
            {
                Name = "ScoringSettings.View",
                DisplayName = "مشاهده تنظیمات امتیازدهی",
                GroupName = "تنظیمات امتیازدهی"
            },

            new Permission
            {
                Name = "ScoringSettings.Edit",
                DisplayName = "ویرایش تنظیمات امتیازدهی",
                GroupName = "تنظیمات امتیازدهی"
            },


            // Users
            new Permission
            {
                Name = "Users.View",
                DisplayName = "مشاهده کاربران",
                GroupName = "کاربران"
            },

            new Permission
            {
                Name = "Users.Create",
                DisplayName = "ایجاد کاربر",
                GroupName = "کاربران"
            },

            new Permission
            {
                Name = "Users.Edit",
                DisplayName = "ویرایش کاربر",
                GroupName = "کاربران"
            },

            new Permission
            {
                Name = "Users.Deactivate",
                DisplayName = "غیرفعال کردن کاربر",
                GroupName = "کاربران"
            },


            // Roles
            new Permission
            {
                Name = "Roles.View",
                DisplayName = "مشاهده نقش‌ها",
                GroupName = "نقش‌ها"
            },

            new Permission
            {
                Name = "Roles.Create",
                DisplayName = "ایجاد نقش",
                GroupName = "نقش‌ها"
            },

            new Permission
            {
                Name = "Roles.Edit",
                DisplayName = "ویرایش نقش",
                GroupName = "نقش‌ها"
            },

            new Permission
            {
                Name = "Roles.Delete",
                DisplayName = "حذف نقش",
                GroupName = "نقش‌ها"
            },


            // Permissions
            new Permission
            {
                Name = "Permissions.View",
                DisplayName = "مشاهده دسترسی‌ها",
                GroupName = "دسترسی‌ها"
            },

            new Permission
            {
                Name = "Permissions.Assign",
                DisplayName = "اختصاص دسترسی",
                GroupName = "دسترسی‌ها"
            }
        };


        // =====================================================
        // Insert missing permissions
        // =====================================================

        foreach (var permission in permissions)
        {
            var exists =
                context.Permissions.Any(
                    x => x.Name == permission.Name);

            if (!exists)
            {
                permission.CreatedAt = DateTime.UtcNow;

                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();


        // =====================================================
        // SuperAdmin gets all permissions
        // =====================================================

        var superAdminRole =
            await roleManager.FindByNameAsync("SuperAdmin");

        if (superAdminRole == null)
        {
            throw new Exception(
                "نقش SuperAdmin پیدا نشد.");
        }


        var allPermissions =
            context.Permissions
                .Where(x => x.IsActive)
                .ToList();


        foreach (var permission in allPermissions)
        {
            var exists =
                context.RolePermissions.Any(
                    x =>
                        x.RoleId == superAdminRole.Id &&
                        x.PermissionId == permission.Id);

            if (!exists)
            {
                context.RolePermissions.Add(
                    new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id
                    });
            }
        }


        await context.SaveChangesAsync();
    }
}