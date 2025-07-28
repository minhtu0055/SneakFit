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
using SneakFit.WebClient.Models;

namespace SneakFit.WebClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHoaDonClientApiClient _hoaDonClientApiClient;
        private readonly IHoaDonChiTietClientApiClient _hoaDonChiTietClientApiClient;
        private readonly IVoucherApiClient _voucherApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IUserApiClient _userApiClient;

        public AccountController(IUserApiClient userApiClient,
            IHoaDonClientApiClient hoaDonClientApiClient,
            IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient,
            IVoucherApiClient voucherApiClient,
            ISpctApiClient spctApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _voucherApiClient = voucherApiClient;
            _spctApiClient = spctApiClient;
            _userApiClient = userApiClient;
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userApiClient.Authenticate(model);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            // Lưu token vào session
            HttpContext.Session.SetString("Token", result.ResultObj);

            // Tách JWT để lấy Id người dùng
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.ResultObj);
            var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Không thể lấy thông tin người dùng từ token.");
                return View(model);
            }

            // Gọi API lấy thông tin chi tiết user
            var userInfo = await _userApiClient.GetById(Guid.Parse(userId));
            if (!userInfo.IsSuccessed)
            {
                ModelState.AddModelError("", "Không thể lấy thông tin người dùng.");
                return View(model);
            }

            var user = userInfo.ResultObj;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.HoVaTen),
                new Claim("AvatarUrl", user.UrlHinhAnh ?? "/assets/img/default-avatar.png"),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("MyProfile");
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

                var request = new PhanTrangHoaDonClient
                {
                    PageIndex = 1,
                    PageSize = 10,
                    Keyword = "",
                    Trangthaihoadon = (Data.Enums.TrangThaiHoaDon?)trangThai,
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    UserId = userId
                };

                var hoaDons = await _hoaDonClientApiClient.GetAllPaging(request);

                if (hoaDons?.Items != null)
                {
                    model.hoaDonClientViewModels = hoaDons.Items
                        .Where(x => x.UserId == userId)
                        .OrderByDescending(x => x.NgayTao)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lấy danh sách hóa đơn: {ex.Message}";
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
                return RedirectToAction("MyProfile", "Account");
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
