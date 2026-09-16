namespace DrugPrioritizationAssistant.Domain.Entities;

public class DrugNeedAssessment
{
    public int Id { get; set; }

    public int DrugId { get; set; }

    public int ShortageSeverity { get; set; }

    public int ImportDependency { get; set; }

    public int DomesticProductionLevel { get; set; }

    public int DemandLevel { get; set; }

    public int TherapeuticImportance { get; set; }

    public int SupplyInstability { get; set; }

    public decimal NeedScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public Drug Drug { get; set; } = null!;
}