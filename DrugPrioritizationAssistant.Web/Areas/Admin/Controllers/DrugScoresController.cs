using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Application.Features.Scoring.Commands;
using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Application.Services;
using DrugPrioritizationAssistant.Web.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class DrugScoresController : AdminBaseController
{
    private readonly IMediator _mediator;
    private readonly IDrugRepository _drugRepository;
    private readonly IDrugNeedAssessmentRepository _assessmentRepository;
    private readonly IProductionFeasibilityRepository _feasibilityRepository;
    private readonly IDrugScoreRepository _scoreRepository;
    private readonly IScoringSettingsService _scoringSettingsService;


public DrugScoresController(
    IMediator mediator,
    IDrugRepository drugRepository,
    IDrugNeedAssessmentRepository assessmentRepository,
    IProductionFeasibilityRepository feasibilityRepository,
    IDrugScoreRepository scoreRepository,
    IScoringSettingsService scoringSettingsService)
    {
        _mediator = mediator;
        _drugRepository = drugRepository;
        _assessmentRepository = assessmentRepository;
        _feasibilityRepository = feasibilityRepository;
        _scoreRepository = scoreRepository;
        _scoringSettingsService = scoringSettingsService;
    }

    [HttpGet]
    [RequirePermission("DrugScores.View")]
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

        var result =
            new List<DrugScoreListItemDto>();

        foreach (var drug in drugs)
        {
            assessmentDictionary.TryGetValue(
                drug.Id,
                out var assessment);

            feasibilityDictionary.TryGetValue(
                drug.Id,
                out var feasibility);

            scoreDictionary.TryGetValue(
                drug.Id,
                out var score);

            result.Add(
                new DrugScoreListItemDto
                {
                    DrugId =
                        drug.Id,

                    DrugName =
                        drug.Name,

                    GenericName =
                        drug.GenericName,

                    NeedScore =
                        assessment?.NeedScore,

                    FeasibilityScore =
                        feasibility?.FeasibilityScore,

                    OpportunityScore =
                        score?.OpportunityScore,

                    Rank =
                        score?.Rank > 0
                            ? score.Rank
                            : null,

                    HasNeedAssessment =
                        assessment != null,

                    HasFeasibilityAssessment =
                        feasibility != null,

                    HasScore =
                        score != null,

                    CalculatedAt =
                        score?.CalculatedAt
                });
        }

        result =
            result
                .OrderBy(x =>
                    x.Rank.HasValue &&
                    x.Rank.Value > 0
                        ? 0
                        : 1)
                .ThenBy(x =>
                    x.Rank.HasValue &&
                    x.Rank.Value > 0
                        ? x.Rank.Value
                        : int.MaxValue)
                .ThenBy(x =>
                    x.DrugName)
                .ToList();

        var settings =
            await _scoringSettingsService.GetOrCreateAsync(
                cancellationToken);

        ViewBag.NeedWeight =
            settings.NeedWeight;

        ViewBag.FeasibilityWeight =
            settings.FeasibilityWeight;

        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("DrugScores.Calculate")]
    public async Task<IActionResult> Calculate(
        int drugId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(
                new CalculateDrugScoreCommand(
                    drugId),
                cancellationToken);

            TempData["SuccessMessage"] =
                "امتیاز دارو با موفقیت محاسبه شد.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("DrugScores.Calculate")]
    public async Task<IActionResult> RankAll(
        CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(
                new RankAllDrugsCommand(),
                cancellationToken);

            TempData["SuccessMessage"] =
                "رتبه‌بندی داروها با موفقیت به‌روزرسانی شد.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "در هنگام به‌روزرسانی رتبه‌بندی خطایی رخ داد.";
        }

        return RedirectToAction(
            nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("DrugScores.Calculate")]
    public async Task<IActionResult> RecalculateAll(
        CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(
                new RecalculateAllDrugScoresCommand(),
                cancellationToken);

            TempData["SuccessMessage"] =
                "امتیاز همه داروها بر اساس وزن‌های فعلی مجدداً محاسبه و رتبه‌بندی شد.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "در هنگام محاسبه مجدد امتیاز داروها خطایی رخ داد.";
        }

        return RedirectToAction(
            nameof(Index));
    }


}
