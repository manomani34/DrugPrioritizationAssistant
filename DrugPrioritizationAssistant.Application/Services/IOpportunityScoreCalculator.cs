namespace DrugPrioritizationAssistant.Application.Services;

public interface IOpportunityScoreCalculator
{
    Task<decimal> CalculateAsync(
        decimal needScore,
        decimal feasibilityScore,
        CancellationToken cancellationToken = default);
}