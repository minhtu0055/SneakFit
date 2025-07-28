using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using System.Threading.Tasks;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.WebClient.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.Data.Enums;
using SneakFit.Data.Entities;
using System.Text.Json;
using SneakFit.ViewModels.Catalog.GioHang;

namespace SneakFit.WebClient.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IDanhMucApiClient _danhMucApiClient;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        private readonly IChatLieuApiClient _chatLieuApiClient;
        private readonly IDeGiayApiClient _deGiayApiClient;
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;
        private readonly IGioHangApiClient _gioHangApiClient;

        public SanPhamController(
            ISanPhamApiClient sanPhamApiClient,
            ISpctApiClient spctApiClient,
            IDanhMucApiClient danhMucApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IThuongHieuApiClient thuongHieuApiClient,
            IChatLieuApiClient chatLieuApiClient,
            IDeGiayApiClient deGiayApiClient,
            IKhuyenMaiApiClient khuyenMaiApiClient,
            IGioHangApiClient gioHangApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _spctApiClient = spctApiClient;
            _danhMucApiClient = danhMucApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _chatLieuApiClient = chatLieuApiClient;
            _deGiayApiClient = deGiayApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _gioHangApiClient = gioHangApiClient;
        }

        // Trang Index: chỉ fill danh sách SanPham + ảnh đại diện
        public async Task<IActionResult> Index(string tuKhoa, Guid? danhMucId, decimal? giaThapNhat, decimal? giaCaoNhat, int pageIndex = 1)
        {
            var categories = await _danhMucApiClient.GetAll();
            var colors = await _mauSacApiClient.GetAll();
            var brands = await _thuongHieuApiClient.GetAll();

            var request = new SanPhamPagingRequest
            {
                Keyword = tuKhoa,
                DanhMucId = danhMucId,
                TrangThai = true,
                PageIndex = pageIndex,
                PageSize = 10
            };
            var pagedSanPham = await _sanPhamApiClient.GetAllPaging(request);
            var allSpct = new List<SPCTViewModels>();

            // Lấy tất cả KM đang hoạt động
            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 10, // hoặc lớn hơn nếu nhiều KM
                Keyword = null,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });

            // Duyệt từng sản phẩm để gắn thông tin khuyến mãi và ảnh đại diện
            foreach (var sanPham in pagedSanPham.Items)
            {
                // Lấy danh sách SPCT theo tên sản phẩm
                var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);

                foreach (var spct in spctList)
                {
                    // Tìm KM hoạt động đúng với SPCT này
                    var km = khuyenMais.Items
                            .Where(x => x.TrangThai == TrangThaiGiamGia.HoatDong
                                        && x.ThoiGianBatDau <= DateTime.Now
                                        && x.ThoiGianKetThuc >= DateTime.Now
                                        && x.SanPhamChiTiets.Any(ct => ct.SPCTId == spct.Id))
                            .OrderByDescending(x => x.ThoiGianBatDau)
                            .FirstOrDefault();

                    spct.GiaGoc = spct.Gia;

                    if (km != null)
                    {
                        if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                        {
                            spct.KhuyenMaiPhanTram = km.GiaTriGiamGia;
                            spct.GiaKhuyenMai = Math.Round(spct.Gia * (1 - km.GiaTriGiamGia / 100), 0);
                        }
                        else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                        {
                            spct.GiaKhuyenMai = Math.Max(0, spct.Gia - km.GiaTriGiamGia);
                            spct.KhuyenMaiPhanTram = spct.Gia > 0 ? Math.Round((km.GiaTriGiamGia / spct.Gia) * 100, 0) : 0;
                        }
                    }
                    else
                    {
                        // Không có khuyến mại => phải reset các trường giảm giá
                        spct.GiaKhuyenMai = spct.Gia;
                        spct.KhuyenMaiPhanTram = 0;
                    }
                }

                allSpct.AddRange(spctList);
                // Lấy 1 ảnh đại diện cho mỗi sản phẩm
                var spctDaiDien = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
                sanPham.ImageDaiDien = spctDaiDien?.Images?.FirstOrDefault() ?? "/images/Default_Logo.png";
            }

            var viewModel = new DanhMucSPCTViewModel
            {
                DanhMucs = categories,
                MauSacs = colors,
                ThuongHieus = brands,
                SanPhams = pagedSanPham,
                AllSpct = allSpct,
            };

            return View(viewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddToCart(Guid sanPhamChiTietId, int soLuong = 1)
        //{
        //    // Nếu chưa login => giả lập userId tạm
        //    var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    Guid userId;

        //    if (string.IsNullOrEmpty(userIdStr))
        //    {
        //        // DEMO: Hardcode userId test nếu chưa làm login
        //        userId = Guid.Parse("69BD714F-9576-45BA-B5B7-F00649BE00DE"); // Gán tạm
        //                                                                     // Nếu muốn bắt buộc phải login thì return lỗi:
        //                                                                     // return Json(new { success = false, requireLogin = true, message = "Bạn cần đăng nhập để mua hàng" });
        //    }
        //    else
        //    {
        //        userId = Guid.Parse(userIdStr);
        //    }

        //    try
        //    {
        //        var request = new ThemVaoGioHangRequest
        //        {
        //            UserId = userId,
        //            SanPhamChiTietId = sanPhamChiTietId,
        //            SoLuong = soLuong
        //        };

        //        var result = await _gioHangApiClient.ThemVaoGioHang(request);
        //        return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cart = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Có lỗi: {ex.Message}" });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid sanPhamChiTietId, int soLuong = 1)
        {
            try
            {
                Guid userId;
                try
                {
                    userId = (User?.Identity?.IsAuthenticated ?? false)
                        ? Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                        : throw new UnauthorizedAccessException();
                }
                catch
                {
                    return Json(new { success = false, requireLogin = true, message = "Bạn cần đăng nhập để mua hàng." });
                }
                // 1. Lấy giỏ hàng của người dùng
                var gioHang = await _gioHangApiClient.GetByUserId(userId);
                var item = gioHang?.GioHangChiTiets?.FirstOrDefault(x => x.SanPhamChiTietId.Equals(sanPhamChiTietId));

                // 2. Lấy thông tin sản phẩm chi tiết
                var spct = await _spctApiClient.GetById(sanPhamChiTietId);
                if (spct == null || spct.SoLuong <= 0)
                {
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
                }

                // 3. Tính số lượng
                int soLuongTrongGio = item?.SoLuong ?? 0;
                int soLuongConLai = spct.SoLuong - soLuongTrongGio;

                // 4. Kiểm tra nếu giỏ hàng đã chứa số lượng tối đa
                if (soLuongTrongGio >= spct.SoLuong)
                {
                    return Json(new { success = false, message = $"Số lượng sản phẩm này trong giỏ hàng đã đạt số lượng tối đa ( {spct.SoLuong} )." });
                }

                // 5. Kiểm tra nếu số lượng yêu cầu vượt quá số lượng còn lại
                if (soLuong > soLuongConLai)
                {
                    return Json(new { success = false, message = $"Chỉ còn {soLuongConLai} sản phẩm có thể thêm vào giỏ hàng." });
                }

                // 6. Thêm vào giỏ hàng
                var request = new ThemVaoGioHangRequest
                {
                    UserId = userId,
                    SanPhamChiTietId = sanPhamChiTietId,
                    SoLuong = soLuong
                };

                var result = await _gioHangApiClient.ThemVaoGioHang(request);
                return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cart = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi: {ex.Message}" });
            }
        }

        // Trang Details: fill full SPCT của sản phẩm để chọn màu/size
        public async Task<IActionResult> Details(Guid id)
        {
            var sanPham = await _sanPhamApiClient.GetById(id);
            var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
            var mausacs = await _mauSacApiClient.GetAll();
            var kichthuocs = await _kichThuocApiClient.GetAll();

            // Đảm bảo đã lấy được danh sách KM HOẠT ĐỘNG
            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                Keyword = null,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });

            // Gắn khuyến mãi vào từng SPCT
            foreach (var spct in spctList)
            {
                var km = khuyenMais.Items
                    .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == spct.Id))
                    .OrderByDescending(x => x.ThoiGianBatDau)
                    .FirstOrDefault();

                if (km != null)
                {
                    spct.GiaGoc = spct.Gia;
                    if (km.LoaiGiamGia == LoaiGiamGia.PhamTram)
                    {
                        spct.KhuyenMaiPhanTram = km.GiaTriGiamGia;
                        spct.GiaKhuyenMai = Math.Round(spct.Gia * (1 - km.GiaTriGiamGia / 100), 0);
                    }
                    else if (km.LoaiGiamGia == LoaiGiamGia.SoTien)
                    {
                        spct.GiaKhuyenMai = Math.Max(0, spct.Gia - km.GiaTriGiamGia);
                        spct.KhuyenMaiPhanTram = spct.Gia > 0 ? Math.Round((km.GiaTriGiamGia / spct.Gia) * 100, 0) : 0;
                    }
                }
                else
                {
                    spct.GiaGoc = spct.Gia;
                    spct.GiaKhuyenMai = spct.Gia;
                    spct.KhuyenMaiPhanTram = 0;
                }
            }

            var viewModel = new SanPhamDetailViewModel
            {
                SanPham = sanPham,
                SanPhamChiTiets = spctList,
                MauSacs = mausacs,
                KichThuocs = kichthuocs,
            };

            return View(viewModel);
        }

    }
}