using DrugPrioritizationAssistant.Infrastructure.Identity;
using DrugPrioritizationAssistant.Web.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class UsersController : AdminBaseController
{
    private const string SuperAdminRole = "SuperAdmin";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    // =========================================================
    // Users List
    // =========================================================

    [HttpGet]
    [RequirePermission("Users.View")]
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users
            .OrderBy(x => x.UserName)
            .ToList();

        var model = new List<UserListItemViewModel>();

        foreach (var user in users)
        {
            var roles =
                await _userManager.GetRolesAsync(user);

            model.Add(
                new UserListItemViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                });
        }

        return View(model);
    }


    // =========================================================
    // Create - GET
    // =========================================================

    [HttpGet]
    [RequirePermission("Users.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadRolesAsync();

        return View();
    }


    // =========================================================
    // Create - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Users.Create")]
    public async Task<IActionResult> Create(
        string userName,
        string email,
        string firstName,
        string lastName,
        string password,
        string role)
    {
        await LoadRolesAsync();

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                string.Empty,
                "نام کاربری، ایمیل و رمز عبور الزامی هستند.");

            return View();
        }

        if (string.IsNullOrWhiteSpace(role) ||
            !await _roleManager.RoleExistsAsync(role))
        {
            ModelState.AddModelError(
                string.Empty,
                "نقش انتخاب‌شده معتبر نیست.");

            return View();
        }

        // فقط SuperAdmin می‌تواند کاربر جدید را با نقش SuperAdmin بسازد.
        if (string.Equals(
                role,
                SuperAdminRole,
                StringComparison.OrdinalIgnoreCase))
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            var isCurrentUserSuperAdmin =
                currentUser != null &&
                await _userManager.IsInRoleAsync(
                    currentUser,
                    SuperAdminRole);

            if (!isCurrentUserSuperAdmin)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "شما مجوز ایجاد کاربر با نقش SuperAdmin را ندارید.");

                return View();
            }
        }

        userName = userName.Trim();
        email = email.Trim();

        var existingUser =
            await _userManager.FindByNameAsync(userName);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                string.Empty,
                "این نام کاربری قبلاً ثبت شده است.");

            return View();
        }

        var existingEmail =
            await _userManager.FindByEmailAsync(email);

        if (existingEmail != null)
        {
            ModelState.AddModelError(
                string.Empty,
                "این ایمیل قبلاً ثبت شده است.");

            return View();
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName?.Trim() ?? string.Empty,
            LastName = lastName?.Trim() ?? string.Empty,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result =
            await _userManager.CreateAsync(
                user,
                password);

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

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View();
        }

        TempData["Success"] =
            "کاربر با موفقیت ایجاد شد.";

        return RedirectToAction(
            nameof(Index));
    }


    // =========================================================
    // Toggle Active
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Users.Deactivate")]
    public async Task<IActionResult> ToggleActive(
        int id)
    {
        var user =
            await _userManager.FindByIdAsync(
                id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        // جلوگیری از غیرفعال کردن حساب خود کاربر
        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser != null &&
            currentUser.Id == user.Id)
        {
            TempData["Error"] =
                "نمی‌توانید حساب کاربری خودتان را غیرفعال کنید.";

            return RedirectToAction(
                nameof(Index));
        }

        user.IsActive = !user.IsActive;

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            TempData["Error"] =
                "تغییر وضعیت کاربر انجام نشد.";

            return RedirectToAction(
                nameof(Index));
        }

        TempData["Success"] =
            user.IsActive
                ? "کاربر فعال شد."
                : "کاربر غیرفعال شد.";

        return RedirectToAction(
            nameof(Index));
    }


    // =========================================================
    // Helpers
    // =========================================================

    private async Task LoadRolesAsync()
    {
        var currentUser =
            await _userManager.GetUserAsync(User);

        var isCurrentUserSuperAdmin =
            currentUser != null &&
            await _userManager.IsInRoleAsync(
                currentUser,
                SuperAdminRole);

        var rolesQuery =
            _roleManager.Roles
                .Where(x => x.Name != null);

        // SuperAdmin فقط برای خود SuperAdmin قابل انتخاب است.
        if (!isCurrentUserSuperAdmin)
        {
            rolesQuery =
                rolesQuery.Where(
                    x => x.Name != SuperAdminRole);
        }

        ViewBag.Roles =
            await rolesQuery
                .OrderBy(x => x.Name)
                .Select(x => x.Name!)
                .ToListAsync();
    }
}


// =============================================================
// ViewModel
// =============================================================

public class UserListItemViewModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<string> Roles { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}