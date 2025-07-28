using Microsoft.AspNetCore.Mvc;
using SneakFit.WebClient.Models;
using System.Diagnostics;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using SneakFit.Data.Entities;

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

        public HomeController(ILogger<HomeController> logger,
            ISanPhamApiClient sanPhamApiClient,
            IConfiguration configuration,
            ISpctApiClient spctApiClient,
            IDanhMucApiClient danhMucApiClient,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IThuongHieuApiClient thuongHieuApiClient)
        {
            _logger = logger;
            _sanPhamApiClient = sanPhamApiClient;
            _configuration = configuration;
            _spctApiClient = spctApiClient;
            _danhMucApiClient = danhMucApiClient;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
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

            // Gán ảnh đại diện cho từng sản phẩm
            foreach (var sanPham in pagedSanPham.Items)
            {
                // Lấy danh sách SPCT theo tên sản phẩm
                var spctList = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);
                allSpct.AddRange(spctList);

                // Lấy ảnh đầu tiên có trong danh sách SPCT
                var spctWithImage = spctList.FirstOrDefault(spct => spct.Images != null && spct.Images.Any());
                var firstImage = spctWithImage?.Images?.FirstOrDefault();

                // Gán đường dẫn đầy đủ
                sanPham.ImageDaiDien = !string.IsNullOrEmpty(firstImage)
                    ? baseAddress + firstImage
                    : baseAddress + "/assets/img/product/no-image.png";
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
            };

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
