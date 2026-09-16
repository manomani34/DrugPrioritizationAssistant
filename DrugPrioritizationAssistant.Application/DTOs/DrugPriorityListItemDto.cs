namespace DrugPrioritizationAssistant.Application.DTOs;

public class DrugPriorityListItemDto
{
    public int DrugId { get; set; }

    public string DrugName { get; set; } = string.Empty;

    public string? GenericName { get; set; }

    public decimal NeedScore { get; set; }

    public decimal FeasibilityScore { get; set; }

    public decimal OpportunityScore { get; set; }

    public int Rank { get; set; }

    public string PriorityLevel { get; set; } = string.Empty;

    public bool HasNeedAssessment { get; set; }

    public bool HasFeasibilityAssessment { get; set; }

    public bool HasScore { get; set; }
}