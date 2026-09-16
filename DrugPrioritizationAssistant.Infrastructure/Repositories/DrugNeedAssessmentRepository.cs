using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Repositories;

public class DrugNeedAssessmentRepository
    : IDrugNeedAssessmentRepository
{
    private readonly AppDbContext _context;

    public DrugNeedAssessmentRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<DrugNeedAssessment?> GetByDrugIdAsync(
        int drugId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugNeedAssessments
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.DrugId == drugId,
                cancellationToken);
    }

    public async Task<DrugNeedAssessment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugNeedAssessments
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<DrugNeedAssessment>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DrugNeedAssessments
            .Include(x => x.Drug)
            .AsNoTracking()
            .OrderByDescending(x => x.NeedScore)
            .ToListAsync(cancellationToken);
    }

    public async Task<DrugNeedAssessment> AddAsync(
        DrugNeedAssessment assessment,
        CancellationToken cancellationToken = default)
    {
        await _context.DrugNeedAssessments.AddAsync(
            assessment,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return assessment;
    }

    public async Task UpdateAsync(
        DrugNeedAssessment assessment,
        CancellationToken cancellationToken = default)
    {
        _context.DrugNeedAssessments.Update(assessment);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}