using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.MauSac;

namespace SneakFit.Admin.Controllers
{
    public class MauSacController : BaseController
    {
        private readonly IMauSacApiClient _mauSacApiClient;
        public MauSacController(IMauSacApiClient mauSacApiClient)
        {
            _mauSacApiClient = mauSacApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, int pageIndex = 1, int pageSize = 8)
        {
            var request = new MauSacPagingRequest()
            {
                Keyword = keyWord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _mauSacApiClient.GetAllPaging(request);
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
        public async Task<IActionResult> Create(ThemMauSac request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _mauSacApiClient.Create(request);
                if (result != null)
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm màu sắc thất bại" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chatLieu = await _mauSacApiClient.GetById(id);
            if (chatLieu == null)
                return NotFound();

            var editModel = new SuaMauSac
            {
                Id = chatLieu.Id,
                TenMauSac = chatLieu.TenMauSac
            };
            return PartialView("Edit", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaMauSac request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _mauSacApiClient.Update(request);
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
            var result = await _mauSacApiClient.GetAll();
            return Ok(result);
        }
    }
}
