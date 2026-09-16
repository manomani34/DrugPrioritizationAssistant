using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Interfaces.Repositories;

public interface IScoringSettingsRepository
{
    Task<ScoringSettings?> GetAsync(
        CancellationToken cancellationToken = default);

    Task<ScoringSettings> AddAsync(
        ScoringSettings settings,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ScoringSettings settings,
        CancellationToken cancellationToken = default);
}