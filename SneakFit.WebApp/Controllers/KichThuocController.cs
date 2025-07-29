using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.KichThuoc;

namespace SneakFit.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KichThuocController : BaseController
    {
        private readonly IKichThuocApiClient _kichThuocApiClient;
        public KichThuocController(IKichThuocApiClient kichThuocApiClient)
        {
            _kichThuocApiClient = kichThuocApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, int pageIndex = 1, int pageSize = 8)
        {
            var request = new KichThuocPagingRequest()
            {
                Keyword = keyWord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _kichThuocApiClient.GetAllPaging(request);
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
        public async Task<IActionResult> Create(ThemKichThuoc request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                // Giả định rằng _kichThuocApiClient.Create trả về đối tượng KichThuocViewModels
                var result = await _kichThuocApiClient.Create(request);
                if (result != null && result.Id != Guid.Empty)
                {
                    // Trả về JSON với thuộc tính 'name' để Javascript sử dụng
                    return Json(new { success = true, id = result.Id, name = result.MaKichThuoc });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm kích thước thất bại" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chatLieu = await _kichThuocApiClient.GetById(id);
            if (chatLieu == null)
                return NotFound();

            var editModel = new SuaKichThuoc
            {
                Id = chatLieu.Id,
                MaKichThuoc = chatLieu.MaKichThuoc
            };
            return PartialView("Edit", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaKichThuoc request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _kichThuocApiClient.Update(request);
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kichThuocs = await _kichThuocApiClient.GetAll(); // Gọi sang API backend
            return Json(kichThuocs);
        }
    }
}
