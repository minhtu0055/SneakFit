using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ViewModels.System.User;
using SneakFit.ApiIntegration.Services;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.WebClient.Models;
using SneakFit.Data.Entities;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.DiaChi;

namespace SneakFit.WebClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHoaDonClientApiClient _hoaDonClientApiClient;
        private readonly IHoaDonChiTietClientApiClient _hoaDonChiTietClientApiClient;
        private readonly IVoucherApiClient _voucherApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IDiaChiApiClient _diaChiApiClient;

        public AccountController(IUserApiClient userApiClient,
            IHoaDonClientApiClient hoaDonClientApiClient,
            IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient,
            IVoucherApiClient voucherApiClient,
            ISpctApiClient spctApiClient,
            IDiaChiApiClient diaChiApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _voucherApiClient = voucherApiClient;
            _spctApiClient = spctApiClient;
            _userApiClient = userApiClient;
            _diaChiApiClient = diaChiApiClient;
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        private Guid GetUserId()
        {
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để tiếp tục.");
            }
            return Guid.Parse(userIdStr);
        }
        // AJAX action để lấy danh sách đơn hàng theo trạng thái
        public async Task<IActionResult> GetOrdersByStatus(string trangThai = null, int pageIndex = 1)
        {
            try
            {
                var userId = GetUserId();

                // Xử lý tham số trangThai
                SneakFit.Data.Enums.TrangThaiHoaDon? trangThaiEnum = null;
                string selectedTrangThaiString = trangThai;

                if (!string.IsNullOrEmpty(trangThai) && trangThai != "all")
                {
                    // Thử parse như string trước
                    if (Enum.TryParse<SneakFit.Data.Enums.TrangThaiHoaDon>(trangThai, out var parsedTrangThai))
                    {
                        trangThaiEnum = parsedTrangThai;
                        selectedTrangThaiString = trangThai; // Giữ nguyên string gốc
                    }
                    else
                    {
                        // Nếu không parse được string, thử parse như int
                        if (int.TryParse(trangThai, out var trangThaiInt))
                        {
                            if (Enum.IsDefined(typeof(SneakFit.Data.Enums.TrangThaiHoaDon), trangThaiInt))
                            {
                                trangThaiEnum = (SneakFit.Data.Enums.TrangThaiHoaDon)trangThaiInt;
                                // Chuyển đổi int thành string tương ứng
                                selectedTrangThaiString = trangThaiEnum.ToString();
                            }
                        }
                    }
                }
                else if (trangThai == "all")
                {
                    selectedTrangThaiString = "all";
                }

                var request = new PhanTrangHoaDonClient
                {
                    PageIndex = pageIndex,
                    PageSize = 10,
                    Keyword = "",
                    Trangthaihoadon = trangThaiEnum,
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    UserId = userId
                };
                var hoaDons = await _hoaDonClientApiClient.GetAllPaging(request);
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = request.PageSize;
                ViewBag.TotalRecords = hoaDons?.TotalRecords ?? 0;
                if (hoaDons == null || hoaDons.Items == null)
                {
                    return PartialView("_OrdersList", new List<HoaDonClientViewModel>());
                }
                var filtered = hoaDons.Items
                .Where(x => x.UserId == userId &&
                            (trangThaiEnum == null || x.TrangThai == trangThaiEnum))
                .OrderByDescending(x => x.NgayTao)
                .ToList();

                ViewData["selectedTrangThai"] = selectedTrangThaiString;
                return PartialView("_OrdersList", filtered);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lấy danh sách hóa đơn";
                return PartialView("_OrdersList", new List<HoaDonClientViewModel>());
            }
        }


        // Trang chủ Account
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> MyProfile(int? trangThai = null)
        {
            ViewBag.HoVaTen = User.FindFirstValue(ClaimTypes.Name) ?? "KHÁCH HÀNG";
            ViewBag.AnhDaiDien = User.FindFirstValue("AvatarUrl") ?? "/assets/img/default-avatar.png";

            var model = new AccountViewModel();

            try
            {
                var userId = GetUserId();

                // ✅ LẤY THÔNG TIN USER
                var userResult = await _userApiClient.GetById(userId);
                if (userResult.IsSuccessed)
                {
                    model.User = userResult.ResultObj;
                }

                // ✅ LẤY TẤT CẢ ĐỊA CHỈ CỦA USER
                try
                {
                    var diaChiList = await _diaChiApiClient.GetAllByUser();
                    // Sắp xếp để địa chỉ mặc định (MacDinh = true) hiển thị trước
                    model.DiaChiList = (diaChiList ?? new List<DiaChiViewModel>())
                        .OrderByDescending(x => x.MacDinh)
                        .ToList();
                }
                catch (Exception ex)
                {
                    // Nếu không lấy được danh sách địa chỉ, tạo list rỗng
                    model.DiaChiList = new List<DiaChiViewModel>();
                }

                // Lấy tất cả hóa đơn để tính số lượng theo trạng thái
                var requestAll = new PhanTrangHoaDonClient
                {
                    PageIndex = 1,
                    PageSize = 1000, // Lấy nhiều để đảm bảo có tất cả dữ liệu
                    Keyword = "",
                    Trangthaihoadon = null, // Không filter theo trạng thái
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    UserId = userId
                };

                var allHoaDons = await _hoaDonClientApiClient.GetAllPaging(requestAll);

                if (allHoaDons?.Items != null)
                {
                    var allDanhSachDon = allHoaDons.Items
                        .Where(x => x.UserId == userId)
                        .ToList();

                    // Tính số lượng theo trạng thái từ tất cả dữ liệu
                    model.SoLuongTheoTrangThai = allDanhSachDon
                        .GroupBy(x => x.TrangThai)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Lấy dữ liệu để hiển thị (có thể filter theo trạng thái)
                    if (trangThai.HasValue)
                    {
                        // Nếu có filter trạng thái, chỉ lấy dữ liệu cho trạng thái đó
                        model.hoaDonClientViewModels = allDanhSachDon
                            .Where(x => x.TrangThai == (Data.Enums.TrangThaiHoaDon)trangThai.Value)
                            .OrderByDescending(x => x.NgayTao)
                            .ToList();
                    }
                    else
                    {
                        // Nếu không có filter, lấy tất cả
                        model.hoaDonClientViewModels = allDanhSachDon
                            .OrderByDescending(x => x.NgayTao)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lấy danh sách hóa đơn";
                model.hoaDonClientViewModels = new(); // fallback an toàn
            }

            return View(model);
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

        // AJAX action để lấy chi tiết hóa đơn
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var hoaDon = await _hoaDonClientApiClient.GetById(id);
                if (hoaDon == null || hoaDon.UserId != userId)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem đơn hàng này." });
                }

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
                return PartialView("_OrderDetails", model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi lấy chi tiết hóa đơn: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(AccountViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Không cần load lại thông tin user nữa
            //    TempData["error"] = "Vui lòng kiểm tra lại thông tin nhập.";
            //    return RedirectToAction("MyProfile");
            //}

            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uid))
            {
                TempData["error"] = "Không thể xác định người dùng.";
                return RedirectToAction("MyProfile", "Account");
            }

            var result = await _userApiClient.DoiMatKhau(Guid.Parse(uid), model.DoiMatKhauRequest);

            if (result.IsSuccessed)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Remove("Token");
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "Login");
            }

            // Nếu đổi mật khẩu thất bại
            TempData["error"] = result.Message ?? "Đổi mật khẩu thất bại.";
            return RedirectToAction("MyProfile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
    }
}