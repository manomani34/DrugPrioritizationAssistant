using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        int userId,
        string permissionName,
        CancellationToken cancellationToken = default);

    Task<List<PermissionItemDto>> GetByRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default);

    Task SetRolePermissionsAsync(
        int roleId,
        IEnumerable<int> permissionIds,
        CancellationToken cancellationToken = default);
}

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;

    public PermissionService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(
        int userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        var roleIds =
            await _context.UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .ToListAsync(cancellationToken);

        if (roleIds.Count == 0)
        {
            return false;
        }

        return await _context.RolePermissions
            .AnyAsync(
                x =>
                    roleIds.Contains(x.RoleId) &&
                    x.Permission.Name == permissionName &&
                    x.Permission.IsActive,
                cancellationToken);
    }

    public async Task<List<PermissionItemDto>> GetByRoleAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(x => x.IsActive)
            .OrderBy(x => x.GroupName)
            .ThenBy(x => x.Name)
            .Select(
                x => new PermissionItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    GroupName = x.GroupName,
                    IsActive = x.IsActive,

                    IsAssigned =
                        _context.RolePermissions.Any(
                            rp =>
                                rp.RoleId == roleId &&
                                rp.PermissionId == x.Id)
                })
            .ToListAsync(cancellationToken);
    }

    public async Task SetRolePermissionsAsync(
        int roleId,
        IEnumerable<int> permissionIds,
        CancellationToken cancellationToken = default)
    {
        var roleExists =
            await _context.Roles
                .AnyAsync(
                    x => x.Id == roleId,
                    cancellationToken);

        if (!roleExists)
        {
            throw new InvalidOperationException(
                "نقش موردنظر پیدا نشد.");
        }

        var selectedIds =
            permissionIds
                .Distinct()
                .ToHashSet();

        var existing =
            await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync(cancellationToken);

        _context.RolePermissions.RemoveRange(existing);

        var validPermissionIds =
            await _context.Permissions
                .Where(
                    x =>
                        x.IsActive &&
                        selectedIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

        foreach (var permissionId in validPermissionIds)
        {
            _context.RolePermissions.Add(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
        }

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}

public class PermissionItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsAssigned { get; set; }
}