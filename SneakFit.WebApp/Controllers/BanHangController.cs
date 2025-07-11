using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.User;

namespace SneakFit.Admin.Controllers
{
    public class BanHangController : BaseController
    {
        private readonly IHoaDonApiClient _hoaDonApiClient;
        private readonly IHoaDonChiTietApiClient _hoaDonChiTietApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IDiaChiApiClient _diaChiApiClient;

        public BanHangController(IHoaDonApiClient hoaDonApiClient, IHoaDonChiTietApiClient hoaDonChiTietApiClient, ISanPhamApiClient sanPhamApiClient, IUserApiClient userApiClient, IDiaChiApiClient diaChiApiClient)
        {
            _hoaDonApiClient = hoaDonApiClient;
            _hoaDonChiTietApiClient = hoaDonChiTietApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _userApiClient = userApiClient;
            _diaChiApiClient = diaChiApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TimKiemKhachHang(string tuKhoa, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var request = new GetUserPagingRequest()
                {
                    TuKhoa = tuKhoa,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Role = "KHÁCH HÀNG" // Chỉ lấy danh sách khách hàng
                };
                
                var result = await _userApiClient.GetUsersPaging(request);
                
                if (result.IsSuccessed)
                {
                    return Json(new { success = true, data = result.ResultObj });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tìm kiếm khách hàng: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDiaChiByUser(Guid userId)
        {
            var result = await _diaChiApiClient.GetAllByUserId(userId);
            return Json(result);
        }
    }
}
