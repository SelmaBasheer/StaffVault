using Microsoft.AspNetCore.Mvc;

namespace StaffVault.Web.Controllers
{
    public sealed class ErrorController : Controller
    {
        [Route("error")]
        public IActionResult Index()
        {
            return View("Error");
        }
    }
}
