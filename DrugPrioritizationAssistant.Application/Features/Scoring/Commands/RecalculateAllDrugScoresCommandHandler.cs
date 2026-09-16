using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Application.Services;
using DrugPrioritizationAssistant.Domain.Entities;
using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Scoring.Commands;

public class RecalculateAllDrugScoresCommandHandler
: IRequestHandler<RecalculateAllDrugScoresCommand>
{
    private readonly IDrugRepository _drugRepository;
    private readonly IDrugNeedAssessmentRepository _assessmentRepository;
    private readonly IProductionFeasibilityRepository _feasibilityRepository;
    private readonly IDrugScoreRepository _scoreRepository;
    private readonly IOpportunityScoreCalculator _scoreCalculator;
    private readonly IRankingService _rankingService;


public RecalculateAllDrugScoresCommandHandler(
    IDrugRepository drugRepository,
    IDrugNeedAssessmentRepository assessmentRepository,
    IProductionFeasibilityRepository feasibilityRepository,
    IDrugScoreRepository scoreRepository,
    IOpportunityScoreCalculator scoreCalculator,
    IRankingService rankingService)
    {
        _drugRepository = drugRepository;
        _assessmentRepository = assessmentRepository;
        _feasibilityRepository = feasibilityRepository;
        _scoreRepository = scoreRepository;
        _scoreCalculator = scoreCalculator;
        _rankingService = rankingService;
    }

    public async Task Handle(
        RecalculateAllDrugScoresCommand request,
        CancellationToken cancellationToken)
    {
        var drugs =
            await _drugRepository.GetAllAsync(
                cancellationToken);

        var assessments =
            await _assessmentRepository.GetAllAsync(
                cancellationToken);

        var feasibilities =
            await _feasibilityRepository.GetAllAsync(
                cancellationToken);

        var scores =
            await _scoreRepository.GetAllAsync(
                cancellationToken);

        var assessmentDictionary =
            assessments.ToDictionary(
                x => x.DrugId,
                x => x);

        var feasibilityDictionary =
            feasibilities.ToDictionary(
                x => x.DrugId,
                x => x);

        var scoreDictionary =
            scores.ToDictionary(
                x => x.DrugId,
                x => x);

        var newScores =
            new List<DrugScore>();

        var updatedScores =
            new List<DrugScore>();

        foreach (var drug in drugs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!assessmentDictionary.TryGetValue(
                    drug.Id,
                    out var assessment))
            {
                continue;
            }

            if (!feasibilityDictionary.TryGetValue(
                    drug.Id,
                    out var feasibility))
            {
                continue;
            }

            var opportunityScore =
                await _scoreCalculator.CalculateAsync(
                    assessment.NeedScore,
                    feasibility.FeasibilityScore,
                    cancellationToken);

            if (!scoreDictionary.TryGetValue(
                    drug.Id,
                    out var existingScore))
            {
                newScores.Add(
                    new DrugScore
                    {
                        DrugId = drug.Id,

                        NeedScore =
                            assessment.NeedScore,

                        FeasibilityScore =
                            feasibility.FeasibilityScore,

                        OpportunityScore =
                            opportunityScore,

                        Rank = 0,

                        CalculatedAt =
                            DateTime.UtcNow
                    });

                continue;
            }

            existingScore.NeedScore =
                assessment.NeedScore;

            existingScore.FeasibilityScore =
                feasibility.FeasibilityScore;

            existingScore.OpportunityScore =
                opportunityScore;

            existingScore.Rank = 0;

            existingScore.CalculatedAt =
                DateTime.UtcNow;

            updatedScores.Add(existingScore);
        }

        if (newScores.Count > 0)
        {
            await _scoreRepository.AddRangeAsync(
                newScores,
                cancellationToken);
        }

        if (updatedScores.Count > 0)
        {
            await _scoreRepository.UpdateRangeAsync(
                updatedScores,
                cancellationToken);
        }

        await _rankingService.RankAllAsync(
            cancellationToken);
    }


}
