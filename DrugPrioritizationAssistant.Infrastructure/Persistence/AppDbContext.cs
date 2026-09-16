using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Drug> Drugs => Set<Drug>();

    public DbSet<DrugNeedAssessment> DrugNeedAssessments
        => Set<DrugNeedAssessment>();

    public DbSet<ProductionFeasibility> ProductionFeasibilities
        => Set<ProductionFeasibility>();

    public DbSet<DrugScore> DrugScores
        => Set<DrugScore>();

    public DbSet<ScoringSettings> ScoringSettings
        => Set<ScoringSettings>();

    public DbSet<Permission> Permissions
        => Set<Permission>();

    public DbSet<RolePermission> RolePermissions
        => Set<RolePermission>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // Decimal Precision
        // =====================================================

        modelBuilder.Entity<DrugNeedAssessment>()
            .Property(x => x.NeedScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ProductionFeasibility>()
            .Property(x => x.FeasibilityScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<DrugScore>()
            .Property(x => x.NeedScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<DrugScore>()
            .Property(x => x.FeasibilityScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<DrugScore>()
            .Property(x => x.OpportunityScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ScoringSettings>()
            .Property(x => x.NeedWeight)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ScoringSettings>()
            .Property(x => x.FeasibilityWeight)
            .HasPrecision(5, 2);


        // =====================================================
        // Drug -> Need Assessment
        // One Drug can have only one Need Assessment
        // =====================================================

        modelBuilder.Entity<Drug>()
            .HasOne(x => x.NeedAssessment)
            .WithOne(x => x.Drug)
            .HasForeignKey<DrugNeedAssessment>(
                x => x.DrugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DrugNeedAssessment>()
            .HasIndex(x => x.DrugId)
            .IsUnique();


        // =====================================================
        // Drug -> Production Feasibility
        // One Drug can have only one Production Feasibility
        // =====================================================

        modelBuilder.Entity<Drug>()
            .HasOne(x => x.ProductionFeasibility)
            .WithOne(x => x.Drug)
            .HasForeignKey<ProductionFeasibility>(
                x => x.DrugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductionFeasibility>()
            .HasIndex(x => x.DrugId)
            .IsUnique();


        // =====================================================
        // Drug -> Score
        // One Drug can have only one Score
        // =====================================================

        modelBuilder.Entity<Drug>()
            .HasOne(x => x.Score)
            .WithOne(x => x.Drug)
            .HasForeignKey<DrugScore>(
                x => x.DrugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DrugScore>()
            .HasIndex(x => x.DrugId)
            .IsUnique();


        // =====================================================
        // Permission
        // =====================================================

        modelBuilder.Entity<Permission>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .Property(x => x.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<Permission>()
            .Property(x => x.DisplayName)
            .HasMaxLength(200);

        modelBuilder.Entity<Permission>()
            .Property(x => x.GroupName)
            .HasMaxLength(100);


        // =====================================================
        // Role Permission
        // =====================================================

        modelBuilder.Entity<RolePermission>()
            .HasKey(x => new
            {
                x.RoleId,
                x.PermissionId
            });

        modelBuilder.Entity<RolePermission>()
            .HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}