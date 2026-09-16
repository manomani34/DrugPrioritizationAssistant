using DrugPrioritizationAssistant.Domain.Enums;

namespace DrugPrioritizationAssistant.Domain.Entities;

public class Drug
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? GenericName { get; set; }

    public string? ActiveIngredient { get; set; }

    public string? DosageForm { get; set; }

    public string? TherapeuticCategory { get; set; }

    public DrugStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DrugNeedAssessment? NeedAssessment { get; set; }

    public ProductionFeasibility? ProductionFeasibility { get; set; }

    public DrugScore? Score { get; set; }
}