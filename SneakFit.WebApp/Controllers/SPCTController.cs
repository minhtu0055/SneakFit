using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services.SPCT;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

namespace SneakFit.Admin.Controllers
{
    public class SPCTController : BaseController
    {
        private readonly ISpctApiClient _spctApiClient;
        private readonly IConfiguration _configuration;

        public SPCTController(
            ISpctApiClient spctApiClient,
            IConfiguration configuration)
        {
            _spctApiClient = spctApiClient;
            _configuration = configuration;
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
            await LoadDropdownData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemSPCT request)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(request);
            }
            var result = await _spctApiClient.Create(request);
            if (result != null)
            {
                TempData["success"] = "Thêm mới sản phẩm chi tiết thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Thêm mới sản phẩm chi tiết thất bại");
            await LoadDropdownData();
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _spctApiClient.GetById(id);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            await LoadDropdownData();
            var updateRequest = new SuaSPCT()
            {
                Id = result.Id,
                Gia = result.Gia,
                SoLuong = result.SoLuong,
                MauSacId = result.MauSacId,
                KichThuocId = result.KichThuocId,
                ChatLieuId = result.ChatLieuId,
                DeGiayId = result.DeGiayId,
                ThuongHieuId = result.ThuongHieuId,
                SanPhamId = result.SanPhamId,
                DanhMucId = result.DanhMucId,
                TrangThai = result.TrangThai
            };
            return View(updateRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SuaSPCT request)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(request);
            }
            var result = await _spctApiClient.Update(request);
            if (result != null)
            {
                TempData["success"] = "Cập nhật sản phẩm chi tiết thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Cập nhật sản phẩm chi tiết thất bại");
            await LoadDropdownData();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, bool trangThai)
        {
            try
            {
                await _spctApiClient.UpdateTrangThai(id, trangThai);
                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatGia(Guid id, decimal giaMoi)
        {
            try
            {
                await _spctApiClient.UpdateGia(id, giaMoi);
                return Json(new { success = true, message = "Cập nhật giá thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(Guid id, int themSoLuong)
        {
            try
            {
                await _spctApiClient.UpdateSoLuong(id, themSoLuong);
                return Json(new { success = true, message = "Cập nhật số lượng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task LoadDropdownData()
        {
            
        }
    }
}
