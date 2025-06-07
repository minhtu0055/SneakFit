using Microsoft.AspNetCore.Mvc;

namespace SneakFit.WebClient.Controllers
{
    public class DanhMucController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
