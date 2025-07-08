using Microsoft.AspNetCore.Mvc;
using SneakFit.WebClient.Models;
using SneakFit.ApiIntegration.Services;
using System.Threading.Tasks;
using SneakFit.ViewModels.Catalog.HoaDonClient;

namespace SneakFit.WebClient.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonClientApiClient _hoaDonClientApiClient;
        private readonly IHoaDonChiTietClientApiClient _hoaDonChiTietClientApiClient;

        public HoaDonController(IHoaDonClientApiClient hoaDonClientApiClient, IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
        }
        private Guid GetUserId()
        {
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để tiếp tục.");
            }
            return Guid.Parse(userIdStr);
        }

        // Hiển thị danh sách hóa đơn
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                var request = new PhanTrangHoaDonClient
                {
                    PageIndex = 1,
                    PageSize = 10,
                    Keyword = "",
                    Trangthaihoadon = null,
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    UserId = userId // Thêm lọc theo userId
                };
                var hoaDons = await _hoaDonClientApiClient.GetAllPaging(request);
                if (hoaDons == null || hoaDons.Items == null)
                {
                    return View(new List<HoaDonClientViewModel>());
                }
                // Chỉ hiển thị hóa đơn của userId hiện tại (phòng trường hợp API trả về nhiều user)
                var filtered = hoaDons.Items.Where(x => x.UserId == userId).ToList();
                return View(filtered);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lấy danh sách hóa đơn: {ex.Message}";
                return View(new List<HoaDonClientViewModel>());
            }
        }

        // Hiển thị chi tiết hóa đơn
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var hoaDon = await _hoaDonClientApiClient.GetById(id);
                if (hoaDon == null || hoaDon.UserId != userId)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem đơn hàng này.";
                    return RedirectToAction("Index");
                }

                var chiTietHoaDon = await _hoaDonChiTietClientApiClient.GetByHoaDonId(id);
                var model = new OrderConfirmationViewModel
                {
                    HoaDonClient = hoaDon,
                    ChiTietHoaDonClient = chiTietHoaDon
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lấy chi tiết hóa đơn: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}