using Microsoft.AspNetCore.Mvc;
using SneakFit.WebClient.Models;
using System.Diagnostics;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;

namespace SneakFit.WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly IConfiguration _configuration;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IDanhMucApiClient _danhMucApiClient;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        private readonly IThongKeApiClient _thongKeApiClient;
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;

        public HomeController(ILogger<HomeController> logger,
            ISanPhamApiClient sanPhamApiClient,
            IConfiguration configuration,
            ISpctApiClient spctApiClient,
            IDanhMucApiClient danhMucApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IThuongHieuApiClient thuongHieuApiClient,
            IThongKeApiClient thongKeApiClient,
            IKhuyenMaiApiClient khuyenMaiApiClient)
        {
            _logger = logger;
            _sanPhamApiClient = sanPhamApiClient;
            _configuration = configuration;
            _spctApiClient = spctApiClient;
            _danhMucApiClient = danhMucApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _thongKeApiClient = thongKeApiClient;
            _khuyenMaiApiClient = khuyenMaiApiClient;
        }

        public async Task<IActionResult> Index(string tuKhoa, Guid? danhMucId, decimal? giaThapNhat, decimal? giaCaoNhat, int pageIndex = 1)
        {
            // Kiểm tra thông báo lỗi thanh toán
            if (TempData["PaymentError"] != null)
            {
                ViewBag.PaymentError = TempData["PaymentError"];
            }
            
            var categories = await _danhMucApiClient.GetAll();
            var colors = await _mauSacApiClient.GetAll();
            var brands = await _thuongHieuApiClient.GetAll();
            var baseAddress = _configuration["BaseAddress"]; // ✅ Lấy baseAddress từ cấu hình

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

            //tìm kiếm product
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                tuKhoa = tuKhoa.ToLower().Trim();
                pagedSanPham.Items = pagedSanPham.Items
                    .Where(x => x.TenSanPham?.ToLower().Contains(tuKhoa) == true)
                    .ToList();
            }

            ViewBag.Keyword = tuKhoa;

            // Lấy danh sách khuyến mãi hoạt động
            var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
            {
                PageIndex = 1,
                PageSize = 100,
                TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
            });

            // Gán ảnh đại diện cho từng sản phẩm và áp dụng khuyến mãi
            foreach (var sanPham in pagedSanPham.Items)
            {
                // Lấy danh sách SPCT theo tên sản phẩm
                var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
                
                // Áp dụng khuyến mãi cho từng SPCT
                foreach (var spct in spctList)
                {
                    // Tìm khuyến mãi áp dụng cho SPCT này
                    var km = khuyenMais.Items
                        .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == spct.Id))
                        .OrderByDescending(x => x.ThoiGianBatDau)
                        .FirstOrDefault();

                    if (km != null)
                    {
                        spct.GiaGoc = spct.Gia;
                        spct.KhuyenMaiId = km.Id;
                        
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
                        spct.KhuyenMaiId = null;
                    }
                }
                
                allSpct.AddRange(spctList);

                // Lấy ảnh đầu tiên có trong danh sách SPCT
                var spctWithImage = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
                var firstImage = spctWithImage?.Images?.FirstOrDefault();

                // Gán đường dẫn đầy đủ
                sanPham.ImageDaiDien = !string.IsNullOrEmpty(firstImage)
                    ? baseAddress + firstImage
                    : baseAddress + "/assets/img/SneakFit_Logo.png";
            }

            // Lấy 1 ảnh đại diện cho mỗi sản phẩm
            //foreach (var sanPham in pagedSanPham.Items)
            //{
            //    // Lấy danh sách SPCT theo tên sản phẩm
            //    var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
            //    allSpct.AddRange(spctList);
            //    var spctDaiDien = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
            //    sanPham.ImageDaiDien = spctDaiDien?.Images?.FirstOrDefault() ?? "/images/Default.jpg";
            //}

            var viewModel = new DanhMucSPCTViewModel
            {
                DanhMucs = categories,
                MauSacs = colors,
                ThuongHieus = brands,
                SanPhams = pagedSanPham,
                AllSpct = allSpct,
                BestSellerSpct = new List<SneakFit.ViewModels.Catalog.ThongKe.SanPhamChiTietThongKeViewModel>()
            };

            // Lấy danh sách sản phẩm bán chạy (SoLuongDaBan > 5)
            var topBanChay = await _thongKeApiClient.GetTopSanPhamBanChayAsync(10);
            foreach (var sp in topBanChay)
            {
                var spctBanChay = await _thongKeApiClient.GetSanPhamChiTietBanChayThongKeAsync(sp.SanPhamId);
                if (spctBanChay != null && spctBanChay.Any())
                {
                    viewModel.BestSellerSpct.AddRange(spctBanChay);
                }
            }

            return View(viewModel);

            //var products = await _sanPhamApiClient.GetAll(); // ✅ Trả về List<SanPhamViewModels>
            //var baseAddress = _configuration["BaseAddress"]; // ✅ Lấy baseAddress từ cấu hình
            //var allSpct = new List<SPCTViewModels>();

            //foreach (var sanPham in products)
            //{
            //    // ✅ Lấy danh sách SPCT theo ID (an toàn hơn theo tên)
            //    var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);

            //    // ✅ Tìm SPCT có ảnh
            //    var spctWithImage = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
            //    var firstImage = spctWithImage?.Images?.FirstOrDefault();

            //    // ✅ Gán ảnh đầy đủ đường dẫn
            //    sanPham.ImageDaiDien = !string.IsNullOrEmpty(firstImage)
            //        ? baseAddress + firstImage
            //        : baseAddress + "/images/Default.jpg";
            //}

            //var model = new SanPhamIndexViewModel
            //{
            //    SanPhams = products
            //};

            //return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        // Phương thức lấy dữ liệu cho modal sản phẩm (tương tự SanPham/Details)
        [HttpGet]
        public async Task<IActionResult> GetProductModalData(Guid productId)
        {
            try
            {
                // Lấy thông tin sản phẩm
                var sanPham = await _sanPhamApiClient.GetById(productId);
                if (sanPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Lấy danh sách SPCT của sản phẩm
                var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
                
                // Lấy danh sách màu sắc và kích thước
                var mausacs = await _mauSacApiClient.GetAll();
                var kichthuocs = await _kichThuocApiClient.GetAll();

                // Đảm bảo đã lấy được danh sách KM HOẠT ĐỘNG
                var khuyenMais = await _khuyenMaiApiClient.GetAllPaging(new PhanTrangKhuyenMai
                {
                    PageIndex = 1,
                    PageSize = 100,
                    TrangThai = SneakFit.Data.Enums.TrangThaiGiamGia.HoatDong
                });

                // Gắn khuyến mãi vào từng SPCT
                foreach (var spct in spctList)
                {
                    // Tìm khuyến mãi áp dụng cho SPCT này
                    var km = khuyenMais.Items
                        .Where(x => x.SanPhamChiTiets != null && x.SanPhamChiTiets.Any(ct => ct.SPCTId == spct.Id))
                        .OrderByDescending(x => x.ThoiGianBatDau)
                        .FirstOrDefault();

                    if (km != null)
                    {
                        spct.GiaGoc = spct.Gia;
                        spct.KhuyenMaiId = km.Id;
                        
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
                        spct.KhuyenMaiId = null;
                    }
                }

                // Tạo response data với đầy đủ thông tin khuyến mãi
                var responseData = new
                {
                    success = true,
                    product = new
                    {
                        id = sanPham.Id,
                        name = sanPham.TenSanPham,
                        image = sanPham.ImageDaiDien
                    },
                    spctList = spctList.Select(spct => new
                    {
                        id = spct.Id,
                        sanPhamId = spct.SanPhamId,
                        mauSac = spct.TenMauSac,
                        mauSacId = spct.MauSacId,
                        kichThuoc = spct.MaKichThuoc,
                        kichThuocId = spct.KichThuocId,
                        gia = spct.Gia,
                        giaGoc = spct.GiaGoc,
                        giaKhuyenMai = spct.GiaKhuyenMai,
                        khuyenMaiPhanTram = spct.KhuyenMaiPhanTram,
                        khuyenMaiId = spct.KhuyenMaiId,
                        soLuong = spct.SoLuong,
                        trangThai = spct.TrangThai,
                        images = spct.Images
                    }).ToList(),
                    colors = mausacs.Select(color => new
                    {
                        id = color.Id,
                        name = color.TenMauSac,
                        hex = color.MaMauSac
                    }).ToList(),
                    sizes = kichthuocs.Select(size => new
                    {
                        id = size.Id,
                        name = size.MaKichThuoc
                    }).ToList()
                };

                return Json(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu modal sản phẩm");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
