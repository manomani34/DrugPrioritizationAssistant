namespace DrugPrioritizationAssistant.Domain.Entities;

public class DrugScore
{
    public int Id { get; set; }

    public int DrugId { get; set; }

    public decimal NeedScore { get; set; }

    public decimal FeasibilityScore { get; set; }

    public decimal OpportunityScore { get; set; }

    public int Rank { get; set; }

    public DateTime CalculatedAt { get; set; }

    public Drug Drug { get; set; } = null!;
}