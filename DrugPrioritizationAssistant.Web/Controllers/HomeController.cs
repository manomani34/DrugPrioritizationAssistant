using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Home",
                new
                {
                    area = "Admin"
                });
        }

        return RedirectToAction(
            "Login",
            "Account",
            new
            {
                area = "Identity"
            });
    }
}