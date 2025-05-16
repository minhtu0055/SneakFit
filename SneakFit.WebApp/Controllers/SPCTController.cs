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
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _spctApiClient.GetAllPaging(request);
            ViewBag.TuKhoa = tuKhoa;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
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

            return View();
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
    }
}
