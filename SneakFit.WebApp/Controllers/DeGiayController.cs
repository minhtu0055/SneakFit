using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.DeGiay;

namespace SneakFit.Admin.Controllers
{
    public class DeGiayController : BaseController
    {
        private readonly IDeGiayApiClient _deGiayApiClient;
        public DeGiayController(IDeGiayApiClient deGiayApiClient)
        {
            _deGiayApiClient = deGiayApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, int pageIndex = 1, int pageSize = 8)
        {
            var request = new DeGiayPagingRequest()
            {
                Keyword = keyWord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _deGiayApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyWord;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
             return  PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemDeGiay request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _deGiayApiClient.Create(request);
                if (result != null)
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm đế giày thất bại" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var degiay = await _deGiayApiClient.GetById(id);
            if (degiay == null)
                return NotFound();

            var editModel = new SuaDeGiay
            {
                Id = degiay.Id,
                TenDeGiay = degiay.TenDeGiay
            };
            return PartialView("Edit", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaDeGiay request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _deGiayApiClient.Update(request);
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
            var result = await _deGiayApiClient.GetAll();
            return Ok(result);
        }
    }
}
