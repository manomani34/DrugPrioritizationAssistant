using Microsoft.AspNetCore.Authorization;

namespace DrugPrioritizationAssistant.Web.Authorization;

public class PermissionRequirement
    : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionRequirement(
        string permissionName)
    {
        PermissionName = permissionName;
    }
}