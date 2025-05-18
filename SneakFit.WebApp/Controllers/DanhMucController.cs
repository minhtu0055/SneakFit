using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakFit.Admin.Controllers;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Common;

namespace SneakFit.WebApp.Controllers
{
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemDanhMuc request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _danhMucApiClient.Create(request);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Thêm mới danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm danh mục thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _danhMucApiClient.GetById(id);
            if (result != null)
            {
                var danhmuc = new SuaDanhMuc()
                {
                    Id = result.Id,
                    TenDanhMuc = result.TenDanhMuc
                };
                return View(danhmuc);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SuaDanhMuc request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _danhMucApiClient.Update(request);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật danh mục thất bại");
            return View(request);
        }
    }
}