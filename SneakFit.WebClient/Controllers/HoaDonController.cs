using Microsoft.AspNetCore.Mvc;
using SneakFit.WebClient.Models;
using SneakFit.ApiIntegration.Services;
using System.Threading.Tasks;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.Voucher;

namespace SneakFit.WebClient.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonClientApiClient _hoaDonClientApiClient;
        private readonly IHoaDonChiTietClientApiClient _hoaDonChiTietClientApiClient;
        private readonly IVoucherApiClient _voucherApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly ITraHangApiClient _traHangApiClient;

        public HoaDonController(IHoaDonClientApiClient hoaDonClientApiClient, 
                                IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient, 
                                IVoucherApiClient voucherApiClient, 
                                ISpctApiClient spctApiClient, 
                                ITraHangApiClient traHangApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _voucherApiClient = voucherApiClient;
            _spctApiClient = spctApiClient;
            _traHangApiClient = traHangApiClient;
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
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            try
            {
                var userId = GetUserId();
                var request = new PhanTrangHoaDonClient
                {
                    PageIndex = pageIndex,
                    PageSize = 10,
                    Keyword = "",
                    Trangthaihoadon = null,
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    UserId = userId // Thêm lọc theo userId
                };
                var hoaDons = await _hoaDonClientApiClient.GetAllPaging(request);
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = request.PageSize;
                ViewBag.TotalRecords = hoaDons?.TotalRecords ?? 0;
                if (hoaDons == null || hoaDons.Items == null)
                {
                    return View(new List<HoaDonClientViewModel>());
                }
                var filtered = hoaDons.Items
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.NgayTao)
                    .ToList();
                return View(filtered);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Index", "Login");
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

                // Kiểm tra xem hóa đơn có yêu cầu trả hàng chưa
                hoaDon.HasReturnRequest = await _traHangApiClient.HasAsync(id);

                var chiTietHoaDon = await _hoaDonChiTietClientApiClient.GetByHoaDonId(id);
                // Lấy thông tin khuyến mãi cho từng sản phẩm chi tiết
                foreach (var item in chiTietHoaDon)
                {
                    try
                    {
                        var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
                        if (spct != null)
                        {
                            item.GiaGoc = spct.Gia; // Đảm bảo luôn truyền giá gốc
                            if (spct.KhuyenMaiId.HasValue && spct.GiaKhuyenMai > 0 && spct.GiaKhuyenMai < spct.Gia)
                            {
                                item.GiaKhuyenMai = spct.GiaKhuyenMai;
                                item.KhuyenMaiPhanTram = spct.KhuyenMaiPhanTram;
                                item.KhuyenMaiId = spct.KhuyenMaiId;
                                item.TenKhuyenMai = $"Giảm {spct.KhuyenMaiPhanTram}%";
                            }
                            else
                            {
                                item.GiaKhuyenMai = null;
                                item.KhuyenMaiPhanTram = null;
                                item.KhuyenMaiId = null;
                                item.TenKhuyenMai = null;
                            }
                        }
                    }
                    catch { }
                }
                VoucherViewModels usedVoucher = null;
                if (hoaDon.VoucherId.HasValue)
                {
                    usedVoucher = await _voucherApiClient.GetById(hoaDon.VoucherId.Value);
                }
                var model = new OrderConfirmationViewModel
                {
                    HoaDonClient = hoaDon,
                    ChiTietHoaDonClient = chiTietHoaDon,
                    UsedVoucher = usedVoucher
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