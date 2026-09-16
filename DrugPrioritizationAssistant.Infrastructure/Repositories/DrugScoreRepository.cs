using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Repositories;

public class DrugScoreRepository
: IDrugScoreRepository
{
    private readonly AppDbContext _context;


public DrugScoreRepository(
    AppDbContext context)
    {
        _context = context;
    }

    public async Task<DrugScore?> GetByDrugIdAsync(
        int drugId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugScores
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.DrugId == drugId,
                cancellationToken);
    }

    public async Task<DrugScore?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugScores
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<DrugScore>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugScores
            .Include(x => x.Drug)
            .OrderBy(x => x.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<DrugScore> AddAsync(
        DrugScore score,
        CancellationToken cancellationToken = default)
    {
        await _context.DrugScores.AddAsync(
            score,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return score;
    }

    public async Task UpdateAsync(
        DrugScore score,
        CancellationToken cancellationToken = default)
    {
        _context.DrugScores.Update(score);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IEnumerable<DrugScore> scores,
        CancellationToken cancellationToken = default)
    {
        _context.DrugScores.UpdateRange(scores);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<DrugScore> scores,
        CancellationToken cancellationToken = default)
    {
        await _context.DrugScores.AddRangeAsync(
            scores,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }


}
