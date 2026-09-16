using DrugPrioritizationAssistant.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }


    // =========================================================
    // Login - GET
    // =========================================================

    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }


    // =========================================================
    // Login - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName,
                                           string password,
                                           bool rememberMe,
                                           string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                string.Empty,
                "نام کاربری و رمز عبور را وارد کنید.");

            return View();
        }


        var result =
            await _signInManager.PasswordSignInAsync(userName,
                                                     password,
                                                     rememberMe,
                                                     lockoutOnFailure: true);


        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                "Index",
                "Home",
                new
                {
                    area = "Admin"
                });
        }


        if (result.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "حساب کاربری موقتاً قفل شده است.");

            return View();
        }


        ModelState.AddModelError(
            string.Empty,
            "نام کاربری یا رمز عبور اشتباه است.");

        return View();
    }


    // =========================================================
    // Logout
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            nameof(Login));
    }


    // =========================================================
    // Access Denied
    // =========================================================

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}