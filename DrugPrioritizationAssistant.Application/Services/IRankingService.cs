using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Services;

public interface IRankingService
{
    Task RankAllAsync(
        CancellationToken cancellationToken = default);
}