using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DrugPrioritizationAssistant.Web.Authorization;

public class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?>
        GetPolicyAsync(
            string policyName)
    {
        if (!policyName.StartsWith(
                "Permission:",
                StringComparison.OrdinalIgnoreCase))
        {
            return await base.GetPolicyAsync(
                policyName);
        }

        var permission =
            policyName["Permission:".Length..];

        var policy =
            new AuthorizationPolicyBuilder()
                .AddRequirements(
                    new PermissionRequirement(
                        permission))
                .Build();

        return policy;
    }
}