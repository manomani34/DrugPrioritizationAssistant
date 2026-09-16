using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Application.Services;
using DrugPrioritizationAssistant.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class DrugPrioritiesController : AdminBaseController
{
    private readonly IDrugRepository _drugRepository;
    private readonly IDrugNeedAssessmentRepository _assessmentRepository;
    private readonly IProductionFeasibilityRepository _feasibilityRepository;
    private readonly IDrugScoreRepository _scoreRepository;
    private readonly IDrugPriorityExplanationService _explanationService;
    private readonly IScoringSettingsService _scoringSettingsService;

    public DrugPrioritiesController(
        IDrugRepository drugRepository,
        IDrugNeedAssessmentRepository assessmentRepository,
        IProductionFeasibilityRepository feasibilityRepository,
        IDrugScoreRepository scoreRepository,
        IDrugPriorityExplanationService explanationService,
        IScoringSettingsService scoringSettingsService)
    {
        _drugRepository = drugRepository;
        _assessmentRepository = assessmentRepository;
        _feasibilityRepository = feasibilityRepository;
        _scoreRepository = scoreRepository;
        _explanationService = explanationService;
        _scoringSettingsService = scoringSettingsService;
    }

    [HttpGet]
    [RequirePermission("DrugPriorities.View")]
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
            new List<DrugPriorityListItemDto>();

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

            // برای رتبه‌بندی معتبر، هر سه بخش باید وجود داشته باشند.
            if (assessment == null ||
                feasibility == null ||
                score == null)
            {
                continue;
            }

            result.Add(
                new DrugPriorityListItemDto
                {
                    DrugId = drug.Id,

                    DrugName = drug.Name,

                    GenericName =
                        drug.GenericName,

                    NeedScore =
                        assessment.NeedScore,

                    FeasibilityScore =
                        feasibility.FeasibilityScore,

                    OpportunityScore =
                        score.OpportunityScore,

                    Rank =
                        score.Rank,

                    PriorityLevel =
                        GetPriorityLevel(
                            score.OpportunityScore),

                    HasNeedAssessment = true,

                    HasFeasibilityAssessment = true,

                    HasScore = true
                });
        }

        result = result
            .OrderBy(x =>
                x.Rank > 0
                    ? 0
                    : 1)
            .ThenBy(x =>
                x.Rank > 0
                    ? x.Rank
                    : int.MaxValue)
            .ThenByDescending(
                x => x.OpportunityScore)
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

    [HttpGet]
    [RequirePermission("DrugPriorities.View")]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var drug =
            await _drugRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (drug == null)
        {
            return NotFound();
        }

        var assessment =
            await _assessmentRepository.GetByDrugIdAsync(
                id,
                cancellationToken);

        var feasibility =
            await _feasibilityRepository.GetByDrugIdAsync(
                id,
                cancellationToken);

        var score =
            await _scoreRepository.GetByDrugIdAsync(
                id,
                cancellationToken);

        if (assessment == null ||
            feasibility == null ||
            score == null)
        {
            TempData["ErrorMessage"] =
                "اطلاعات کافی برای تحلیل اولویت این دارو وجود ندارد.";

            return RedirectToAction(nameof(Index));
        }

        var explanation =
            _explanationService.Generate(
                drug,
                assessment,
                feasibility,
                score);

        var settings =
            await _scoringSettingsService.GetOrCreateAsync(
                cancellationToken);

        ViewBag.NeedWeight =
            settings.NeedWeight;

        ViewBag.FeasibilityWeight =
            settings.FeasibilityWeight;

        return View(explanation);
    }
}