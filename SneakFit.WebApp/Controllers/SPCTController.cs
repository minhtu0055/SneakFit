using Microsoft.AspNetCore.Mvc;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ApiIntegration.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.DeGiay;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Catalog.SanPham;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SneakFit.Admin.Controllers
{
    public class SPCTController : BaseController
    {
        private readonly ISpctApiClient _spctApiClient;
        private readonly IConfiguration _configuration;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IChatLieuApiClient _chatLieuApiClient;
        private readonly IDeGiayApiClient _deGiayApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ILogger<SPCTController> _logger;

        public SPCTController(
            ISpctApiClient spctApiClient,
            IConfiguration configuration,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IChatLieuApiClient chatLieuApiClient,
            IDeGiayApiClient deGiayApiClient,
            IThuongHieuApiClient thuongHieuApiClient,
            ISanPhamApiClient sanPhamApiClient,
            ILogger<SPCTController> logger)
        {
            _spctApiClient = spctApiClient;
            _configuration = configuration;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _chatLieuApiClient = chatLieuApiClient;
            _deGiayApiClient = deGiayApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string tuKhoa, int pageIndex = 1, int pageSize = 10)
        {
            var request = new PhanTrangSPCT()
            {
                TuKhoa = tuKhoa,
                //TrangThai = trangThai,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _spctApiClient.GetAllPaging(request);
            ViewBag.TuKhoa = tuKhoa;
            //ViewBag.TrangThai = trangThai?.ToString().ToLower();
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try 
            {
                var mauSacs = await _mauSacApiClient.GetAll();
                var kichThuocs = await _kichThuocApiClient.GetAll();
                var chatLieus = await _chatLieuApiClient.GetAll();
                var deGiays = await _deGiayApiClient.GetAll();
                var thuongHieus = await _thuongHieuApiClient.GetAll();
                var sanPhams = await _sanPhamApiClient.GetAll();

                ViewBag.MauSacs = mauSacs.Select(x => new SelectListItem()
                {
                    Text = x.TenMauSac,
                    Value = x.Id.ToString()
                });
                ViewBag.KichThuocs = kichThuocs.Select(x => new SelectListItem()
                {
                    Text = x.MaKichThuoc.ToString(),
                    Value = x.Id.ToString()
                });
                ViewBag.ChatLieus = chatLieus.Select(x => new SelectListItem()
                {
                    Text = x.TenChatLieu,
                    Value = x.Id.ToString()
                });
                ViewBag.DeGiays = deGiays.Select(x => new SelectListItem()
                {
                    Text = x.TenDeGiay,
                    Value = x.Id.ToString()
                });
                ViewBag.ThuongHieus = thuongHieus.Select(x => new SelectListItem()
                {
                    Text = x.TenThuongHieu,
                    Value = x.Id.ToString()
                });
                ViewBag.SanPhams = sanPhams.Select(x => new SelectListItem()
                {
                    Text = x.TenSanPham,
                    Value = x.Id.ToString()
                });
                return PartialView("Create"); // Trả về partial view thay vì view hoàn chỉnh
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi load form thêm mới");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải form" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemSPCT request)
        {
            if (!ModelState.IsValid)
            {
                var mauSacs = await _mauSacApiClient.GetAll();
                var kichThuocs = await _kichThuocApiClient.GetAll();
                var chatLieus = await _chatLieuApiClient.GetAll();
                var deGiays = await _deGiayApiClient.GetAll();
                var thuongHieus = await _thuongHieuApiClient.GetAll();
                var sanPhams = await _sanPhamApiClient.GetAll();

                ViewBag.MauSacs = mauSacs.Select(x => new SelectListItem()
                {
                    Text = x.TenMauSac,
                    Value = x.Id.ToString()
                });
                ViewBag.KichThuocs = kichThuocs.Select(x => new SelectListItem()
                {
                    Text = x.MaKichThuoc.ToString(),
                    Value = x.Id.ToString()
                });
                ViewBag.ChatLieus = chatLieus.Select(x => new SelectListItem()
                {
                    Text = x.TenChatLieu,
                    Value = x.Id.ToString()
                });
                ViewBag.DeGiays = deGiays.Select(x => new SelectListItem()
                {
                    Text = x.TenDeGiay,
                    Value = x.Id.ToString()
                });
                ViewBag.ThuongHieus = thuongHieus.Select(x => new SelectListItem()
                {
                    Text = x.TenThuongHieu,
                    Value = x.Id.ToString()
                });
                ViewBag.SanPhams = sanPhams.Select(x => new SelectListItem()
                {
                    Text = x.TenSanPham,
                    Value = x.Id.ToString()
                });
                return View(request);
            }

            try
            {
                var result = await _spctApiClient.Create(request);
                if (result != null)
                {
                    TempData["result"] = "Thêm mới sản phẩm chi tiết thành công";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            ModelState.AddModelError("", "Thêm sản phẩm chi tiết thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> GetMauSacById(Guid id)
        {
            try
            {
                var mauSac = await _mauSacApiClient.GetById(id);
                return Json(new { success = true, resultObj = mauSac });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin màu sắc");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin màu sắc" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetKichThuocById(Guid id)
        {
            try
            {
                var kichThuoc = await _kichThuocApiClient.GetById(id);
                return Json(new { success = true, resultObj = kichThuoc });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin kích thước");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin kích thước" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChatLieuById(Guid id)
        {
            try
            {
                var chatLieu = await _chatLieuApiClient.GetById(id);
                return Json(new { success = true, resultObj = chatLieu });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chất liệu");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin chất liệu" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeGiayById(Guid id)
        {
            try
            {
                var deGiay = await _deGiayApiClient.GetById(id);
                return Json(new { success = true, resultObj = deGiay });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin đế giày");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin đế giày" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetThuongHieuById(Guid id)
        {
            try
            {
                var thuongHieu = await _thuongHieuApiClient.GetById(id);
                return Json(new { success = true, resultObj = thuongHieu });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin thương hiệu");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin thương hiệu" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSanPhamById(Guid id)
        {
            try
            {
                var sanPham = await _sanPhamApiClient.GetById(id);
                return Json(new { success = true, resultObj = sanPham });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sản phẩm");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin sản phẩm" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, bool trangThai)
        {
            try
            {
                var result = await _spctApiClient.UpdateTrangThai(id, trangThai);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật trạng thái thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatGia(Guid id, decimal giaMoi)
        {
            try
            {
                var result = await _spctApiClient.UpdateGia(id, giaMoi);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật giá thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giá");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật giá" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(Guid id, int soLuongThem)
        {
            try
            {
                var result = await _spctApiClient.UpdateSoLuong(id, soLuongThem);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật số lượng thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật số lượng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
            }
        }
    }
}
