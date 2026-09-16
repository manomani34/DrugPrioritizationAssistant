using DrugPrioritizationAssistant.Application.Interfaces.Repositories;

namespace DrugPrioritizationAssistant.Application.Services;

public class RankingService : IRankingService
{
    private readonly IDrugScoreRepository _scoreRepository;


public RankingService(
    IDrugScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository;
    }

    public async Task RankAllAsync(
        CancellationToken cancellationToken = default)
    {
        var scores =
            await _scoreRepository.GetAllAsync(
                cancellationToken);

        foreach (var score in scores)
        {
            score.Rank = 0;
        }

        var rankedScores = scores
            .Where(x => x.OpportunityScore > 0)
            .OrderByDescending(x => x.OpportunityScore)
            .ThenByDescending(x => x.NeedScore)
            .ToList();

        var rank = 1;

        foreach (var score in rankedScores)
        {
            score.Rank = rank;
            rank++;
        }

        await _scoreRepository.UpdateRangeAsync(
            scores,
            cancellationToken);
    }


}
