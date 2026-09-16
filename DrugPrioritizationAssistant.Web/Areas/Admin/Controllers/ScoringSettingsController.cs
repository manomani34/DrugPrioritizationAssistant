using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Application.Services;
using Microsoft.AspNetCore.Mvc;
using DrugPrioritizationAssistant.Web.Authorization;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

public class ScoringSettingsController : AdminBaseController
{
    private readonly IScoringSettingsService _settingsService;

    public ScoringSettingsController(
        IScoringSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    [RequirePermission("ScoringSettings.View")]
    public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
    {
        var settings =
            await _settingsService.GetOrCreateAsync(
                cancellationToken);

        var model = new ScoringSettingsDto
        {
            NeedWeight = settings.NeedWeight,
            FeasibilityWeight =
                settings.FeasibilityWeight
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ScoringSettings.Edit")]
    public async Task<IActionResult> Index(
    ScoringSettingsDto model,
    CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _settingsService.UpdateAsync(
                model.NeedWeight,
                model.FeasibilityWeight,
                cancellationToken);

            TempData["SuccessMessage"] =
                "تنظیمات امتیازدهی با موفقیت ذخیره شد.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
}