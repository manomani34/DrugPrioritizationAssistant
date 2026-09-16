using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrugPrioritizationAssistant.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class AdminBaseController : Controller
{
}