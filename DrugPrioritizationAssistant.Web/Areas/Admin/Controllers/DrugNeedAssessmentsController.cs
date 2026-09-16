using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class DrugNeedAssessmentsController : AdminBaseController
{
    private readonly IDrugRepository _drugRepository;

    private readonly IDrugNeedAssessmentRepository
        _assessmentRepository;

    public DrugNeedAssessmentsController(
        IDrugRepository drugRepository,
        IDrugNeedAssessmentRepository assessmentRepository)
    {
        _drugRepository = drugRepository;
        _assessmentRepository = assessmentRepository;
    }


    // GET: /Admin/DrugNeedAssessments
    [HttpGet]
    [RequirePermission("NeedAssessment.View")]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var assessments =
            await _assessmentRepository.GetAllAsync(
                cancellationToken);

        return View(assessments);
    }


    // GET: /Admin/DrugNeedAssessments/Create?drugId=1
    [HttpGet]
    [RequirePermission("NeedAssessment.Create")]
    public async Task<IActionResult> Create(
        int drugId,
        CancellationToken cancellationToken)
    {
        var drug = await _drugRepository.GetByIdAsync(
            drugId,
            cancellationToken);

        if (drug == null)
        {
            return NotFound();
        }

        var existingAssessment =
            await _assessmentRepository.GetByDrugIdAsync(
                drugId,
                cancellationToken);

        if (existingAssessment != null)
        {
            TempData["ErrorMessage"] =
                "برای این دارو قبلاً ارزیابی نیاز ثبت شده است.";

            return RedirectToAction(
                nameof(Index));
        }

        ViewBag.Drug = drug;

        return View();
    }


    // POST: /Admin/DrugNeedAssessments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("NeedAssessment.Create")]
    public async Task<IActionResult> Create(
        int drugId,
        int shortageSeverity,
        int importDependency,
        int domesticProductionLevel,
        int demandLevel,
        int therapeuticImportance,
        int supplyInstability,
        CancellationToken cancellationToken)
    {
        var drug = await _drugRepository.GetByIdAsync(
            drugId,
            cancellationToken);

        if (drug == null)
        {
            return NotFound();
        }

        if (!IsValidScore(shortageSeverity) ||
            !IsValidScore(importDependency) ||
            !IsValidScore(domesticProductionLevel) ||
            !IsValidScore(demandLevel) ||
            !IsValidScore(therapeuticImportance) ||
            !IsValidScore(supplyInstability))
        {
            ModelState.AddModelError(
                "",
                "تمام شاخص‌ها باید عددی بین ۰ تا ۱۰۰ باشند.");

            ViewBag.Drug = drug;

            return View();
        }

        var existingAssessment =
            await _assessmentRepository.GetByDrugIdAsync(
                drugId,
                cancellationToken);

        if (existingAssessment != null)
        {
            TempData["ErrorMessage"] =
                "برای این دارو قبلاً ارزیابی نیاز ثبت شده است.";

            return RedirectToAction(
                nameof(Index));
        }

        var needScore =
            CalculateNeedScore(
                shortageSeverity,
                importDependency,
                domesticProductionLevel,
                demandLevel,
                therapeuticImportance,
                supplyInstability);

        var assessment = new DrugNeedAssessment
        {
            DrugId = drugId,

            ShortageSeverity = shortageSeverity,

            ImportDependency = importDependency,

            DomesticProductionLevel =
                domesticProductionLevel,

            DemandLevel = demandLevel,

            TherapeuticImportance =
                therapeuticImportance,

            SupplyInstability =
                supplyInstability,

            NeedScore = needScore,

            CreatedAt = DateTime.UtcNow
        };

        await _assessmentRepository.AddAsync(
            assessment,
            cancellationToken);

        TempData["SuccessMessage"] =
            "ارزیابی نیاز دارو با موفقیت ثبت شد.";

        return RedirectToAction(
            nameof(Index));
    }


    // GET: /Admin/DrugNeedAssessments/Edit/1
    [HttpGet]
    [RequirePermission("NeedAssessment.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var assessment =
            await _assessmentRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (assessment == null)
        {
            return NotFound();
        }

        return View(assessment);
    }


    // POST: /Admin/DrugNeedAssessments/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("NeedAssessment.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        int shortageSeverity,
        int importDependency,
        int domesticProductionLevel,
        int demandLevel,
        int therapeuticImportance,
        int supplyInstability,
        CancellationToken cancellationToken)
    {
        var assessment =
            await _assessmentRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (assessment == null)
        {
            return NotFound();
        }

        if (!IsValidScore(shortageSeverity) ||
            !IsValidScore(importDependency) ||
            !IsValidScore(domesticProductionLevel) ||
            !IsValidScore(demandLevel) ||
            !IsValidScore(therapeuticImportance) ||
            !IsValidScore(supplyInstability))
        {
            ModelState.AddModelError(
                "",
                "تمام شاخص‌ها باید عددی بین ۰ تا ۱۰۰ باشند.");

            return View(assessment);
        }

        assessment.ShortageSeverity =
            shortageSeverity;

        assessment.ImportDependency =
            importDependency;

        assessment.DomesticProductionLevel =
            domesticProductionLevel;

        assessment.DemandLevel =
            demandLevel;

        assessment.TherapeuticImportance =
            therapeuticImportance;

        assessment.SupplyInstability =
            supplyInstability;

        assessment.NeedScore =
            CalculateNeedScore(
                shortageSeverity,
                importDependency,
                domesticProductionLevel,
                demandLevel,
                therapeuticImportance,
                supplyInstability);

        await _assessmentRepository.UpdateAsync(
            assessment,
            cancellationToken);

        TempData["SuccessMessage"] =
            "ارزیابی نیاز با موفقیت به‌روزرسانی شد.";

        return RedirectToAction(
            nameof(Index));
    }


    private static bool IsValidScore(
        int value)
    {
        return value >= 0 && value <= 100;
    }


    private static decimal CalculateNeedScore(
        int shortageSeverity,
        int importDependency,
        int domesticProductionLevel,
        int demandLevel,
        int therapeuticImportance,
        int supplyInstability)
    {
        /*
         * فعلاً وزن همه شاخص‌ها برابر است.
         *
         * سطح تولید داخلی شاخص معکوس است.
         *
         * یعنی:
         * تولید داخلی 100
         * => نیاز کمتر
         *
         * بنابراین در محاسبه:
         *
         * DomesticProductionLevel
         * تبدیل می‌شود به:
         *
         * 100 - DomesticProductionLevel
         */

        var domesticProductionNeed =
            100 - domesticProductionLevel;

        var score =
            (
                shortageSeverity +
                importDependency +
                domesticProductionNeed +
                demandLevel +
                therapeuticImportance +
                supplyInstability
            ) / 6m;

        return Math.Round(
            score,
            2);
    }
}