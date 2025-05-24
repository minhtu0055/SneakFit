using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.SanPham;

namespace SneakFit.Admin.Controllers
{
    public class SanPhamController : BaseController
    {
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly IDanhMucApiClient _danhMucApiClient;

        public SanPhamController(ISanPhamApiClient sanPhamApiClient, IDanhMucApiClient danhMucApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _danhMucApiClient = danhMucApiClient;
        }

        public async Task<IActionResult> Index(string keyWord, Guid? danhMucId, int pageIndex = 1, int pageSize = 8)
        {
            var request = new SanPhamPagingRequest()
            {
                Keyword = keyWord,
                //DanhMucId = danhMucId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var data = await _sanPhamApiClient.GetAllPaging(request);
            var danhmucs = await _danhMucApiClient.GetAll();

            ViewBag.Keyword = keyWord;
            ViewBag.DanhMucs = danhmucs.Select(x => new SelectListItem()
            {
                Text = x.TenDanhMuc,
                Value = x.Id.ToString(),
                Selected = danhMucId.HasValue && danhMucId.Value == x.Id
            });

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var danhmucs = await _danhMucApiClient.GetAll();
            ViewBag.DanhMucs = danhmucs.Select(x => new SelectListItem()
            {
                Text = x.TenDanhMuc,
                Value = x.Id.ToString()
            });
            return PartialView("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemSanPham request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _sanPhamApiClient.Create(request);
                if (result != null)
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Thêm sản phẩm thất bại" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var sanPham = await _sanPhamApiClient.GetById(id);
            if (sanPham == null)
                return NotFound();

            var danhmucs = await _danhMucApiClient.GetAll();
            ViewBag.DanhMucs = danhmucs.Select(x => new SelectListItem()
            {
                Text = x.TenDanhMuc,
                Value = x.Id.ToString(),
                Selected = sanPham.DanhMucId == x.Id
            });

            var editModel = new SuaSanPham
            {
                Id = sanPham.Id,
                TenSanPham = sanPham.TenSanPham,
                Mota = sanPham.Mota,
                DanhMucId = sanPham.DanhMucId
            };
            return PartialView("Edit", editModel);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuaSanPham request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _sanPhamApiClient.Update(request);
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
            var result = await _sanPhamApiClient.GetAll();
            return Ok(result);
        }
    }
}