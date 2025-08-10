using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.GHN;
using SneakFit.ViewModels.System.DiaChi;
using SneakFit.WebClient.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SneakFit.WebClient.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;
        private readonly IGioHangApiClient _gioHangApiClient;
        private readonly IVoucherApiClient _voucherApiClient;
        private readonly IDiaChiApiClient _diaChiApiClient;
        private readonly IGhnApiClient _ghnApiClient;

        public GioHangController(
            ISanPhamApiClient sanPhamApiClient,
            ISpctApiClient spctApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IKhuyenMaiApiClient khuyenMaiApiClient,
            IGioHangApiClient gioHangApiClient,
            IVoucherApiClient voucherApiClient,
            IDiaChiApiClient diaChiApiClient,
            IGhnApiClient ghnApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _gioHangApiClient = gioHangApiClient;
            _voucherApiClient = voucherApiClient;
            _diaChiApiClient = diaChiApiClient;
            _ghnApiClient = ghnApiClient;
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    await _gioHangApiClient.TaoGioHangMoi(userId);
                    gioHang = await _gioHangApiClient.GetByUserId(userId);
                }
                var list = gioHang?.GioHangChiTiets?.Select(x => new GioHangItemViewModel
                {
                    SanPhamChiTietId = x.SanPhamChiTietId,
                    TenSanPham = x.TenSanPham,
                    AnhSanPham = x.HinhAnh ?? "/images/Default_Logo.png",
                    MauSac = x.MauSac,
                    KichThuoc = x.KichThuoc.ToString(),
                    GiaGoc = x.DonGia,
                    GiaKhuyenMai = x.DonGia,
                    SoLuong = x.SoLuong,
                    TrangThai = true, // Mặc định là true, sẽ được cập nhật sau
                }).ToList() ?? new List<GioHangItemViewModel>();

                // Kiểm tra trạng thái sản phẩm và cập nhật thông tin
                foreach (var item in list)
                {
                    var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
                    if (spct != null)
                    {
                        item.TrangThai = spct.TrangThai;
                        item.SoLuongTon = spct.SoLuong;
                    }
                    else
                    {
                        item.TrangThai = false; // Sản phẩm không tồn tại
                        item.SoLuongTon = 0;
                    }
                }
                // Giữ lại tất cả sản phẩm trong giỏ hàng, không xóa

                var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
                {
                    PageIndex = 1,
                    PageSize = 100,
                    Keyword = null,
                    TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
                });

                foreach (var item in list)
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

                decimal shippingFee = 0m;
                var diaChis = await _diaChiApiClient.GetAllByUser() ?? new List<DiaChiViewModel>();
                var defaultAddress = diaChis.FirstOrDefault(x => x.MacDinh);
                if (defaultAddress != null && !string.IsNullOrEmpty(defaultAddress.MaHuyen) && !string.IsNullOrEmpty(defaultAddress.MaXa))
                {
                    var request = new ShippingFeeRequest
                    {
                        FromDistrictId = 1452,
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
                                shippingFee = totalProp.GetDecimal();
                            }
                            else if (root.TryGetProperty("data", out data) && data.TryGetProperty("service_fee", out totalProp))
                            {
                                shippingFee = totalProp.GetDecimal();
                            }
                        }
                    }
                    catch
                    {
                        // Nếu lỗi thì shippingFee giữ nguyên là 0
                    }
                }

                ViewBag.ShippingFee = shippingFee;
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(new List<GioHangItemViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(Guid sanPhamChiTietId, int soLuong)
        {
            try
            {
                // Validate input
                if (soLuong < 1)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });
                }
                if (soLuong > 99)
                {
                    return Json(new { success = false, message = "Số lượng không được vượt quá 99" });
                }

                var userId = GetUserId();

                // Kiểm tra tồn kho trước khi cập nhật
                var sanPhamChiTiet = await _spctApiClient.GetById(sanPhamChiTietId);
                if (sanPhamChiTiet == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Lấy giỏ hàng hiện tại để kiểm tra số lượng
                var gioHangHienTai = await _gioHangApiClient.GetByUserId(userId);
                var currentItem = gioHangHienTai?.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == sanPhamChiTietId);

                // Kiểm tra tồn kho với số lượng mới
                if (soLuong > sanPhamChiTiet.SoLuong)
                {
                    return Json(new { success = false, message = $"Số lượng vượt quá tồn kho (còn: {sanPhamChiTiet.SoLuong} sản phẩm)" });
                }

                // Update số lượng qua API
                var request = new CapNhatGioHang
                {
                    UserId = userId,
                    SanPhamChiTietId = sanPhamChiTietId,
                    SoLuong = soLuong
                };
                var result = await _gioHangApiClient.CapNhatSoLuong(request);
                if (!result.IsSuccessed)
                {
                    return Json(new { success = false, message = result.Message });
                }

                // Lấy lại cart mới nhất từ DB để đảm bảo data chính xác
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                var item = gioHang?.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == sanPhamChiTietId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                // Lấy danh sách khuyến mãi để tính giá chính xác
                var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
                {
                    PageIndex = 1,
                    PageSize = 100,
                    Keyword = null,
                    TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
                });

                // Helper function để tính giá khuyến mãi
                decimal GetGiaKhuyenMai(GioHangChiTietViewModel sp)
                {
                    var km = khuyenMais.Items
                        .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == sp.SanPhamChiTietId))
                        .OrderByDescending(x => x.ThoiGianBatDau)
                        .FirstOrDefault();
                    if (km != null)
                    {
                        if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                            return Math.Round(sp.DonGia * (1 - km.GiaTriGiamGia / 100m), 0);
                        else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                            return Math.Max(0, sp.DonGia - km.GiaTriGiamGia);
                    }
                    return sp.DonGia;
                }

                // Tính giá và tổng tiền chính xác
                decimal giaKhuyenMai = GetGiaKhuyenMai(item);
                decimal thanhTien = giaKhuyenMai * item.SoLuong;

                // Tổng tiền toàn bộ giỏ hàng
                var tongTien = gioHang.GioHangChiTiets.Sum(sp => GetGiaKhuyenMai(sp) * sp.SoLuong);

                var maxQuantity = sanPhamChiTiet?.SoLuong ?? 99;

                return Json(new
                {
                    success = true,
                    soLuong = item.SoLuong, // Trả về số lượng thực tế từ DB
                    thanhTien = thanhTien,
                    tongTien = tongTien,
                    maxQuantity = maxQuantity,
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaSanPham(Guid sanPhamChiTietId)
        {
            try
            {
                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng" });
                }

                var item = gioHang.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == sanPhamChiTietId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                var result = await _gioHangApiClient.XoaSanPhamKhoiGioHang(item.Id);
                if (!result)
                {
                    return Json(new { success = false, message = "Không thể xóa sản phẩm khỏi giỏ hàng" });
                }

                return Json(new { success = true, message = "Xóa sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaTatCa()
        {
            try
            {
                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng" });
                }

                var result = await _gioHangApiClient.XoaGioHang(gioHang.Id);
                if (!result)
                {
                    return Json(new { success = false, message = "Không thể xóa toàn bộ giỏ hàng" });
                }

                return Json(new { success = true, message = "Xóa toàn bộ giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaNhieuSanPham([FromBody] List<Guid> sanPhamChiTietIds)
        {
            try
            {
                if (sanPhamChiTietIds == null || !sanPhamChiTietIds.Any())
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất một sản phẩm để xóa" });
                }

                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng" });
                }

                var failedIds = new List<Guid>();
                foreach (var spctId in sanPhamChiTietIds)
                {
                    var item = gioHang.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == spctId);
                    if (item == null)
                    {
                        failedIds.Add(spctId);
                        continue;
                    }

                    var result = await _gioHangApiClient.XoaSanPhamKhoiGioHang(item.Id);
                    if (!result)
                    {
                        failedIds.Add(spctId);
                    }
                }

                if (failedIds.Any())
                {
                    return Json(new { success = false, message = $"Không thể xóa {failedIds.Count} sản phẩm. Vui lòng thử lại." });
                }

                return Json(new { success = true, message = "Xóa các sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApDungVoucher(string code, string selectedIds)
        {
            try
            {
                var selectedIdList = new List<Guid>();
                if (!string.IsNullOrEmpty(selectedIds))
                {
                    foreach (var id in selectedIds.Split(','))
                    {
                        if (Guid.TryParse(id, out Guid guid))
                            selectedIdList.Add(guid);
                    }
                }

                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null || !gioHang.GioHangChiTiets.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống, không thể áp dụng voucher!" });
                }

                // Lấy các sản phẩm đã chọn
                var selectedItems = gioHang.GioHangChiTiets.Where(x => selectedIdList.Contains(x.SanPhamChiTietId)).ToList();
                if (!selectedItems.Any())
                {
                    return Json(new { success = false, message = "Vui lòng chọn sản phẩm để áp dụng voucher!" });
                }

                // Tính subtotal chỉ trên selectedItems
                var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
                {
                    PageIndex = 1,
                    PageSize = 100,
                    Keyword = null,
                    TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
                });

                decimal GetGiaKhuyenMai(GioHangChiTietViewModel sp)
                {
                    var km = khuyenMais.Items
                        .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == sp.SanPhamChiTietId))
                        .OrderByDescending(x => x.ThoiGianBatDau)
                        .FirstOrDefault();

                    if (km != null)
                    {
                        if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                            return Math.Round(sp.DonGia * (1 - km.GiaTriGiamGia / 100m), 0);
                        else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                            return Math.Max(0, sp.DonGia - km.GiaTriGiamGia);
                    }
                    return sp.DonGia;
                }

                var subtotal = selectedItems.Sum(sp => GetGiaKhuyenMai(sp) * sp.SoLuong);

                // Lấy danh sách voucher đang hoạt động để kiểm tra
                var voucherPagingRequest = new GetVoucherPagingRequest
                {
                    PageIndex = 1,
                    PageSize = 100,
                    Keyword = null,
                    Status = TrangThaiGiamGia.HoatDong
                };
                var vouchers = await _voucherApiClient.GetAllPaging(voucherPagingRequest);
                var voucher = vouchers.Items.FirstOrDefault(v => v.MaVoucher == code);

                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã voucher không tồn tại hoặc không hoạt động!" });
                }

                if (voucher.SoLuong <= 0)
                {
                    return Json(new { success = false, message = "Voucher đã hết số lượng sử dụng!" });
                }

                if (subtotal < voucher.DieuKienApDung)
                {
                    return Json(new { success = false, message = $"Tổng đơn hàng phải đạt tối thiểu {voucher.DieuKienApDung:N0} VNĐ để sử dụng voucher này!" });
                }

                // Kiểm tra quyền sử dụng voucher riêng tư
                if (voucher.loaiVoucher == LoaiVoucher.RiengTu)
                {
                    var voucherUsers = await _voucherApiClient.GetUsersForVoucher(voucher.Id);
                    var isAssigned = voucherUsers.Any(vu => vu.Id == userId);
                    if (!isAssigned)
                    {
                        return Json(new { success = false, message = "Bạn không có quyền sử dụng voucher này!" });
                    }
                }

                // Áp dụng voucher qua API
                var canUse = await _voucherApiClient.UseVoucher(code, userId);
                if (!canUse)
                {
                    return Json(new { success = false, message = "Không thể sử dụng voucher này!" });
                }

                // Tính giá trị giảm giá
                decimal discountAmount = 0;
                if (voucher.LoaiGiamGia == LoaiGiamGia.PhamTram)
                {
                    discountAmount = Math.Round(subtotal * (voucher.GiaTriGiamGia / 100m), 0);
                }
                else if (voucher.LoaiGiamGia == LoaiGiamGia.SoTien)
                {
                    discountAmount = Math.Min(voucher.GiaTriGiamGia, subtotal); // Không vượt quá tổng tiền
                }

                // Lưu mã voucher vào Session để sử dụng trong checkout
                HttpContext.Session.SetString("AppliedVoucherCode", code);

                return Json(new
                {
                    success = true,
                    message = $"Áp dụng voucher {code} thành công! Giảm {discountAmount:N0} VNĐ",
                    voucher = new
                    {
                        voucher.Id,
                        voucher.MaVoucher,
                        voucher.GiaTriGiamGia,
                        voucher.LoaiGiamGia
                    },
                    discountAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProceedToCheckout(string selectedIds)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedIds))
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn sản phẩm để thanh toán.";
                    return RedirectToAction("Index");
                }

                // Phân tích selectedIds với xử lý lỗi
                var selectedIdList = new List<Guid>();
                foreach (var id in selectedIds.Split(','))
                {
                    if (Guid.TryParse(id, out Guid guid))
                    {
                        selectedIdList.Add(guid);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Một hoặc nhiều ID sản phẩm không hợp lệ.";
                        return RedirectToAction("Index");
                    }
                }

                var userId = GetUserId();
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy giỏ hàng.";
                    return RedirectToAction("Index");
                }

                var cartItems = gioHang.GioHangChiTiets?.Select(x => new GioHangItemViewModel
                {
                    SanPhamChiTietId = x.SanPhamChiTietId,
                    TenSanPham = x.TenSanPham ?? "Sản phẩm không xác định",
                    AnhSanPham = !string.IsNullOrEmpty(x.HinhAnh) ? x.HinhAnh : "/images/Default_Logo.png",
                    MauSac = x.MauSac,
                    KichThuoc = x.KichThuoc.ToString() ?? "N/A",
                    GiaGoc = x.DonGia,
                    GiaKhuyenMai = x.DonGia,
                    SoLuong = x.SoLuong,
                    TrangThai = true // Mặc định là true, sẽ được cập nhật sau
                }).ToList() ?? new List<GioHangItemViewModel>();

                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra trạng thái sản phẩm trước khi chuyển đến thanh toán
                var validSelectedItems = new List<GioHangItemViewModel>();
                var invalidProducts = new List<string>();

                foreach (var item in cartItems.Where(x => selectedIdList.Contains(x.SanPhamChiTietId)))
                {
                    var spct = await _spctApiClient.GetById(item.SanPhamChiTietId);
                    if (spct != null && spct.TrangThai)
                    {
                        item.TrangThai = spct.TrangThai;
                        item.SoLuongTon = spct.SoLuong;
                        validSelectedItems.Add(item);
                    }
                    else if (spct != null && !spct.TrangThai)
                    {
                        invalidProducts.Add($"{item.TenSanPham} (sản phẩm đã ngưng hoạt động)");
                    }
                }

                if (invalidProducts.Any())
                {
                    TempData["ErrorMessage"] = $"Sản phẩm sau không thể thanh toán: {string.Join(", ", invalidProducts)}. Vui lòng xóa những sản phẩm này khỏi giỏ hàng.";
                    return RedirectToAction("Index");
                }

                if (!validSelectedItems.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm hợp lệ để thanh toán.";
                    return RedirectToAction("Index");
                }

                // Lưu vào Session thay vì TempData
                HttpContext.Session.SetString("SelectedCartItems", JsonSerializer.Serialize(validSelectedItems));
                HttpContext.Session.SetString("UserId", userId.ToString());

                return RedirectToAction("Checkout", "ThanhToan");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi chuyển đến thanh toán: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
