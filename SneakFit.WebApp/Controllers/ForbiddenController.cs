using Microsoft.AspNetCore.Mvc;

namespace SneakFit.Admin.Controllers
{
    public class ForbiddenController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Hãy liên hệ Minh Tú để giải quyết.";
            return View();
        }
    }
}
