using DrugPrioritizationAssistant.Infrastructure.Identity;
using DrugPrioritizationAssistant.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DrugPrioritizationAssistant.Web.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly IPermissionService
        _permissionService;

    public PermissionAuthorizationHandler(
        UserManager<ApplicationUser> userManager,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var user =
            await _userManager.GetUserAsync(
                context.User);

        if (user == null)
        {
            return;
        }

        if (!user.IsActive)
        {
            return;
        }

        // SuperAdmin همیشه مجاز است.
        if (await _userManager.IsInRoleAsync(
                user,
                "SuperAdmin"))
        {
            context.Succeed(requirement);
            return;
        }

        var hasPermission =
            await _permissionService.HasPermissionAsync(
                user.Id,
                requirement.PermissionName);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}