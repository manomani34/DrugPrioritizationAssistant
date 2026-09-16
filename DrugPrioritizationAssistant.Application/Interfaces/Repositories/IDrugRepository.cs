using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Interfaces.Repositories;

public interface IDrugRepository
{
    Task<Drug> AddAsync(
        Drug drug,
        CancellationToken cancellationToken = default);

    Task<Drug?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<Drug>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Drug drug,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Drug drug,
        CancellationToken cancellationToken = default);
}