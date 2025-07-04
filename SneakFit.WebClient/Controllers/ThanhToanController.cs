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

        public ThanhToanController(IHoaDonClientApiClient hoaDonClientApiClient, 
                                   IGioHangApiClient gioHangApiClient, 
                                   ISanPhamApiClient sanPhamApiClient, 
                                   ISpctApiClient spctApiClient,
                                   IHoaDonChiTietClientApiClient hoaDonChiTietClientApiClient,
                                   IKhuyenMaiApiClient khuyenMaiApiClient)
        {
            _hoaDonClientApiClient = hoaDonClientApiClient;
            _gioHangApiClient = gioHangApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _hoaDonChiTietClientApiClient = hoaDonChiTietClientApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
        }

        private Guid GetUserId()
        {
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdStr)
                ? Guid.Parse("69BD714F-9576-45BA-B5B7-F00649BE00DE") // hardcode for demo
                : Guid.Parse(userIdStr);
        }

        //public IActionResult Checkout()
        //{
        //    var cartJson = HttpContext.Session.GetString("SelectedCartItems");
        //    var cartItems = string.IsNullOrEmpty(cartJson)
        //        ? new List<GioHangItemViewModel>()
        //        : JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson);

        //    if (cartItems.Count == 0)
        //    {
        //        TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    // Lấy danh sách khuyến mãi đang hoạt động
        //    var khuyenMais = _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
        //    {
        //        PageIndex = 1,
        //        PageSize = 100,
        //        Keyword = null,
        //        TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
        //    }).GetAwaiter().GetResult(); // Sử dụng GetAwaiter để đồng bộ trong phương thức không async

        //    // Áp dụng giá khuyến mãi cho từng sản phẩm
        //    foreach (var item in cartItems)
        //    {
        //        var km = khuyenMais.Items
        //            .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == item.SanPhamChiTietId))
        //            .OrderByDescending(x => x.ThoiGianBatDau)
        //            .FirstOrDefault();

        //        if (km != null)
        //        {
        //            if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
        //            {
        //                item.PhanTramGiamGia = (int)Math.Round(km.GiaTriGiamGia);
        //                item.GiaKhuyenMai = Math.Round(item.GiaGoc * (1 - km.GiaTriGiamGia / 100m), 0);
        //            }
        //            else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
        //            {
        //                item.GiaKhuyenMai = Math.Max(0, item.GiaGoc - km.GiaTriGiamGia);
        //                item.PhanTramGiamGia = item.GiaGoc > 0 ? (int)Math.Round((km.GiaTriGiamGia / item.GiaGoc) * 100, 0) : 0;
        //            }
        //        }
        //        else
        //        {
        //            item.GiaKhuyenMai = item.GiaGoc;
        //            item.PhanTramGiamGia = 0;
        //        }
        //    }

        //    var model = new CheckoutViewModel
        //    {
        //        HoTen = "Lại Gia Kiệt",
        //        SoDienThoai = "+84 383212289",
        //        Email = "laigiakiet@gmail.com",
        //        DiaChiMoi = string.Empty, // Để trống nếu không có địa chỉ mới
        //        DiaChi = "Thôn Thương, Xã Hồng Phong, Huyện Chương Mỹ, Hà Nội",
        //        PhuongThucThanhToan = PhuongThucThanhToan.COD,
        //        PhiVanChuyen = 35000,
        //        GioHangItems = cartItems,
        //        TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong),
        //        DiscountAmount = 0, // Mặc định không có giảm giá
        //        GhiChu = string.Empty // Để trống nếu không có ghi chú
        //    };

        //    return View("Checkout", model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Checkout(CheckoutViewModel model)
        //{
        //    var cartJson = HttpContext.Session.GetString("SelectedCartItems");
        //    if (string.IsNullOrEmpty(cartJson))
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    var cartItems = JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson);
        //    if (cartItems == null || !cartItems.Any())
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    // Lấy danh sách khuyến mãi đang hoạt động
        //    var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
        //    {
        //        PageIndex = 1,
        //        PageSize = 100,
        //        Keyword = null,
        //        TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
        //    });

        //    // Áp dụng giá khuyến mãi cho từng sản phẩm
        //    foreach (var item in cartItems)
        //    {
        //        var km = khuyenMais.Items
        //            .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == item.SanPhamChiTietId))
        //            .OrderByDescending(x => x.ThoiGianBatDau)
        //            .FirstOrDefault();

        //        if (km != null)
        //        {
        //            if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
        //            {
        //                item.PhanTramGiamGia = (int)Math.Round(km.GiaTriGiamGia);
        //                item.GiaKhuyenMai = Math.Round(item.GiaGoc * (1 - km.GiaTriGiamGia / 100m), 0);
        //            }
        //            else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
        //            {
        //                item.GiaKhuyenMai = Math.Max(0, item.GiaGoc - km.GiaTriGiamGia);
        //                item.PhanTramGiamGia = item.GiaGoc > 0 ? (int)Math.Round((km.GiaTriGiamGia / item.GiaGoc) * 100, 0) : 0;
        //            }
        //        }
        //        else
        //        {
        //            item.GiaKhuyenMai = item.GiaGoc;
        //            item.PhanTramGiamGia = 0;
        //        }
        //    }

        //    // Kiểm tra số lượng tồn kho
        //    bool isStockValid = true;
        //    foreach (var item in cartItems)
        //    {
        //        var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
        //        if (spct == null)
        //        {
        //            ModelState.AddModelError("", $"Sản phẩm {item.TenSanPham} không tồn tại.");
        //            isStockValid = false;
        //            break;
        //        }

        //        if (item.SoLuong > spct.SoLuong)
        //        {
        //            ModelState.AddModelError("", $"Sản phẩm {item.TenSanPham} chỉ còn {spct.SoLuong} trong kho.");
        //            isStockValid = false;
        //            break;
        //        }
        //    }

        //    if (!isStockValid || !ModelState.IsValid)
        //    {
        //        model.GioHangItems = cartItems;
        //        model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //        return View(model);
        //    }

        //    // Sử dụng địa chỉ mới nếu có
        //    var diaChiHienTai = !string.IsNullOrEmpty(model.DiaChiMoi) ? model.DiaChiMoi : model.DiaChi;

        //    // Tạo yêu cầu hóa đơn
        //    var userId = GetUserId();
        //    var tongTien = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong) + model.PhiVanChuyen;

        //    var hoaDonRequest = new ThemHoaDonClient
        //    {
        //        TongTien = tongTien,
        //        TrangThai = TrangThaiHoaDon.ChoXacNhan,
        //        UserId = userId,
        //        HoTen = model.HoTen,
        //        SoDienThoai = model.SoDienThoai,
        //        DiaChi = diaChiHienTai,
        //        PhiVanChuyen = model.PhiVanChuyen,
        //        PhuongThucThanhToan = model.PhuongThucThanhToan ?? PhuongThucThanhToan.COD,
        //        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
        //        LoaiHoaDon = model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo ? LoaiHoaDon.Online : LoaiHoaDon.TaiQuay,
        //        GhiChu = model.GhiChu,
        //        Email = model.Email
        //    };

        //    try
        //    {
        //        var hoaDon = await _hoaDonClientApiClient.Create(hoaDonRequest);

        //        foreach (var item in cartItems)
        //        {
        //            var chiTietRequest = new ThemHoaDonChiTietClient
        //            {
        //                HoaDonId = hoaDon.Id,
        //                SanPhamChiTietId = item.SanPhamChiTietId,
        //                SoLuong = item.SoLuong,
        //                GiaBan = item.GiaKhuyenMai // Sử dụng giá khuyến mãi
        //            };
        //            await _hoaDonChiTietClientApiClient.Create(chiTietRequest);

        //            await _spctApiClient.UpdateSoLuong(item.SanPhamChiTietId, -item.SoLuong);
        //        }

        //        HttpContext.Session.Remove("SelectedCartItems");
        //        await _gioHangApiClient.XoaGioHang(userId);

        //        return RedirectToAction("OrderConfirmation", new { id = hoaDon.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Lỗi khi tạo hóa đơn: {ex.Message}");
        //        model.GioHangItems = cartItems;
        //        model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //        return View(model);
        //    }
        //}

        //public async Task<IActionResult> OrderConfirmation(Guid id)
        //{
        //    try
        //    {
        //        var hoaDon = await _hoaDonClientApiClient.GetById(id);
        //        if (hoaDon == null)
        //        {
        //            TempData["ErrorMessage"] = "Không tìm thấy hóa đơn.";
        //            return RedirectToAction("Index", "Home");
        //        }

        //        var chiTietHoaDon = await _hoaDonChiTietClientApiClient.GetByHoaDonId(id);
        //        if (chiTietHoaDon == null)
        //        {
        //            chiTietHoaDon = new List<HoaDonChiTietClientViewModel>(); // Mặc định rỗng nếu không có chi tiết
        //        }

        //        var model = new OrderConfirmationViewModel
        //        {
        //            HoaDonClient = hoaDon,
        //            ChiTietHoaDonClient = chiTietHoaDon
        //        };

        //        return View("OrderConfirmation", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Lỗi khi tải thông tin hóa đơn: {ex.Message}";
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        public IActionResult Checkout()
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

            // Lấy danh sách khuyến mãi đang hoạt động
            var khuyenMais = _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                Keyword = null,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            }).GetAwaiter().GetResult();

            // Cập nhật số lượng tồn kho từ DB
            foreach (var item in cartItems)
            {
                var spct = _spctApiClient.GetById(item.SanPhamChiTietId).GetAwaiter().GetResult();
                if (spct != null)
                {
                    item.SoLuongTon = spct.SoLuong; // Cập nhật số lượng tồn từ DB
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

            var model = new CheckoutViewModel
            {
                HoTen = "Lại Gia Kiệt",
                SoDienThoai = "+84 383212289",
                Email = "laigiakiet@gmail.com",
                DiaChiMoi = string.Empty,
                DiaChi = "Thôn Thương, Xã Hồng Phong, Huyện Chương Mỹ, Hà Nội",
                PhuongThucThanhToan = PhuongThucThanhToan.COD,
                PhiVanChuyen = 35000,
                GioHangItems = cartItems,
                TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong),
                DiscountAmount = 0,
                GhiChu = string.Empty
            };

            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Checkout(CheckoutViewModel model)
        //{
        //    var cartJson = HttpContext.Session.GetString("SelectedCartItems");
        //    if (string.IsNullOrEmpty(cartJson))
        //    {
        //        TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    List<GioHangItemViewModel> cartItems;
        //    try
        //    {
        //        cartItems = JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson) ?? new List<GioHangItemViewModel>();
        //    }
        //    catch (JsonException ex)
        //    {
        //        TempData["ErrorMessage"] = "Dữ liệu giỏ hàng không hợp lệ. Vui lòng thử lại.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    if (cartItems == null || !cartItems.Any() || cartItems.Any(x => x.SoLuong <= 0))
        //    {
        //        TempData["ErrorMessage"] = "Số lượng sản phẩm phải lớn hơn 0. Vui lòng kiểm tra lại giỏ hàng.";
        //        return RedirectToAction("Index", "GioHang");
        //    }

        //    // Cập nhật số lượng tồn kho từ DB trước khi xử lý
        //    foreach (var item in cartItems)
        //    {
        //        var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
        //        if (spct == null)
        //        {
        //            ModelState.AddModelError("", $"Sản phẩm với ID {item.SanPhamChiTietId} không tồn tại.");
        //            model.GioHangItems = cartItems;
        //            model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //            return View(model);
        //        }
        //        item.SoLuongTon = spct.SoLuong; // Gán giá trị mặc định 0 nếu null
        //    }

        //    var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
        //    {
        //        PageIndex = 1,
        //        PageSize = 100,
        //        Keyword = null,
        //        TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
        //    });

        //    foreach (var item in cartItems)
        //    {
        //        var km = khuyenMais.Items
        //            .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == item.SanPhamChiTietId))
        //            .OrderByDescending(x => x.ThoiGianBatDau)
        //            .FirstOrDefault();

        //        if (km != null)
        //        {
        //            if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
        //            {
        //                item.PhanTramGiamGia = (int)Math.Round(km.GiaTriGiamGia);
        //                item.GiaKhuyenMai = Math.Round(item.GiaGoc * (1 - km.GiaTriGiamGia / 100m), 0);
        //            }
        //            else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
        //            {
        //                item.GiaKhuyenMai = Math.Max(0, item.GiaGoc - km.GiaTriGiamGia);
        //                item.PhanTramGiamGia = item.GiaGoc > 0 ? (int)Math.Round((km.GiaTriGiamGia / item.GiaGoc) * 100, 0) : 0;
        //            }
        //        }
        //        else
        //        {
        //            item.GiaKhuyenMai = item.GiaGoc;
        //            item.PhanTramGiamGia = 0;
        //        }
        //    }

        //    bool isStockValid = true;
        //    foreach (var item in cartItems)
        //    {
        //        if (item.SoLuong > item.SoLuongTon)
        //        {
        //            ModelState.AddModelError("", $"Sản phẩm {item.TenSanPham} chỉ còn {item.SoLuongTon} trong kho.");
        //            isStockValid = false;
        //            break;
        //        }
        //    }

        //    if (!isStockValid || !ModelState.IsValid)
        //    {
        //        model.GioHangItems = cartItems;
        //        model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //        return View(model);
        //    }

        //    var diaChiHienTai = !string.IsNullOrEmpty(model.DiaChiMoi) ? model.DiaChiMoi : model.DiaChi;

        //    if (model.PhuongThucThanhToan == null)
        //    {
        //        ModelState.AddModelError("PhuongThucThanhToan", "Vui lòng chọn một hình thức thanh toán.");
        //        model.GioHangItems = cartItems;
        //        model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //        return View(model);
        //    }

        //    var userId = GetUserId();
        //    var tongTien = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong) + model.PhiVanChuyen;

        //    var hoaDonRequest = new ThemHoaDonClient
        //    {
        //        TongTien = tongTien,
        //        TrangThai = TrangThaiHoaDon.ChoXacNhan,
        //        UserId = userId,
        //        HoTen = model.HoTen,
        //        SoDienThoai = model.SoDienThoai,
        //        DiaChi = diaChiHienTai,
        //        PhiVanChuyen = model.PhiVanChuyen,
        //        PhuongThucThanhToan = model.PhuongThucThanhToan.Value,
        //        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
        //        LoaiHoaDon = model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo ? LoaiHoaDon.Online : LoaiHoaDon.TaiQuay,
        //        GhiChu = model.GhiChu,
        //        Email = model.Email,
        //        NgayThanhToan = null,
        //        MaGiaoDich = string.Empty,
        //        DonViVanChuyen = string.Empty,
        //        MaVanDon = string.Empty,
        //        VoucherId = null,
        //        NgayDatHang = DateTime.Now
        //    };

        //    try
        //    {
        //        var hoaDon = await _hoaDonClientApiClient.Create(hoaDonRequest);

        //        foreach (var item in cartItems)
        //        {
        //            var chiTietRequest = new ThemHoaDonChiTietClient
        //            {
        //                HoaDonId = hoaDon.Id,
        //                SanPhamChiTietId = item.SanPhamChiTietId,
        //                SoLuong = item.SoLuong,
        //                GiaBan = item.GiaKhuyenMai
        //            };
        //            await _hoaDonChiTietClientApiClient.Create(chiTietRequest);

        //            // Kiểm tra trước khi cập nhật
        //            if (item.SoLuongTon < item.SoLuong)
        //            {
        //                throw new InvalidOperationException($"Số lượng tồn không đủ cho sản phẩm {item.TenSanPham}.");
        //            }
        //            await _spctApiClient.UpdateSoLuong(item.SanPhamChiTietId, -item.SoLuong);
        //        }

        //        HttpContext.Session.Remove("SelectedCartItems");
        //        await _gioHangApiClient.XoaGioHang(userId);

        //        return RedirectToAction("OrderConfirmation", new { id = hoaDon.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Lỗi khi tạo hóa đơn: {ex.Message}");
        //        model.GioHangItems = cartItems;
        //        model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
        //        return View(model);
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cartJson = HttpContext.Session.GetString("SelectedCartItems");
            if (string.IsNullOrEmpty(cartJson))
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm.";
                return RedirectToAction("Index", "GioHang");
            }

            List<GioHangItemViewModel> cartItems;
            try
            {
                cartItems = JsonSerializer.Deserialize<List<GioHangItemViewModel>>(cartJson) ?? new List<GioHangItemViewModel>();
            }
            catch (JsonException ex)
            {
                TempData["ErrorMessage"] = "Dữ liệu giỏ hàng không hợp lệ. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }

            if (cartItems == null || !cartItems.Any() || cartItems.Any(x => x.SoLuong <= 0))
            {
                TempData["ErrorMessage"] = "Số lượng sản phẩm phải lớn hơn 0. Vui lòng kiểm tra lại giỏ hàng.";
                return RedirectToAction("Index", "GioHang");
            }

            // Cập nhật số lượng tồn kho từ DB
            foreach (var item in cartItems)
            {
                var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
                if (spct == null)
                {
                    ModelState.AddModelError("", $"Sản phẩm với ID {item.SanPhamChiTietId} không tồn tại.");
                    model.GioHangItems = cartItems;
                    model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                    return View(model);
                }
                item.SoLuongTon = spct.SoLuongTon ?? 0; // Sử dụng SoLuongTon thay vì SoLuong
            }

            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                Keyword = null,
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

            bool isStockValid = true;
            foreach (var item in cartItems)
            {
                if (item.SoLuong > item.SoLuongTon)
                {
                    ModelState.AddModelError("", $"Sản phẩm {item.TenSanPham} chỉ còn {item.SoLuongTon} trong kho.");
                    isStockValid = false;
                    break;
                }
            }

            if (!isStockValid || !ModelState.IsValid)
            {
                model.GioHangItems = cartItems;
                model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                return View(model);
            }

            var diaChiHienTai = !string.IsNullOrEmpty(model.DiaChiMoi) ? model.DiaChiMoi : model.DiaChi;

            if (model.PhuongThucThanhToan == null)
            {
                ModelState.AddModelError("PhuongThucThanhToan", "Vui lòng chọn một hình thức thanh toán.");
                model.GioHangItems = cartItems;
                model.TongTienSanPham = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong);
                return View(model);
            }

            var userId = GetUserId();
            var tongTien = cartItems.Sum(x => x.GiaKhuyenMai * x.SoLuong) + model.PhiVanChuyen;

            var hoaDonRequest = new ThemHoaDonClient
            {
                TongTien = tongTien,
                TrangThai = TrangThaiHoaDon.ChoXacNhan,
                UserId = userId,
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                DiaChi = diaChiHienTai,
                PhiVanChuyen = model.PhiVanChuyen,
                PhuongThucThanhToan = model.PhuongThucThanhToan.Value,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                LoaiHoaDon = model.PhuongThucThanhToan == PhuongThucThanhToan.VnPay || model.PhuongThucThanhToan == PhuongThucThanhToan.MoMo ? LoaiHoaDon.Online : LoaiHoaDon.TaiQuay,
                GhiChu = model.GhiChu,
                Email = model.Email,
                NgayThanhToan = null,
                MaGiaoDich = string.Empty,
                DonViVanChuyen = string.Empty,
                MaVanDon = string.Empty,
                VoucherId = null,
                NgayDatHang = DateTime.Now
            };

            try
            {
                var hoaDon = await _hoaDonClientApiClient.Create(hoaDonRequest);

                foreach (var item in cartItems)
                {
                    var chiTietRequest = new ThemHoaDonChiTietClient
                    {
                        HoaDonId = hoaDon.Id,
                        SanPhamChiTietId = item.SanPhamChiTietId,
                        SoLuong = item.SoLuong,
                        GiaBan = item.GiaKhuyenMai
                    };
                    await _hoaDonChiTietClientApiClient.Create(chiTietRequest);

                    // Kiểm tra trước khi cập nhật
                    if (item.SoLuongTon < item.SoLuong)
                    {
                        throw new InvalidOperationException($"Số lượng tồn không đủ cho sản phẩm {item.TenSanPham}.");
                    }
                    await _spctApiClient.UpdateSoLuong(item.SanPhamChiTietId, -item.SoLuong);
                }

                HttpContext.Session.Remove("SelectedCartItems");
                await _gioHangApiClient.XoaGioHang(userId);

                return RedirectToAction("OrderConfirmation", new { id = hoaDon.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo hóa đơn: {ex.Message}");
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
