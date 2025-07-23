using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.ThuongHieu;

namespace SneakFit.Admin.Controllers
{
    public class ThuongHieuController : BaseController
    {
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        public ThuongHieuController(IThuongHieuApiClient thuongHieuApiClient)
        {
            _thuongHieuApiClient = thuongHieuApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, int pageIndex = 1, int pageSize = 8)
        {
            var request = new ThuongHieuPagingRequest()
            {
                Keyword = keyWord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _thuongHieuApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyWord;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemThuongHieu request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _thuongHieuApiClient.Create(request);
                if (result != null && result.Id != Guid.Empty)
                {
                    // Trả về id và tên để JS có thể chọn lại dropdown
                    return Json(new { success = true, id = result.Id, name = result.TenThuongHieu });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm thương hiệu thất bại" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var thuongHieu = await _thuongHieuApiClient.GetById(id);
            if (thuongHieu == null)
                return NotFound();

            var editModel = new SuaThuongHieu
            {
                Id = thuongHieu.Id,
                TenThuongHieu = thuongHieu.TenThuongHieu
            };
            return PartialView("Edit", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaThuongHieu request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _thuongHieuApiClient.Update(request);
                if (result != null)
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _thuongHieuApiClient.GetAll();
            return Ok(result);
        }
    }
}