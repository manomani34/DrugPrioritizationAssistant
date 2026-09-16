using DrugPrioritizationAssistant.Infrastructure.Identity;
using DrugPrioritizationAssistant.Infrastructure.Services;
using DrugPrioritizationAssistant.Web.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class PermissionsController : AdminBaseController
{
    private const string SuperAdminRole = "SuperAdmin";

    private readonly IPermissionService _permissionService;

    private readonly RoleManager<IdentityRole<int>>
        _roleManager;

    private readonly UserManager<ApplicationUser>
        _userManager;

    public PermissionsController(
        IPermissionService permissionService,
        RoleManager<IdentityRole<int>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _permissionService = permissionService;
        _roleManager = roleManager;
        _userManager = userManager;
    }


    [HttpGet]
    [RequirePermission("Permissions.View")]
    public async Task<IActionResult> Index(
        int? roleId)
    {
        var roles =
            _roleManager.Roles
                .OrderBy(x => x.Name)
                .ToList();

        ViewBag.Roles = roles;

        if (!roleId.HasValue)
        {
            return View(
                new List<PermissionItemDto>());
        }

        var role =
            await _roleManager.FindByIdAsync(
                roleId.Value.ToString());

        if (role == null)
        {
            return NotFound();
        }

        var permissions =
            await _permissionService.GetByRoleAsync(
                roleId.Value);

        ViewBag.SelectedRoleId = roleId.Value;
        ViewBag.SelectedRoleName = role.Name;

        return View(permissions);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Permissions.Assign")]
    public async Task<IActionResult> Save(
        int roleId,
        int[] permissionIds)
    {
        var role =
            await _roleManager.FindByIdAsync(
                roleId.ToString());

        if (role == null)
        {
            return NotFound();
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return Forbid();
        }

        var isSuperAdmin =
            await _userManager.IsInRoleAsync(
                currentUser,
                SuperAdminRole);

        if (!isSuperAdmin)
        {
            return Forbid();
        }

        await _permissionService.SetRolePermissionsAsync(
            roleId,
            permissionIds);

        TempData["Success"] =
            "دسترسی‌های نقش با موفقیت ذخیره شد.";

        return RedirectToAction(
            nameof(Index),
            new
            {
                roleId
            });
    }
}