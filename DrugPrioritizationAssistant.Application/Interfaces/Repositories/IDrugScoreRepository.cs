using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Interfaces.Repositories;

public interface IDrugScoreRepository
{
    Task<DrugScore?> GetByDrugIdAsync(
    int drugId,
    CancellationToken cancellationToken = default);


Task<DrugScore?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default);

    Task<List<DrugScore>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DrugScore> AddAsync(
        DrugScore score,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        DrugScore score,
        CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(
        IEnumerable<DrugScore> scores,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<DrugScore> scores,
        CancellationToken cancellationToken = default);


}
