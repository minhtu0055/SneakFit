using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.WebClient.Models;
using System.Text.Json;

namespace SneakFit.WebClient.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly IHoaDonClientApiClient _hoaDonClientApiClient;
        private readonly IGioHangApiClient _gioHangApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IHoaDonChiTietClientApiClient _hoaDonChiTietClientApiClient;
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;
        private readonly IVoucherApiClient _voucherApiClient;
        private readonly IDiaChiApiClient _diaChiApiClient;

        public ThanhToanController(IHoaDonClientApiClient hoaDonClientApiClient, 
                                   IGioHangApiClient gioHangApiClient, 
                                   ISanPhamApiClient sanPhamApiClient, 
                                   ISpctApiClient spctApiClient,
                                   IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient,
                                   IKhuyenMaiApiClient khuyenMaiApiClient,
                                   IVoucherApiClient voucherApiClient,
                                   IDiaChiApiClient diaChiApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _gioHangApiClient = gioHangApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _voucherApiClient = voucherApiClient;
            _diaChiApiClient = diaChiApiClient;
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

        public async Task<IActionResult> Checkout()
        {
            var cartJson = HttpContext.Session.GetString("SelectedCartItems");
            var cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<GioHangItemViewModel>()
                : JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson);

            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán.";
                return RedirectToAction("Index", "GioHang");
            }

            var khuyenMais = _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                Keyword = null,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            }).GetAwaiter().GetResult();

            foreach (var item in cartItems)
            {
                var spct = _spctApiClient.GetById(item.SanPhamChiTietId).GetAwaiter().GetResult();
                if (spct != null)
                {
                    item.SoLuongTon = spct.SoLuong;
                }

                var km = khuyenMais.Items
                    .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == item.SanPhamChiTietId))
                    .OrderByDescending(x => x.ThoiGianBatDau)
                    .FirstOrDefault();

                if (km != null)
                {
                    if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                    {
                        item.PhanTramGiamGia = (int)Math.Round(km.GiaTriGiamGia);
                        item.GiaKhuyenMai = Math.Round(item.GiaGoc * (1 - km.GiaTriGiamGia / 100m), 0);
                    }
                    else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                    {
                        item.GiaKhuyenMai = Math.Max(0, item.GiaGoc - km.GiaTriGiamGia);
                        item.PhanTramGiamGia = item.GiaGoc > 0 ? (int)Math.Round((km.GiaTriGiamGia / item.GiaGoc) * 100, 0) : 0;
                    }
                }
                else
                {
                    item.GiaKhuyenMai = item.GiaGoc;
                    item.PhanTramGiamGia = 0;
                }
            }

            // Lấy danh sách voucher đang hoạt động
            var voucherPaging = await _voucherApiClient.GetAllPaging(new SneakFit.ViewModels.Catalog.Voucher.GetVoucherPagingRequest
            {
                PageIndex = 1,
                PageSize = 100,
                Status = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });
            var vouchers = voucherPaging.Items?.ToList() ?? new List<SneakFit.ViewModels.Catalog.Voucher.VoucherViewModels>();

            // Lấy userId từ claim
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = null;
            Guid? defaultAddressId = null;
            if (!string.IsNullOrEmpty(userIdStr))
                userId = Guid.Parse(userIdStr);

            string hoTen = string.Empty, soDienThoai = string.Empty, diaChi = string.Empty, email = string.Empty;

            // Nếu có userId, lấy địa chỉ mặc định từ API client
            if (userId.HasValue)
            {
                var diaChis = await _diaChiApiClient.GetAllByUser();
                var defaultAddress = diaChis.FirstOrDefault(x => x.MacDinh);
                if (defaultAddress != null)
                {
                    hoTen = defaultAddress.TenNguoiNhan;
                    soDienThoai = defaultAddress.SoDienThoai;
                    diaChi = $"{defaultAddress.TenDiaChi}, {defaultAddress.TenXa}, {defaultAddress.TenHuyen}, {defaultAddress.TenThanhPho}";
                    defaultAddressId = defaultAddress.Id;
                }
                else
                {
                    TempData["WarningMessage"] = "Bạn chưa có địa chỉ mặc định. Vui lòng thêm địa chỉ trước khi thanh toán.";
                }
                // Lấy email từ claim
                email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            }

            var model = new CheckoutViewModel
            {
                HoTen = hoTen,
                SoDienThoai = soDienThoai,
                DiaChiMoi = string.Empty,
                DiaChi = diaChi,
                PhuongThucThanhToan = PhuongThucThanhToan.COD,
                PhiVanChuyen = 35000,
                GioHangItems = cartItems,
                TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong),
                DiscountAmount = 0,
                GhiChu = string.Empty,
                DefaultAddressId = defaultAddressId, // Thêm DefaultAddressId vào model
                Email = email,
                Vouchers = vouchers
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cartJson = HttpContext.Session.GetString("SelectedCartItems");

            if (string.IsNullOrEmpty(cartJson) || !cartJson.Trim().StartsWith("["))
            {
                TempData["ErrorMessage"] = "Giỏ hàng không hợp lệ hoặc trống.";
                return RedirectToAction("Index", "GioHang");
            }

            List<GioHangItemViewModel> cartItems;
            try
            {
                cartItems = JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson) ?? new();
            }
            catch
            {
                TempData["ErrorMessage"] = "Lỗi đọc giỏ hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var invalidProducts = new List<string>();
            foreach (var item in cartItems)
            {
                var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
                if (spct == null)
                {
                    invalidProducts.Add($"{item.TenSanPham} (không tìm thấy sản phẩm)");
                    continue;
                }
                if (spct.SoLuong < item.SoLuong)
                {
                    invalidProducts.Add($"{item.TenSanPham} (còn {spct.SoLuong})");
                }
            }

            if (invalidProducts.Any())
            {
                ModelState.AddModelError("", $"Sản phẩm sau không đủ tồn kho: {string.Join(", ", invalidProducts)}");
                model.GioHangItems = cartItems;
                model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                return View(model);
            }

            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });

            foreach (var item in cartItems)
            {
                var km = khuyenMais.Items
                    .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == item.SanPhamChiTietId))
                    .OrderByDescending(x => x.ThoiGianBatDau)
                    .FirstOrDefault();

                if (km != null)
                {
                    if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                    {
                        item.GiaKhuyenMai = Math.Round(item.GiaGoc * (1 - km.GiaTriGiamGia / 100m), 0);
                    }
                    else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                    {
                        item.GiaKhuyenMai = Math.Max(0, item.GiaGoc - km.GiaTriGiamGia);
                    }
                }
                else
                {
                    item.GiaKhuyenMai = item.GiaGoc;
                }
            }

            if (model.PhuongThucThanhToan == null)
            {
                ModelState.AddModelError("PhuongThucThanhToan", "Vui lòng chọn hình thức thanh toán.");
                model.GioHangItems = cartItems;
                model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                return View(model);
            }

            var userId = GetUserId();
            var diaChi = !string.IsNullOrEmpty(model.DiaChiMoi) ? model.DiaChiMoi : model.DiaChi;
            var tongTien = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong) + model.PhiVanChuyen;

            // Lấy email từ claim thay vì model.Email
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            var hoaDonRequest = new ThemHoaDonClient
            {
                TongTien = tongTien,
                TrangThai = TrangThaiHoaDon.ChoXacNhan,
                UserId = userId,
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                DiaChi = diaChi,
                PhiVanChuyen = model.PhiVanChuyen,
                PhuongThucThanhToan = model.PhuongThucThanhToan.Value,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LoaiHoaDon = (model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo)
                    ? LoaiHoaDon.Online : LoaiHoaDon.TaiQuay,
                Email = email,
                GhiChu = model.GhiChu,
                NgayDatHang = DateTime.Now,
                MaHoaDon = string.Empty,
                DonViVanChuyen = string.Empty,
                MaVanDon = string.Empty
            };

            try
            {
                var hoaDon = await _hoaDonClientApiClient.Create(hoaDonRequest);

                foreach (var item in cartItems)
                {
                    await _hoaDonChiTietClientApiClient.Create(new ThemHoaDonChiTietClient
                    {
                        HoaDonId = hoaDon.Id,
                        SanPhamChiTietId = item.SanPhamChiTietId,
                        SoLuong = item.SoLuong,
                        GiaBan = item.GiaKhuyenMai,

                    });

                    var delta = -item.SoLuong;
                    var success = await _spctApiClient.UpdateSoLuong(item.SanPhamChiTietId, delta);
                    if (!success)
                    {
                        throw new InvalidOperationException($"Không thể cập nhật số lượng cho sản phẩm {item.TenSanPham}.");
                    }
                }

                var sanPhamChiTietIds = cartItems.Select(x => x.SanPhamChiTietId).ToList();
                var xoaGioHangSuccess = await _gioHangApiClient.XoaSanPhamDaMuaKhoiGioHang(userId, sanPhamChiTietIds);
                if (!xoaGioHangSuccess)
                {
                    // Không dùng log, chỉ bỏ qua nếu không xóa được (có thể do không tìm thấy sản phẩm)
                }

                HttpContext.Session.Remove("SelectedCartItems");

                return RedirectToAction("OrderConfirmation", new { id = hoaDon.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi đặt hàng: {ex.Message}");
                model.GioHangItems = cartItems;
                model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                return View(model);
            }
        }

        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            try
            {
                var hoaDon = await _hoaDonClientApiClient.GetById(id);
                if (hoaDon == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hóa đơn.";
                    return RedirectToAction("Index", "Home");
                }

                var chiTietHoaDon = await _hoaDonChiTietClientApiClient.GetByHoaDonId(id);

                if (chiTietHoaDon == null)
                {
                    chiTietHoaDon = new List<HoaDonChiTietClientViewModel>();
                }

                var model = new OrderConfirmationViewModel
                {
                    HoaDonClient = hoaDon,
                    ChiTietHoaDonClient = chiTietHoaDon
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải thông tin hóa đơn: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
