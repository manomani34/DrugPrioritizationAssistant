namespace DrugPrioritizationAssistant.Domain.Entities;

public class ProductionFeasibility
{
    public int Id { get; set; }

    public int DrugId { get; set; }

    public int RawMaterialAvailability { get; set; }

    public int RawMaterialCost { get; set; }

    public int SynthesisComplexity { get; set; }

    public int EnzymaticRoutePotential { get; set; }

    public int ExpectedYield { get; set; }

    public int PurityPotential { get; set; }

    public int ScaleUpFeasibility { get; set; }

    public int EquipmentAvailability { get; set; }

    public int TechnicalRisk { get; set; }

    public decimal FeasibilityScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public Drug Drug { get; set; } = null!;
}