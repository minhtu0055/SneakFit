using Microsoft.AspNetCore.Mvc;

namespace SneakFit.WebClient.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
