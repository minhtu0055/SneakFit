using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakFit.Admin.Controllers;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;

namespace SneakFit.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DanhMucController : BaseController
    {
        private readonly IDanhMucApiClient _danhMucApiClient;

        public DanhMucController(IDanhMucApiClient danhMucApiClient)
        {
            _danhMucApiClient = danhMucApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new DanhMucPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _danhMucApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemDanhMuc request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _danhMucApiClient.Create(request);
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
            var chatLieu = await _danhMucApiClient.GetById(id);
            if (chatLieu == null)
                return NotFound();

            var editModel = new SuaDanhMuc
            {
                Id = chatLieu.Id,
                TenDanhMuc = chatLieu.TenDanhMuc
            };
            return PartialView("Edit", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaDanhMuc request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _danhMucApiClient.Update(request);
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
            var result = await _danhMucApiClient.GetAll();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProductCount(Guid id)
        {
            var result = await _danhMucApiClient.UpdateProductCount(id);
            return Json(result);
        }
    }
}