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
using System.Collections.Generic;

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
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _spctApiClient.GetAllPaging(request);
            ViewBag.TuKhoa = tuKhoa;

            // Load all data cần thiết song song
            var mauSacs = await _mauSacApiClient.GetAll();
            var kichThuocs = await _kichThuocApiClient.GetAll();
            var chatLieus = await _chatLieuApiClient.GetAll();
            var deGiays = await _deGiayApiClient.GetAll();
            var thuongHieus = await _thuongHieuApiClient.GetAll();
            var sanPhams = await _sanPhamApiClient.GetAll();

            // Sử dụng Dictionary để truy xuất nhanh hơn vì mỗi lần truy xuất sẽ không phải gọi lại API
            // 1 Dictionary gồm 1 key và 1 value
            ViewBag.MauSacs = mauSacs.ToDictionary(x => x.Id, x => x.TenMauSac);
            ViewBag.KichThuocs = kichThuocs.ToDictionary(x => x.Id, x => x.MaKichThuoc.ToString());
            ViewBag.ChatLieus = chatLieus.ToDictionary(x => x.Id, x => x.TenChatLieu);
            ViewBag.DeGiays = deGiays.ToDictionary(x => x.Id, x => x.TenDeGiay);
            ViewBag.ThuongHieus = thuongHieus.ToDictionary(x => x.Id, x => x.TenThuongHieu);
            ViewBag.SanPhams = sanPhams.ToDictionary(x => x.Id, x => x.TenSanPham);

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        private async Task LoadCombobox()
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
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try 
            {
                await LoadCombobox();
                return PartialView("Create");
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
                await LoadCombobox();
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

        [HttpPost]
        public async Task<IActionResult> CreateMultiple([FromBody] List<SPCTViewModels> items)
        {
            if (items == null || !items.Any())
                return Json(new { success = false, message = "Dữ liệu không hợp lệ hoặc chưa chọn màu/kích thước" });

            try
            {
                int result = 0;
                foreach (var item in items)
                {
                    var request = new ThemSPCT
                    {
                        // Gán các trường tương ứng từ item sang request
                        SanPhamId = item.SanPhamId,
                        ThuongHieuId = item.ThuongHieuId,
                        ChatLieuId = item.ChatLieuId,
                        DeGiayId = item.DeGiayId,
                        MauSacId = item.MauSacId,
                        KichThuocId = item.KichThuocId,
                        SoLuong = item.SoLuong,
                        Gia = item.Gia
                        // ... các trường khác nếu cần
                    };

                    var res = await _spctApiClient.Create(request);
                    if (res != null) result++;
                }

                if (result > 0)
                {
                    return Json(new { success = true, message = $"Thêm mới {result} sản phẩm chi tiết thành công" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = false, message = "Thêm sản phẩm chi tiết thất bại" });
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
