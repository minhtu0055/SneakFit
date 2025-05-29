using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.HoaDon;

namespace SneakFit.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonApiClient _hoaDonApiClient;

        public HoaDonController(IHoaDonApiClient hoaDonApiClient)
        {
            _hoaDonApiClient = hoaDonApiClient;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string keyword = "")
        {
            var request = new PhanTrangHoaDon { PageIndex = pageIndex, PageSize = pageSize, Keyword = keyword };
            var result = await _hoaDonApiClient.GetAllPaging(request);
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var hoaDon = await _hoaDonApiClient.GetById(id);
            if (hoaDon == null)
                return NotFound();
            return View(hoaDon);
        }
    }
}
