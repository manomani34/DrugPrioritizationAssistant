using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Interfaces.Repositories;

public interface IDrugNeedAssessmentRepository
{
    Task<DrugNeedAssessment?> GetByDrugIdAsync(
        int drugId,
        CancellationToken cancellationToken = default);

    Task<DrugNeedAssessment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<DrugNeedAssessment>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DrugNeedAssessment> AddAsync(
        DrugNeedAssessment assessment,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        DrugNeedAssessment assessment,
        CancellationToken cancellationToken = default);
}