using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;

namespace SneakFit.Admin.Controllers
{
    public class BanHangController : BaseController
    {
        private readonly IHoaDonApiClient _hoaDonApiClient;
        private readonly IHoaDonChiTietApiClient _hoaDonChiTietApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;

        public BanHangController(IHoaDonApiClient hoaDonApiClient, IHoaDonChiTietApiClient hoaDonChiTietApiClient, ISanPhamApiClient sanPhamApiClient)
        {
            _hoaDonApiClient = hoaDonApiClient;
            _hoaDonChiTietApiClient = hoaDonChiTietApiClient;
            _sanPhamApiClient = sanPhamApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
