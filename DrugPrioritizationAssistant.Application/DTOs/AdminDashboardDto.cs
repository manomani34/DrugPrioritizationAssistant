namespace DrugPrioritizationAssistant.Application.DTOs;

public class AdminDashboardDto
{
    public int TotalDrugs { get; set; }

    public int TotalNeedAssessments { get; set; }

    public int TotalFeasibilityAssessments { get; set; }

    public int TotalScoredDrugs { get; set; }

    public int TotalPrioritizedDrugs { get; set; }

    public decimal AverageNeedScore { get; set; }

    public decimal AverageFeasibilityScore { get; set; }

    public decimal AverageOpportunityScore { get; set; }

    public decimal AssessmentCompletionPercentage { get; set; }

    public int VeryHighPriorityCount { get; set; }

    public int HighPriorityCount { get; set; }

    public int MediumPriorityCount { get; set; }

    public int LowPriorityCount { get; set; }

    public List<AdminDashboardPriorityItemDto> TopPriorityDrugs
    { get; set; } = [];

    public List<AdminDashboardScorePointDto> ScorePoints
    { get; set; } = [];
}

public class AdminDashboardPriorityItemDto
{
    public int DrugId { get; set; }

    public string DrugName { get; set; } = string.Empty;

    public string? GenericName { get; set; }

    public decimal NeedScore { get; set; }

    public decimal FeasibilityScore { get; set; }

    public decimal OpportunityScore { get; set; }

    public int Rank { get; set; }

    public string PriorityLevel { get; set; } = string.Empty;
}

public class AdminDashboardScorePointDto
{
    public string DrugName { get; set; } = string.Empty;

    public decimal NeedScore { get; set; }

    public decimal FeasibilityScore { get; set; }

    public decimal OpportunityScore { get; set; }
}