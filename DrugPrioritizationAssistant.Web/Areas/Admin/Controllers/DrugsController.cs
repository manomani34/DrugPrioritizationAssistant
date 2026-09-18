using DrugPrioritizationAssistant.Application.Features.Drugs.Commands;
using DrugPrioritizationAssistant.Application.Interfaces.Repositories;
using DrugPrioritizationAssistant.Web.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class DrugsController : AdminBaseController
{
    private readonly IDrugRepository _drugRepository;
    private readonly IMediator _mediator;


public DrugsController(
    IDrugRepository drugRepository,
    IMediator mediator)
    {
        _drugRepository = drugRepository;
        _mediator = mediator;
    }

    // =====================================================
    // List
    // =====================================================

    [HttpGet]
    [RequirePermission("Drugs.View")]
    public async Task<IActionResult> Index()
    {
        var drugs =
            await _drugRepository.GetAllAsync();

        return View(drugs);
    }


    // =====================================================
    // Create - GET
    // =====================================================

    [HttpGet]
    [RequirePermission("Drugs.Create")]
    public IActionResult Create()
    {
        return View();
    }


    // =====================================================
    // Create - POST
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Drugs.Create")]
    public async Task<IActionResult> Create(
        CreateDrugCommand command)
    {
        if (!ModelState.IsValid)
        {
            return View(command);
        }

        await _mediator.Send(command);

        return RedirectToAction(
            nameof(Index));
    }


    // =====================================================
    // Edit - GET
    // =====================================================

    [HttpGet]
    [RequirePermission("Drugs.Edit")]
    public async Task<IActionResult> Edit(
        int id)
    {
        var drug =
            await _drugRepository.GetByIdAsync(id);

        if (drug == null)
        {
            return NotFound();
        }

        return View(drug);
    }


    // =====================================================
    // Edit - POST
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Drugs.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        DrugEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var drug =
            await _drugRepository.GetByIdAsync(id);

        if (drug == null)
        {
            return NotFound();
        }

        drug.Name = model.Name;
        drug.GenericName = model.GenericName;
        drug.ActiveIngredient = model.ActiveIngredient;
        drug.DosageForm = model.DosageForm;
        drug.TherapeuticCategory = model.TherapeuticCategory;
        drug.UpdatedAt = DateTime.UtcNow;

        await _drugRepository.UpdateAsync(drug);

        return RedirectToAction(
            nameof(Index));
    }


    // =====================================================
    // Delete
    // =====================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Drugs.Delete")]
    public async Task<IActionResult> Delete(
    int id)
    {
        var drug =
            await _drugRepository.GetByIdAsync(id);

        if (drug == null)
        {
            return NotFound();
        }

        await _drugRepository.DeleteAsync(drug);

        return RedirectToAction(
            nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Drugs.View")]
    public async Task<IActionResult> Details(
    int id,
    CancellationToken cancellationToken)
    {
        var drug = await _drugRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (drug == null)
            return NotFound();

        return View(drug);
    }


}

// =========================================================
// ViewModel
// =========================================================

public class DrugEditViewModel
{
    public string Name { get; set; } = string.Empty;


public string? GenericName { get; set; }

    public string? ActiveIngredient { get; set; }

    public string? DosageForm { get; set; }

    public string? TherapeuticCategory { get; set; }


}
