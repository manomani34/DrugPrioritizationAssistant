namespace DrugPrioritizationAssistant.Domain.Entities;

public class ScoringSettings
{
    public int Id { get; set; }

    public decimal NeedWeight { get; set; }

    public decimal FeasibilityWeight { get; set; }

    public DateTime UpdatedAt { get; set; }
}