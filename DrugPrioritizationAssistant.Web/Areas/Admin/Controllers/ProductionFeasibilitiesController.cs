using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Domain.Entities;
using DrugPrioritizationAssistant.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class ProductionFeasibilitiesController : AdminBaseController
{
    private readonly IDrugRepository _drugRepository;

    private readonly IProductionFeasibilityRepository
        _feasibilityRepository;

    public ProductionFeasibilitiesController(
        IDrugRepository drugRepository,
        IProductionFeasibilityRepository feasibilityRepository)
    {
        _drugRepository = drugRepository;
        _feasibilityRepository = feasibilityRepository;
    }

    // ---------------------------------------------------------
    // Index
    // ---------------------------------------------------------

    [HttpGet]
    [RequirePermission("ProductionFeasibility.View")]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var feasibilities =
            await _feasibilityRepository.GetAllAsync(
                cancellationToken);

        return View(feasibilities);
    }


    // ---------------------------------------------------------
    // Create - GET
    // ---------------------------------------------------------

    [HttpGet]
    [RequirePermission("ProductionFeasibility.Create")]
    public async Task<IActionResult> Create(
        int drugId,
        CancellationToken cancellationToken)
    {
        var drug =
            await _drugRepository.GetByIdAsync(
                drugId,
                cancellationToken);

        if (drug == null)
        {
            return NotFound();
        }

        var existingFeasibility =
            await _feasibilityRepository.GetByDrugIdAsync(
                drugId,
                cancellationToken);

        if (existingFeasibility != null)
        {
            TempData["ErrorMessage"] =
                "برای این دارو قبلاً ارزیابی امکان تولید ثبت شده است.";

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Drug = drug;

        return View();
    }


    // ---------------------------------------------------------
    // Create - POST
    // ---------------------------------------------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ProductionFeasibility.Create")]
    public async Task<IActionResult> Create(
        int drugId,
        int rawMaterialAvailability,
        int rawMaterialCost,
        int synthesisComplexity,
        int enzymaticRoutePotential,
        int expectedYield,
        int purityPotential,
        int scaleUpFeasibility,
        int equipmentAvailability,
        int technicalRisk,
        CancellationToken cancellationToken)
    {
        var drug =
            await _drugRepository.GetByIdAsync(
                drugId,
                cancellationToken);

        if (drug == null)
        {
            return NotFound();
        }

        if (!IsValidScore(rawMaterialAvailability) ||
            !IsValidScore(rawMaterialCost) ||
            !IsValidScore(synthesisComplexity) ||
            !IsValidScore(enzymaticRoutePotential) ||
            !IsValidScore(expectedYield) ||
            !IsValidScore(purityPotential) ||
            !IsValidScore(scaleUpFeasibility) ||
            !IsValidScore(equipmentAvailability) ||
            !IsValidScore(technicalRisk))
        {
            ModelState.AddModelError(
                "",
                "تمام شاخص‌ها باید عددی بین ۰ تا ۱۰۰ باشند.");

            ViewBag.Drug = drug;

            return View();
        }

        var existingFeasibility =
            await _feasibilityRepository.GetByDrugIdAsync(
                drugId,
                cancellationToken);

        if (existingFeasibility != null)
        {
            TempData["ErrorMessage"] =
                "برای این دارو قبلاً ارزیابی امکان تولید ثبت شده است.";

            return RedirectToAction(nameof(Index));
        }

        var feasibilityScore =
            CalculateFeasibilityScore(
                rawMaterialAvailability,
                rawMaterialCost,
                synthesisComplexity,
                enzymaticRoutePotential,
                expectedYield,
                purityPotential,
                scaleUpFeasibility,
                equipmentAvailability,
                technicalRisk);

        var feasibility =
            new ProductionFeasibility
            {
                DrugId = drugId,

                RawMaterialAvailability =
                    rawMaterialAvailability,

                RawMaterialCost =
                    rawMaterialCost,

                SynthesisComplexity =
                    synthesisComplexity,

                EnzymaticRoutePotential =
                    enzymaticRoutePotential,

                ExpectedYield =
                    expectedYield,

                PurityPotential =
                    purityPotential,

                ScaleUpFeasibility =
                    scaleUpFeasibility,

                EquipmentAvailability =
                    equipmentAvailability,

                TechnicalRisk =
                    technicalRisk,

                FeasibilityScore =
                    feasibilityScore,

                CreatedAt =
                    DateTime.UtcNow
            };

        await _feasibilityRepository.AddAsync(
            feasibility,
            cancellationToken);

        TempData["SuccessMessage"] =
            "ارزیابی امکان تولید دارو با موفقیت ثبت شد.";

        return RedirectToAction(nameof(Index));
    }


    // ---------------------------------------------------------
    // Edit - GET
    // ---------------------------------------------------------

    [HttpGet]
    [RequirePermission("ProductionFeasibility.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var feasibility =
            await _feasibilityRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (feasibility == null)
        {
            return NotFound();
        }

        return View(feasibility);
    }


    // ---------------------------------------------------------
    // Edit - POST
    // ---------------------------------------------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ProductionFeasibility.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        int rawMaterialAvailability,
        int rawMaterialCost,
        int synthesisComplexity,
        int enzymaticRoutePotential,
        int expectedYield,
        int purityPotential,
        int scaleUpFeasibility,
        int equipmentAvailability,
        int technicalRisk,
        CancellationToken cancellationToken)
    {
        var feasibility =
            await _feasibilityRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (feasibility == null)
        {
            return NotFound();
        }

        if (!IsValidScore(rawMaterialAvailability) ||
            !IsValidScore(rawMaterialCost) ||
            !IsValidScore(synthesisComplexity) ||
            !IsValidScore(enzymaticRoutePotential) ||
            !IsValidScore(expectedYield) ||
            !IsValidScore(purityPotential) ||
            !IsValidScore(scaleUpFeasibility) ||
            !IsValidScore(equipmentAvailability) ||
            !IsValidScore(technicalRisk))
        {
            ModelState.AddModelError(
                "",
                "تمام شاخص‌ها باید عددی بین ۰ تا ۱۰۰ باشند.");

            return View(feasibility);
        }

        feasibility.RawMaterialAvailability =
            rawMaterialAvailability;

        feasibility.RawMaterialCost =
            rawMaterialCost;

        feasibility.SynthesisComplexity =
            synthesisComplexity;

        feasibility.EnzymaticRoutePotential =
            enzymaticRoutePotential;

        feasibility.ExpectedYield =
            expectedYield;

        feasibility.PurityPotential =
            purityPotential;

        feasibility.ScaleUpFeasibility =
            scaleUpFeasibility;

        feasibility.EquipmentAvailability =
            equipmentAvailability;

        feasibility.TechnicalRisk =
            technicalRisk;

        feasibility.FeasibilityScore =
            CalculateFeasibilityScore(
                rawMaterialAvailability,
                rawMaterialCost,
                synthesisComplexity,
                enzymaticRoutePotential,
                expectedYield,
                purityPotential,
                scaleUpFeasibility,
                equipmentAvailability,
                technicalRisk);

        await _feasibilityRepository.UpdateAsync(
            feasibility,
            cancellationToken);

        TempData["SuccessMessage"] =
            "ارزیابی امکان تولید با موفقیت به‌روزرسانی شد.";

        return RedirectToAction(nameof(Index));
    }


    // ---------------------------------------------------------
    // Validation
    // ---------------------------------------------------------

    private static bool IsValidScore(int value)
    {
        return value >= 0 && value <= 100;
    }


    // ---------------------------------------------------------
    // Feasibility Score
    // ---------------------------------------------------------

    private static decimal CalculateFeasibilityScore(
        int rawMaterialAvailability,
        int rawMaterialCost,
        int synthesisComplexity,
        int enzymaticRoutePotential,
        int expectedYield,
        int purityPotential,
        int scaleUpFeasibility,
        int equipmentAvailability,
        int technicalRisk)
    {
        var rawMaterialCostScore =
            100 - rawMaterialCost;

        var synthesisComplexityScore =
            100 - synthesisComplexity;

        var technicalRiskScore =
            100 - technicalRisk;

        var score =
            (
                rawMaterialAvailability +
                rawMaterialCostScore +
                synthesisComplexityScore +
                enzymaticRoutePotential +
                expectedYield +
                purityPotential +
                scaleUpFeasibility +
                equipmentAvailability +
                technicalRiskScore
            ) / 9m;

        return Math.Round(
            score,
            2);
    }
}