using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.WebClient.Models;
using System.Collections.Generic;
using System.Text.Json;

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

        public GioHangController(
            ISanPhamApiClient sanPhamApiClient,
            ISpctApiClient spctApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IKhuyenMaiApiClient khuyenMaiApiClient,
            IGioHangApiClient gioHangApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _gioHangApiClient = gioHangApiClient;
        }

        private Guid GetUserId()
        {
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdStr)
                ? Guid.Parse("69BD714F-9576-45BA-B5B7-F00649BE00DE") // hardcode for demo
                : Guid.Parse(userIdStr);
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            var gioHang = await _gioHangApiClient.GetByUserId(userId);
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
            }).ToList() ?? new List<GioHangItemViewModel>();

            // Lấy danh sách khuyến mãi đang hoạt động
            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                Keyword = null,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });

            // Gắn giá KM vào từng item
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

            return View(list);
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

                return Json(new
                {
                    success = true,
                    soLuong = item.SoLuong, // Trả về số lượng thực tế từ DB
                    thanhTien = thanhTien,
                    tongTien = tongTien
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> CapNhatSoLuong(Guid sanPhamChiTietId, int soLuong)
        //{
        //    // Cập nhật logic cart ở DB/session...
        //    // Sau khi cập nhật xong:
        //    var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    Guid userId = string.IsNullOrEmpty(userIdStr)
        //        ? Guid.Parse("69BD714F-9576-45BA-B5B7-F00649BE00DE")
        //        : Guid.Parse(userIdStr);

        //    // Gọi API update số lượng vào DB
        //    var request = new CapNhatGioHang
        //    {
        //        UserId = userId,
        //        SanPhamChiTietId = sanPhamChiTietId,
        //        SoLuong = soLuong
        //    };
        //    var result = await _gioHangApiClient.CapNhatSoLuong(request);

        //    if (!result.IsSuccessed)
        //        return Json(new { success = false, message = result.Message });

        //    // Lấy lại cart mới nhất từ DB/session:
        //    var gioHang = _gioHangApiClient.GetByUserId(userId).Result;
        //    var item = gioHang?.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == sanPhamChiTietId);

        //    return Json(new
        //    {
        //        success = true,
        //        soLuong = item?.SoLuong ?? soLuong, // confirm lại số lượng thực tế (đề phòng backend auto fix)
        //        thanhTien = item?.ThanhTien ?? 0,
        //        tongTien = gioHang?.GioHangChiTiets?.Sum(x => x.ThanhTien) ?? 0
        //    });
        //}


        //[HttpPost]
        //public IActionResult XoaSanPham(Guid sanPhamChiTietId)
        //{
        //    var gioHang = HttpContext.Session.GetObjectFromJson<List<GioHangItemViewModel>>("GioHang") ?? new List<GioHangItemViewModel>();
        //    gioHang.RemoveAll(x => x.SanPhamChiTietId == sanPhamChiTietId);
        //    HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
        //    return Json(new
        //    {
        //        success = true,
        //        tongTien = gioHang.Sum(x => x.ThanhTien)
        //    });
        //}
        [HttpPost]
        public async Task<IActionResult> XoaSanPham(Guid sanPhamChiTietId)
        {
            try
            {
                var userId = GetUserId();

                // Lấy giỏ hàng hiện tại
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                if (gioHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng" });
                }

                // Tìm item cần xóa
                var item = gioHang.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId == sanPhamChiTietId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                // Gọi API xóa sản phẩm (bạn cần implement method này trong IGioHangApiClient)
                // var result = await _gioHangApiClient.XoaSanPham(item.Id);

                // Tạm thời return success để test
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //[HttpPost]
        //public IActionResult XoaTatCa()
        //{
        //    HttpContext.Session.Remove("GioHang");
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> XoaTatCa()
        {
            try
            {
                var userId = GetUserId();

                // Gọi API xóa toàn bộ giỏ hàng (bạn cần implement method này)
                var result = await _gioHangApiClient.XoaGioHang(userId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
