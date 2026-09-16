using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrugPrioritizationAssistant.Web.Authorization;


namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class RolesController : AdminBaseController
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly UserManager<
        DrugPrioritizationAssistant.Infrastructure.Identity.ApplicationUser>
        _userManager;

    public RolesController(
        RoleManager<IdentityRole<int>> roleManager,
        UserManager<
            DrugPrioritizationAssistant.Infrastructure.Identity.ApplicationUser>
            userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // =========================================================
    // Index
    // =========================================================

    [HttpGet]
    [RequirePermission("Roles.View")]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();

        var model = new List<RoleListItemViewModel>();

        foreach (var role in roles)
        {
            var users =
                await _userManager.GetUsersInRoleAsync(
                    role.Name!);

            model.Add(
                new RoleListItemViewModel
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    UserCount = users.Count,
                    IsSystemRole =
                        role.Name == "SuperAdmin" ||
                        role.Name == "Admin" ||
                        role.Name == "Researcher" ||
                        role.Name == "Viewer"
                });
        }

        return View(model);
    }

    // =========================================================
    // Create - GET
    // =========================================================

    [HttpGet]
    [RequirePermission("Roles.Create")]
    public IActionResult Create()
    {
        return View();
    }

    // =========================================================
    // Create - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Roles.Create")]
    public async Task<IActionResult> Create(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                string.Empty,
                "نام نقش را وارد کنید.");

            return View();
        }

        name = name.Trim();

        if (await _roleManager.RoleExistsAsync(name))
        {
            ModelState.AddModelError(
                string.Empty,
                "این نقش قبلاً وجود دارد.");

            return View();
        }

        var role = new IdentityRole<int>
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant()
        };

        var result =
            await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View();
        }

        return RedirectToAction(
            nameof(Index));
    }

    // =========================================================
    // Delete
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Roles.Delete")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var role =
            await _roleManager.FindByIdAsync(
                id.ToString());

        if (role == null)
        {
            return NotFound();
        }

        // نقش‌های سیستمی قابل حذف نیستند.
        if (IsSystemRole(role.Name))
        {
            TempData["Error"] =
                "نقش‌های سیستمی قابل حذف نیستند.";

            return RedirectToAction(
                nameof(Index));
        }

        var users =
            await _userManager.GetUsersInRoleAsync(
                role.Name!);

        // اگر کاربری این نقش را دارد،
        // فعلاً اجازه حذف نمی‌دهیم.
        if (users.Count > 0)
        {
            TempData["Error"] =
                "این نقش دارای کاربر است و قابل حذف نیست.";

            return RedirectToAction(
                nameof(Index));
        }

        var result =
            await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            TempData["Error"] =
                "حذف نقش انجام نشد.";

            return RedirectToAction(
                nameof(Index));
        }

        TempData["Success"] =
            "نقش با موفقیت حذف شد.";

        return RedirectToAction(
            nameof(Index));
    }

    // =========================================================
    // Helpers
    // =========================================================

    private static bool IsSystemRole(
        string? roleName)
    {
        return roleName == "SuperAdmin" ||
               roleName == "Admin" ||
               roleName == "Researcher" ||
               roleName == "Viewer";
    }
}


// =============================================================
// ViewModel
// =============================================================

public class RoleListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int UserCount { get; set; }

    public bool IsSystemRole { get; set; }
}