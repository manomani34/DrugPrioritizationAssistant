using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Services;

public interface IDrugPriorityExplanationService
{
    DrugPriorityExplanationDto Generate(
        Drug drug,
        DrugNeedAssessment assessment,
        ProductionFeasibility feasibility,
        DrugScore score);
}