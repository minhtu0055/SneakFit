using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

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

        public SanPhamController(ISanPhamApiClient sanPhamApiClient, 
                                IDanhMucApiClient danhMucApiClient,
                                ISpctApiClient spctApiClient,
                                IKichThuocApiClient kichThuocApiClient,
                                IMauSacApiClient mauSacApiClient,
                                IChatLieuApiClient chatLieuApiClient,
                                IThuongHieuApiClient thuongHieuApiClient,
                                IDeGiayApiClient deGiayApiClient)
        {
            _sanPhamApiClient = sanPhamApiClient;
            _danhMucApiClient = danhMucApiClient;
            _spctApiClient = spctApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _mauSacApiClient = mauSacApiClient;
            _chatLieuApiClient = chatLieuApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _deGiayApiClient = deGiayApiClient;
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
        public async Task<IActionResult> UpdateSPCT(Guid id, [FromBody] List<SanPhamChiTietCapNhat> updates)
        {
            try
            {
                var result = await _sanPhamApiClient.UpdateSPCT(id, updates);
                return Json(new { success = true, message = "Cập nhật thành công" });
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
            return Json(detail);
        }
    }
}