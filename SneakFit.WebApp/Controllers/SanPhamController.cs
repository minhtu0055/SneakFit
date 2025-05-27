using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using Microsoft.AspNetCore.Http;

namespace SneakFit.Admin.Controllers
{
    public class SanPhamController : BaseController
    {
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly IDanhMucApiClient _danhMucApiClient;
        private readonly ISpctApiClient _spctApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IChatLieuApiClient _chatLieuApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        private readonly IDeGiayApiClient _deGiayApiClient;

        private readonly IConfiguration _configuration;

        public SanPhamController(ISanPhamApiClient sanPhamApiClient, 
                                IDanhMucApiClient danhMucApiClient,
                                ISpctApiClient spctApiClient,
                                IKichThuocApiClient kichThuocApiClient,
                                IMauSacApiClient mauSacApiClient,
                                IChatLieuApiClient chatLieuApiClient,
                                IThuongHieuApiClient thuongHieuApiClient,
                                IDeGiayApiClient deGiayApiClient,
                                IConfiguration configuration)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _danhMucApiClient = danhMucApiClient;
            _spctApiClient = spctApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _mauSacApiClient = mauSacApiClient;
            _chatLieuApiClient = chatLieuApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _deGiayApiClient = deGiayApiClient;
            _configuration = configuration;
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

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, bool trangThai)
        {
            try
            {
                var result = await _sanPhamApiClient.UpdateTrangThai(id, trangThai);
                if (result != null)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = false, message = "Cập nhật trạng thái thất bại" });
        }

        [HttpGet]
        public async Task<IActionResult> EditSPCT(Guid id)
        {
            var sanPham = await _sanPhamApiClient.GetById(id);
            if (sanPham == null) return NotFound();

            // Lấy danh sách các thuộc tính cho ViewBag
            var chatLieus = await _chatLieuApiClient.GetAll();
            var thuongHieus = await _thuongHieuApiClient.GetAll();
            var deGiays = await _deGiayApiClient.GetAll();
            var kichThuocs = await _kichThuocApiClient.GetAll();
            var mauSacs = await _mauSacApiClient.GetAll();
            var sanphams = await _sanPhamApiClient.GetAll();

            ViewBag.SanPhams = sanphams.ToDictionary(x => x.Id, x => x.TenSanPham);
            ViewBag.ChatLieus = chatLieus.ToDictionary(x => x.Id, x => x.TenChatLieu);
            ViewBag.ThuongHieus = thuongHieus.ToDictionary(x => x.Id, x => x.TenThuongHieu);
            ViewBag.DeGiays = deGiays.ToDictionary(x => x.Id, x => x.TenDeGiay);
            ViewBag.KichThuocs = kichThuocs.ToDictionary(x => x.Id, x => x.MaKichThuoc.ToString());
            ViewBag.MauSacs = mauSacs.ToDictionary(x => x.Id, x => x.TenMauSac);

            var danhSachSPCT = await _sanPhamApiClient.GetSPCTByProductName(sanPham.TenSanPham);

            var model = new SuaSPCT
            {
                SanPhamId = sanPham.Id,
                TenSanPham = sanPham.TenSanPham,
                DanhSachSPCT = danhSachSPCT
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSPCT([FromBody] List<SanPhamChiTietCapNhat> updates) // ở ngoài table list danh sách spct
        {
            try
            {
                var result = await _sanPhamApiClient.UpdateSPCT(updates);
                if (result)
                    return Json(new { success = true, message = "Cập nhật thành công" });
                return Json(new { success = false, message = "Cập nhật thất bại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSPCTDetail(Guid id)
        {
            var detail = await _sanPhamApiClient.GetSPCTDetail(id);
            var apiBaseUrl = _configuration["BackEndApiBaseUrl"];
            foreach (var img in detail.Images)
            {
                if (!string.IsNullOrEmpty(img.UrlHinhAnh) && !img.UrlHinhAnh.StartsWith("http"))
                {
                    img.UrlHinhAnh = apiBaseUrl.TrimEnd('/') + img.UrlHinhAnh;
                }
            }
            return Json(detail);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSPCTDetail([FromBody] SuaSPCTDetailViewModel model) // ở trong modal detail spct
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(); // mess báo lỗi
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var result = await _sanPhamApiClient.UpdateSPCTDetail(model);
            if (result)
                return Json(new { success = true });
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }

        [HttpGet("GetChatlieuForm")]
        public IActionResult GetChatlieuForm()
        {
            return PartialView("_AddChatLieuPartial");
        }

        [HttpGet("GetThuonghieuForm")]
        public IActionResult GetThuonghieuForm()
        {
            return PartialView("_AddThuongHieuPartial");
        }

        [HttpGet("GetDeGiayForm")]
        public IActionResult GetDeGiayForm()
        {
            return PartialView("_AddDeGiayPartial");
        }

        [HttpGet("GetKichthuocForm")]
        public IActionResult GetKichthuocForm()
        {
            return PartialView("_AddKichThuocPartial");
        }

        [HttpGet("GetMausacForm")]
        public IActionResult GetMausacForm()
        {
            return PartialView("_AddMauSacPartial");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(Guid sanPhamChiTietId, List<IFormFile> files)
        {
            var result = await _sanPhamApiClient.UploadImages(sanPhamChiTietId, files);
            if (result)
            {
                var spctDetail = await _sanPhamApiClient.GetSPCTDetail(sanPhamChiTietId);
                return Json(new { success = true, images = spctDetail.Images });
            }
            return Json(new { success = false, message = "Upload ảnh thất bại" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImage(Guid imageId, Guid sanPhamChiTietId)
        {
            var result = await _sanPhamApiClient.DeleteImage(imageId, sanPhamChiTietId);
            if (result)
            {
                var spctDetail = await _sanPhamApiClient.GetSPCTDetail(sanPhamChiTietId);
                return Json(new { success = true, images = spctDetail.Images });
            }
            return Json(new { success = false, message = "Xóa ảnh thất bại" });
        }

        [HttpPut]
        public async Task<IActionResult> SetDefaultImage(Guid imageId, Guid sanPhamChiTietId)
        {
            try
            {
                var result = await _sanPhamApiClient.SetDefaultImage(imageId, sanPhamChiTietId);
                if (result)
                {
                    // Lấy lại thông tin chi tiết SPCT để cập nhật danh sách ảnh
                    var spctDetail = await _sanPhamApiClient.GetSPCTDetail(sanPhamChiTietId);
                    return Json(new { success = true, images = spctDetail.Images });
                }
                return Json(new { success = false, message = "Set ảnh mặc định thất bại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}