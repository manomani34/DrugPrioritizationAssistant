using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    private readonly IDrugRepository _drugRepository;
    private readonly IDrugNeedAssessmentRepository _assessmentRepository;
    private readonly IProductionFeasibilityRepository _feasibilityRepository;
    private readonly IDrugScoreRepository _scoreRepository;


public HomeController(
    IDrugRepository drugRepository,
    IDrugNeedAssessmentRepository assessmentRepository,
    IProductionFeasibilityRepository feasibilityRepository,
    IDrugScoreRepository scoreRepository)
    {
        _drugRepository = drugRepository;
        _assessmentRepository = assessmentRepository;
        _feasibilityRepository = feasibilityRepository;
        _scoreRepository = scoreRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
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

        var drugDictionary =
            drugs.ToDictionary(
                x => x.Id,
                x => x);

        var assessmentDictionary =
            assessments.ToDictionary(
                x => x.DrugId,
                x => x);

        var feasibilityDictionary =
            feasibilities.ToDictionary(
                x => x.DrugId,
                x => x);

        var scoredDrugs =
            scores
                .Where(x => x.OpportunityScore > 0)
                .OrderBy(x =>
                    x.Rank > 0
                        ? x.Rank
                        : int.MaxValue)
                .ThenByDescending(
                    x => x.OpportunityScore)
                .ToList();

        var priorityItems =
            new List<AdminDashboardPriorityItemDto>();

        foreach (var score in scoredDrugs)
        {
            if (!drugDictionary.TryGetValue(
                    score.DrugId,
                    out var drug))
            {
                continue;
            }

            assessmentDictionary.TryGetValue(
                drug.Id,
                out var assessment);

            feasibilityDictionary.TryGetValue(
                drug.Id,
                out var feasibility);

            priorityItems.Add(
                new AdminDashboardPriorityItemDto
                {
                    DrugId =
                        drug.Id,

                    DrugName =
                        drug.Name,

                    GenericName =
                        drug.GenericName,

                    NeedScore =
                        assessment?.NeedScore
                        ?? score.NeedScore,

                    FeasibilityScore =
                        feasibility?.FeasibilityScore
                        ?? score.FeasibilityScore,

                    OpportunityScore =
                        score.OpportunityScore,

                    Rank =
                        score.Rank,

                    PriorityLevel =
                        GetPriorityLevel(
                            score.OpportunityScore)
                });
        }

        var averageNeedScore =
            priorityItems.Count > 0
                ? priorityItems.Average(
                    x => x.NeedScore)
                : 0m;

        var averageFeasibilityScore =
            priorityItems.Count > 0
                ? priorityItems.Average(
                    x => x.FeasibilityScore)
                : 0m;

        var averageOpportunityScore =
            priorityItems.Count > 0
                ? priorityItems.Average(
                    x => x.OpportunityScore)
                : 0m;

        var assessmentCompletionPercentage =
            drugs.Count > 0
                ? Math.Round(
                    assessments.Count * 100m / drugs.Count,
                    1)
                : 0m;

        var model =
            new AdminDashboardDto
            {
                TotalDrugs =
                    drugs.Count,

                TotalNeedAssessments =
                    assessments.Count,

                TotalFeasibilityAssessments =
                    feasibilities.Count,

                TotalScoredDrugs =
                    scores.Count,

                TotalPrioritizedDrugs =
                    scores.Count(
                        x => x.Rank > 0),

                AverageNeedScore =
                    Math.Round(
                        averageNeedScore,
                        1),

                AverageFeasibilityScore =
                    Math.Round(
                        averageFeasibilityScore,
                        1),

                AverageOpportunityScore =
                    Math.Round(
                        averageOpportunityScore,
                        1),

                AssessmentCompletionPercentage =
                    assessmentCompletionPercentage,

                VeryHighPriorityCount =
                    priorityItems.Count(
                        x =>
                            x.PriorityLevel ==
                            "خیلی بالا"),

                HighPriorityCount =
                    priorityItems.Count(
                        x =>
                            x.PriorityLevel ==
                            "بالا"),

                MediumPriorityCount =
                    priorityItems.Count(
                        x =>
                            x.PriorityLevel ==
                            "متوسط"),

                LowPriorityCount =
                    priorityItems.Count(
                        x =>
                            x.PriorityLevel ==
                            "پایین"),

                TopPriorityDrugs =
                    priorityItems
                        .Take(5)
                        .ToList(),

                ScorePoints =
                    priorityItems
                        .Select(
                            x =>
                                new AdminDashboardScorePointDto
                                {
                                    DrugName =
                                        x.DrugName,

                                    NeedScore =
                                        x.NeedScore,

                                    FeasibilityScore =
                                        x.FeasibilityScore,

                                    OpportunityScore =
                                        x.OpportunityScore
                                })
                        .ToList()
            };

        return View(model);
    }

    private static string GetPriorityLevel(
        decimal opportunityScore)
    {
        if (opportunityScore >= 80)
        {
            return "خیلی بالا";
        }

        if (opportunityScore >= 65)
        {
            return "بالا";
        }

        if (opportunityScore >= 50)
        {
            return "متوسط";
        }

        return "پایین";
    }


}
