namespace DrugPrioritizationAssistant.Application.DTOs;

public class DrugPriorityExplanationDto
{
    public int DrugId { get; set; }

    public string DrugName { get; set; } = string.Empty;

    public decimal NeedScore { get; set; }

    public decimal FeasibilityScore { get; set; }

    public decimal OpportunityScore { get; set; }

    public int Rank { get; set; }

    public string PriorityLevel { get; set; } = string.Empty;

    public List<DrugPriorityReasonDto> Reasons { get; set; } = [];
}

public class DrugPriorityReasonDto
{
    public string Title { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}