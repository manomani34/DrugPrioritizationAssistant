namespace DrugPrioritizationAssistant.Application.Services;

public class OpportunityScoreCalculator
    : IOpportunityScoreCalculator
{
    private readonly IScoringSettingsService _settingsService;

    public OpportunityScoreCalculator(
        IScoringSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<decimal> CalculateAsync(
        decimal needScore,
        decimal feasibilityScore,
        CancellationToken cancellationToken = default)
    {
        var settings =
            await _settingsService.GetOrCreateAsync(
                cancellationToken);

        var opportunityScore =
            (needScore * settings.NeedWeight / 100m) +
            (feasibilityScore * settings.FeasibilityWeight / 100m);

        return Math.Round(
            opportunityScore,
            2);
    }
}