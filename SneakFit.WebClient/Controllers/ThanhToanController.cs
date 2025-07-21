using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Application.Catalog.ThanhToan;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.GHN;
using SneakFit.ViewModels.System.DiaChi;
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
        private readonly IGhnApiClient _ghnApiClient;
        private readonly IThanhToanApiClient _thanhToanApiClient;

        public ThanhToanController(IHoaDonClientApiClient hoaDonClientApiClient,
                                   IGioHangApiClient gioHangApiClient,
                                   ISanPhamApiClient sanPhamApiClient,
                                   ISpctApiClient spctApiClient,
                                   IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient,
                                   IKhuyenMaiApiClient khuyenMaiApiClient,
                                   IVoucherApiClient voucherApiClient,
                                   IDiaChiApiClient diaChiApiClient,
                                   IGhnApiClient ghnApiClient,
                                   IThanhToanApiClient thanhToanApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _gioHangApiClient = gioHangApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _voucherApiClient = voucherApiClient;
            _diaChiApiClient = diaChiApiClient;
            _ghnApiClient = ghnApiClient;
            _thanhToanApiClient = thanhToanApiClient;
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
                if (spct != null) item.SoLuongTon = spct.SoLuong;

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

            var voucherPaging = await _voucherApiClient.GetAllPaging(new GetVoucherPagingRequest
            {
                PageIndex = 1,
                PageSize = 100,
                Status = TrangThaiGiamGia.HoatDong
            });
            var vouchers = voucherPaging.Items?.ToList() ?? new List<VoucherViewModels>();

            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = string.IsNullOrEmpty(userIdStr) ? null : Guid.Parse(userIdStr);
            Guid? defaultAddressId = null;
            string hoTen = string.Empty, soDienThoai = string.Empty, diaChi = string.Empty, email = string.Empty;

            decimal phiVanChuyen = 0m;

            if (userId.HasValue)
            {
                var diaChis = await _diaChiApiClient.GetAllByUser() ?? new List<DiaChiViewModel>();
                var defaultAddress = diaChis.FirstOrDefault(x => x.MacDinh);

                if (defaultAddress != null)
                {
                    hoTen = defaultAddress.TenNguoiNhan ?? string.Empty;
                    soDienThoai = defaultAddress.SoDienThoai ?? string.Empty;
                    diaChi = $"{defaultAddress.TenDiaChi ?? ""}, {defaultAddress.TenXa ?? ""}, {defaultAddress.TenHuyen ?? ""}, {defaultAddress.TenThanhPho ?? ""}";
                    defaultAddressId = defaultAddress.Id;

                    if (!string.IsNullOrEmpty(defaultAddress.MaHuyen) && !string.IsNullOrEmpty(defaultAddress.MaXa))
                    {
                        var request = new ShippingFeeRequest
                        {
                            FromDistrictId = 1452, // Địa chỉ shop
                            ToDistrictId = int.TryParse(defaultAddress.MaHuyen, out int districtId) ? districtId : 0,
                            ToWardCode = defaultAddress.MaXa ?? "",
                            Weight = 700,
                            Length = 33,
                            Width = 20,
                            Height = 12,
                            ServiceId = 53321
                        };

                        try
                        {
                            var responseJson = await _ghnApiClient.CalculateShippingFee(request);
                            if (!string.IsNullOrEmpty(responseJson))
                            {
                                using var jsonDoc = JsonDocument.Parse(responseJson);
                                var root = jsonDoc.RootElement;
                                if (root.TryGetProperty("data", out var data) && data.TryGetProperty("total", out var totalProp))
                                {
                                    phiVanChuyen = totalProp.GetDecimal();
                                }
                                else if (root.TryGetProperty("data", out data) && data.TryGetProperty("service_fee", out totalProp)) // Thử key khác
                                {
                                    phiVanChuyen = totalProp.GetDecimal();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Nếu lỗi thì phiVanChuyen giữ nguyên là 0
                            TempData["WarningMessage"] = "Không thể tính phí vận chuyển, vui lòng thử lại.";
                        }
                    }
                }
                else
                {
                    TempData["WarningMessage"] = "Bạn chưa có địa chỉ mặc định. Vui lòng thêm địa chỉ trước khi thanh toán.";
                }

                email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            }

            var model = new CheckoutViewModel
            {
                HoTen = hoTen,
                SoDienThoai = soDienThoai,
                DiaChiMoi = string.Empty,
                DiaChi = diaChi,
                PhuongThucThanhToan = PhuongThucThanhToan.COD,
                PhiVanChuyen = phiVanChuyen,
                GioHangItems = cartItems,
                TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong),
                DiscountAmount = 0,
                GhiChu = string.Empty,
                DefaultAddressId = defaultAddressId,
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

            // Check tồn kho (giữ nguyên)
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

            decimal tongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
            decimal giamVoucher = 0;
            Guid? voucherId = null;

            // Parse voucher từ model (hidden fields)
            if (model.VoucherId.HasValue)
            {
                var voucher = await _voucherApiClient.GetById(model.VoucherId.Value);
                if (voucher != null && tongTienSanPham >= voucher.DieuKienApDung)
                {
                    voucherId = model.VoucherId;
                    if (voucher.LoaiGiamGia == LoaiGiamGia.PhamTram)
                    {
                        giamVoucher = Math.Round(tongTienSanPham * (voucher.GiaTriGiamGia / 100), 0);
                        if (voucher.GiaTriToiDa > 0 && giamVoucher > voucher.GiaTriToiDa)
                        {
                            giamVoucher = voucher.GiaTriToiDa;
                        }
                    }
                    else
                    {
                        giamVoucher = voucher.GiaTriGiamGia;
                        if (giamVoucher > tongTienSanPham)
                        {
                            giamVoucher = tongTienSanPham;
                        }
                    }
                }
            }

            // Tính phí ship (giữ nguyên, nhưng đảm bảo parse MaHuyen đúng)
            decimal phiVanChuyen = model.PhiVanChuyen;
            if (!string.IsNullOrEmpty(model.DiaChiMoi))
            {
                var diaChis = await _diaChiApiClient.GetAllByUser() ?? new List<DiaChiViewModel>();
                var selectedAddress = diaChis.FirstOrDefault(x => $"{x.TenDiaChi}, {x.TenXa}, {x.TenHuyen}, {x.TenThanhPho}" == model.DiaChiMoi);

                if (selectedAddress != null && int.TryParse(selectedAddress.MaHuyen, out int toDistrictId))
                {
                    var request = new ShippingFeeRequest
                    {
                        FromDistrictId = 1452,
                        ToDistrictId = toDistrictId,
                        ToWardCode = selectedAddress.MaXa,
                        Weight = 700,
                        Length = 33,
                        Width = 20,
                        Height = 12,
                        ServiceId = 53321
                    };

                    var responseJson = await _ghnApiClient.CalculateShippingFee(request);
                    if (!string.IsNullOrEmpty(responseJson))
                    {
                        using var jsonDoc = JsonDocument.Parse(responseJson);
                        var root = jsonDoc.RootElement;
                        if (root.TryGetProperty("data", out var data) && (data.TryGetProperty("total", out var totalProp) || data.TryGetProperty("service_fee", out totalProp)))
                        {
                            phiVanChuyen = totalProp.GetDecimal();
                        }
                    }
                }
            }

            // Tính tổng đúng nghiệp vụ
            var tongTien = tongTienSanPham - giamVoucher + phiVanChuyen;
            if (tongTien < 0) tongTien = 0;

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
            var hoaDonRequest = new ThemHoaDonClient
            {
                TongTien = tongTien,
                TrangThai = TrangThaiHoaDon.ChoXacNhan,
                UserId = GetUserId(),
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                DiaChi = !string.IsNullOrEmpty(model.DiaChiMoi) ? model.DiaChiMoi : model.DiaChi,
                PhiVanChuyen = phiVanChuyen,
                PhuongThucThanhToan = model.PhuongThucThanhToan.Value,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                //LoaiHoaDon = (model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo)
                //    ? LoaiHoaDon.Online : LoaiHoaDon.TaiQuay, // nếu thanh toán tại quầy = hóa đơn tại quầy
                LoaiHoaDon = LoaiHoaDon.Online,
                Email = email,
                GhiChu = model.GhiChu,
                NgayDatHang = DateTime.Now,
                MaHoaDon = string.Empty,
                DonViVanChuyen = string.Empty,
                MaVanDon = string.Empty,
                VoucherId = voucherId
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
                await _gioHangApiClient.XoaSanPhamDaMuaKhoiGioHang(GetUserId(), sanPhamChiTietIds);

                HttpContext.Session.Remove("SelectedCartItems");

                // Xử lý redirect theo phương thức thanh toán
                if (model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay)
                {
                    var vnpRequest = new VNPayPaymentRequest
                    {
                        Amount = tongTien,
                        OrderId = hoaDon.Id.ToString(),
                        OrderInfo = $"Thanh toán đơn hàng {hoaDon.MaHoaDon}",
                        ReturnUrl = "https://localhost:7277/api/thanhtoan/vnpay-callback-client", // <-- Sửa dòng này
                        NotifyUrl = ""
                    };
                    var paymentUrlJson = await _thanhToanApiClient.CreateVnPayPaymentUrlClient(vnpRequest);
                    var paymentUrl = System.Text.Json.JsonDocument.Parse(paymentUrlJson).RootElement.TryGetProperty("paymentUrl", out var urlProp) ? urlProp.GetString() : paymentUrlJson;
                    return Redirect(paymentUrl);
                }
                else if (model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo)
                {
                    var momoRequest = new MomoPaymentRequest
                    {
                        Amount = tongTien,
                        OrderId = hoaDon.Id.ToString(),
                        OrderInfo = $"Thanh toán đơn hàng {hoaDon.MaHoaDon}",
                        ReturnUrl = "https://localhost:7277/api/thanhtoan/momo-callback-client",
                        NotifyUrl = ""
                    };
                    var paymentUrlJson = await _thanhToanApiClient.CreateMomoPaymentUrl(momoRequest);
                    var paymentUrl = System.Text.Json.JsonDocument.Parse(paymentUrlJson).RootElement.TryGetProperty("paymentUrl", out var urlProp) ? urlProp.GetString() : paymentUrlJson;
                    return Redirect(paymentUrl);
                }
                else
                {
                    // COD
                    return RedirectToAction("OrderConfirmation", new { id = hoaDon.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi đặt hàng: {ex.Message}");
                model.GioHangItems = cartItems;
                model.TongTienSanPham = tongTienSanPham;
                model.DiscountAmount = giamVoucher;
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
                TempData["ErrorMessage"] = $"Lỗi khi tải thông tin hóa đơn: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            await _hoaDonClientApiClient.UpdateStatus(id, SneakFit.Data.Enums.TrangThaiHoaDon.DaHuy);
            return RedirectToAction("Details", "HoaDon", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ReturnOrder(Guid id)
        {
            await _hoaDonClientApiClient.UpdateStatus(id, SneakFit.Data.Enums.TrangThaiHoaDon.TraHang);
            return RedirectToAction("Details", "HoaDon", new { id });
        }
    }
}