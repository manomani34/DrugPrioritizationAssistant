using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Services;

public interface IScoringSettingsService
{
    Task<ScoringSettings> GetOrCreateAsync(
        CancellationToken cancellationToken = default);

    Task<ScoringSettings> UpdateAsync(
        decimal needWeight,
        decimal feasibilityWeight,
        CancellationToken cancellationToken = default);
}