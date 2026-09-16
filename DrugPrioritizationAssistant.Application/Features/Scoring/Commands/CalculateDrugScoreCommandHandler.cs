using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Application.Services;
using DrugPrioritizationAssistant.Domain.Entities;
using MediatR;

namespace DrugPrioritizationAssistant.Application.Features.Scoring.Commands;

public class CalculateDrugScoreCommandHandler
    : IRequestHandler<CalculateDrugScoreCommand, int>
{
    private readonly IDrugRepository _drugRepository;
    private readonly IDrugNeedAssessmentRepository _assessmentRepository;
    private readonly IProductionFeasibilityRepository _feasibilityRepository;
    private readonly IDrugScoreRepository _scoreRepository;
    private readonly IOpportunityScoreCalculator _scoreCalculator;
    private readonly IRankingService _rankingService;

    public CalculateDrugScoreCommandHandler(
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

    public async Task<int> Handle(
        CalculateDrugScoreCommand request,
        CancellationToken cancellationToken)
    {
        var drug =
            await _drugRepository.GetByIdAsync(
                request.DrugId,
                cancellationToken);

        if (drug == null)
        {
            throw new InvalidOperationException(
                "داروی مورد نظر یافت نشد.");
        }

        var assessment =
            await _assessmentRepository.GetByDrugIdAsync(
                request.DrugId,
                cancellationToken);

        if (assessment == null)
        {
            throw new InvalidOperationException(
                "ارزیابی نیاز دارو انجام نشده است.");
        }

        var feasibility =
            await _feasibilityRepository.GetByDrugIdAsync(
                request.DrugId,
                cancellationToken);

        if (feasibility == null)
        {
            throw new InvalidOperationException(
                "ارزیابی امکان تولید دارو انجام نشده است.");
        }

        var opportunityScore =
    await _scoreCalculator.CalculateAsync(
        assessment.NeedScore,
        feasibility.FeasibilityScore,
        cancellationToken);

        var existingScore =
            await _scoreRepository.GetByDrugIdAsync(
                request.DrugId,
                cancellationToken);

        int scoreId;

        if (existingScore == null)
        {
            var score = new DrugScore
            {
                DrugId = request.DrugId,

                NeedScore =
                    assessment.NeedScore,

                FeasibilityScore =
                    feasibility.FeasibilityScore,

                OpportunityScore =
                    opportunityScore,

                Rank = 0,

                CalculatedAt =
                    DateTime.UtcNow
            };

            var result =
                await _scoreRepository.AddAsync(
                    score,
                    cancellationToken);

            scoreId = result.Id;
        }
        else
        {
            existingScore.NeedScore =
                assessment.NeedScore;

            existingScore.FeasibilityScore =
                feasibility.FeasibilityScore;

            existingScore.OpportunityScore =
                opportunityScore;

            existingScore.CalculatedAt =
                DateTime.UtcNow;

            existingScore.Rank = 0;

            await _scoreRepository.UpdateAsync(
                existingScore,
                cancellationToken);

            scoreId = existingScore.Id;
        }

        // بعد از محاسبه امتیاز،
        // رتبه‌بندی تمام داروها را به‌روزرسانی می‌کنیم.
        await _rankingService.RankAllAsync(
            cancellationToken);

        return scoreId;
    }
}