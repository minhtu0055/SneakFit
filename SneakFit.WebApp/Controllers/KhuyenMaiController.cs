using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;

namespace SneakFit.Admin.Controllers
{
    public class KhuyenMaiController : BaseController
    {
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;
       // private readonly ISanPhamApiClient _sanPhamApiClient;

        public KhuyenMaiController(IKhuyenMaiApiClient khuyenMaiApiClient /*, ISanPhamApiClient sanPhamApiClient*/)
        {
            _khuyenMaiApiClient = khuyenMaiApiClient;
           // _sanPhamApiClient = sanPhamApiClient;
        }

        public async Task<IActionResult> Index(string keyword, TrangThaiGiamGia? trangThai, int pageIndex = 1, int pageSize = 10)
        {
            var request = new PhanTrangKhuyenMai()
            {
                Keyword = keyword,
                TrangThai = trangThai,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var data = await _khuyenMaiApiClient.GetAllPaging(request);

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.StatusSelectList = GetTrangThaiSelectList(trangThai);

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SetLoaiGiamGiaViewBag();
          //  await SetSanPhamViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemKhuyenMai request)
        {
            if (!ModelState.IsValid)
            {
                SetLoaiGiamGiaViewBag();
              //  await SetSanPhamViewBag();
                return View(request);
            }

            try
            {
                var result = await _khuyenMaiApiClient.Create(request);
                TempData["SuccessMsg"] = "Tạo khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                SetLoaiGiamGiaViewBag();
               // await SetSanPhamViewBag();
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _khuyenMaiApiClient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("Index");
            }

            var editModel = new SuaKhuyenMai()
            {
                Id = result.Id,
                TenKhuyenMai = result.TenKhuyenMai,
                MoTa = result.MoTa,
                LoaiGiamGia = result.LoaiGiamGia,
                GiaTriGiamGia = result.GiaTriGiamGia,
                ThoiGianBatDau = result.ThoiGianBatDau,
                ThoiGianKetThuc = result.ThoiGianKetThuc,
                TrangThai = result.TrangThai,
                SanPhamIds = result.SanPhamIds
            };

            SetLoaiGiamGiaViewBag();
           // await SetSanPhamViewBag();
            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SuaKhuyenMai request)
        {
            if (!ModelState.IsValid)
            {
                SetLoaiGiamGiaViewBag();
              //  await SetSanPhamViewBag();
                return View(request);
            }

            try
            {
                var result = await _khuyenMaiApiClient.Update(request);
                TempData["SuccessMsg"] = "Cập nhật khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                SetLoaiGiamGiaViewBag();
               // await SetSanPhamViewBag();
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _khuyenMaiApiClient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("Index");
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, TrangThaiGiamGia trangThai)
        {
            try
            {
                var result = await _khuyenMaiApiClient.UpdateStatus(id, trangThai);
                if (result)
                {
                    TempData["SuccessMsg"] = "Cập nhật trạng thái khuyến mãi thành công";
                }
                else
                {
                    TempData["ErrorMsg"] = "Cập nhật trạng thái khuyến mãi thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void SetLoaiGiamGiaViewBag()
        {
            ViewBag.DiscountTypes = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };
        }

        //private async Task SetSanPhamViewBag()
        //{
        //    var products = await _sanPhamApiClient.GetAll();
        //    ViewBag.SanPhams = products;
        //}

        private List<SelectListItem> GetTrangThaiSelectList(TrangThaiGiamGia? trangThai)
        {
            return new List<SelectListItem>()
        {
            new SelectListItem("Tất cả", "", !trangThai.HasValue),
            new SelectListItem("Không hoạt động", ((int)TrangThaiGiamGia.KhongHoatDong).ToString(), (int?)trangThai == (int)TrangThaiGiamGia.KhongHoatDong),
            new SelectListItem("Hoạt động", ((int)TrangThaiGiamGia.HoatDong).ToString(), (int?)trangThai == (int)TrangThaiGiamGia.HoatDong),
            new SelectListItem("Hết hạn", ((int)TrangThaiGiamGia.HetHan).ToString(), (int?)trangThai == (int)TrangThaiGiamGia.HetHan)
        };
        }
    }
}
