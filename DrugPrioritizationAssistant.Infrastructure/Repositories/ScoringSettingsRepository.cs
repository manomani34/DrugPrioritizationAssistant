using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrugPrioritizationAssistant.Infrastructure.Repositories;

public class ScoringSettingsRepository
    : IScoringSettingsRepository
{
    private readonly AppDbContext _context;

    public ScoringSettingsRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScoringSettings?> GetAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ScoringSettings
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    public async Task<ScoringSettings> AddAsync(
        ScoringSettings settings,
        CancellationToken cancellationToken = default)
    {
        await _context.ScoringSettings.AddAsync(
            settings,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return settings;
    }

    public async Task UpdateAsync(
        ScoringSettings settings,
        CancellationToken cancellationToken = default)
    {
        _context.ScoringSettings.Update(settings);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}