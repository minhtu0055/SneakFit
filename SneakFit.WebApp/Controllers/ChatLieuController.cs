using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.ChatLieu;

namespace SneakFit.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChatLieuController : BaseController
    {
        private readonly IChatLieuApiClient _chatLieuApiClient;
        public ChatLieuController(IChatLieuApiClient chatLieuApiClient)
        {
            _chatLieuApiClient = chatLieuApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, int pageIndex = 1, int pageSize = 8)
        {
            var request = new ChatLieuPagingRequest()
            {
                Keyword = keyWord,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _chatLieuApiClient.GetAllPaging(request);
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
        public async Task<IActionResult> Create(ThemChatLieu request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _chatLieuApiClient.Create(request);
                if (result != null && result.Id != Guid.Empty)
                {
                    return Json(new { success = true, id = result.Id, name = result.TenChatLieu });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm chất liệu thất bại" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chatLieu = await _chatLieuApiClient.GetById(id);
            if (chatLieu == null)
                return NotFound();

            var editModel = new SuaChatLieu
            {
                Id = chatLieu.Id,
                TenChatLieu = chatLieu.TenChatLieu
            };
            return PartialView("Edit", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaChatLieu request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _chatLieuApiClient.Update(request);
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
            var result = await _chatLieuApiClient.GetAll();
            return Ok(result);
        }
    }
}
