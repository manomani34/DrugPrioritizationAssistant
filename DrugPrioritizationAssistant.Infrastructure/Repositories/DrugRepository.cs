using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Repositories;

public class DrugRepository : IDrugRepository
{
    private readonly AppDbContext _context;

    public DrugRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Drug> AddAsync(
        Drug drug,
        CancellationToken cancellationToken = default)
    {
        await _context.Drugs.AddAsync(
            drug,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return drug;
    }

    public async Task<Drug?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drugs
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Drug>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Drugs
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Drug drug,
        CancellationToken cancellationToken = default)
    {
        _context.Drugs.Update(drug);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Drug drug,
        CancellationToken cancellationToken = default)
    {
        _context.Drugs.Remove(drug);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}