using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Repositories;

public class ProductionFeasibilityRepository
    : IProductionFeasibilityRepository
{
    private readonly AppDbContext _context;

    public ProductionFeasibilityRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductionFeasibility?> GetByDrugIdAsync(
        int drugId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionFeasibilities
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.DrugId == drugId,
                cancellationToken);
    }

    public async Task<ProductionFeasibility?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionFeasibilities
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<ProductionFeasibility>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductionFeasibilities
            .Include(x => x.Drug)
            .AsNoTracking()
            .OrderByDescending(x => x.FeasibilityScore)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductionFeasibility> AddAsync(
        ProductionFeasibility feasibility,
        CancellationToken cancellationToken = default)
    {
        await _context.ProductionFeasibilities.AddAsync(
            feasibility,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return feasibility;
    }

    public async Task UpdateAsync(
        ProductionFeasibility feasibility,
        CancellationToken cancellationToken = default)
    {
        _context.ProductionFeasibilities.Update(
            feasibility);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}