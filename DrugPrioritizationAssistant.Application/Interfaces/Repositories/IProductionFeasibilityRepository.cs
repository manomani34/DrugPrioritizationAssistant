using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Interfaces.Repositories;

public interface IProductionFeasibilityRepository
{
    Task<ProductionFeasibility?> GetByDrugIdAsync(
        int drugId,
        CancellationToken cancellationToken = default);

    Task<ProductionFeasibility?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<ProductionFeasibility>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductionFeasibility> AddAsync(
        ProductionFeasibility feasibility,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProductionFeasibility feasibility,
        CancellationToken cancellationToken = default);
}