using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using System.Threading.Tasks;

namespace SneakFit.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KhuyenMaiController : BaseController
    {
        private readonly IKhuyenMaiApiClient _khuyenMaiApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;

        public KhuyenMaiController(IKhuyenMaiApiClient khuyenMaiApiClient, ISanPhamApiClient sanPhamApiClient)
        {
            _khuyenMaiApiClient = khuyenMaiApiClient;
            _sanPhamApiClient = sanPhamApiClient;
        }

        public async Task<IActionResult> Index(string? keyword, TrangThaiGiamGia? trangThai, int pageIndex = 1, int pageSize = 10)
        {
            var request = new PhanTrangKhuyenMai()
            {
                Keyword = keyword,
                TrangThai = trangThai,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var data = await _khuyenMaiApiClient.GetAllPaging(request);

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.DanhSachTrangThai = new List<SelectListItem>()
            {
                new SelectListItem("Tất cả", "", !trangThai.HasValue),
                new SelectListItem("Không hoạt động", TrangThaiGiamGia.KhongHoatDong.ToString(), trangThai == TrangThaiGiamGia.KhongHoatDong),
                new SelectListItem("Đang hoạt động", TrangThaiGiamGia.HoatDong.ToString(), trangThai == TrangThaiGiamGia.HoatDong),
                new SelectListItem("Đã hết hạn", TrangThaiGiamGia.HetHan.ToString(), trangThai == TrangThaiGiamGia.HetHan)
            };

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var sanPhams = await _sanPhamApiClient.GetAll();
            ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham"); // Chuyển thành SelectList

            ViewBag.LoaiGiamGia = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemKhuyenMai request)
        {
            if (!ModelState.IsValid)
            {
                var sanPhams = await _sanPhamApiClient.GetAll();
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham");
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                    new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
                };
                return View(request);
            }
            try
            {
                var result = await _khuyenMaiApiClient.Create(request);
                TempData["SuccessMsg"] = "Tạo khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var sanPhams = await _sanPhamApiClient.GetAll();
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham");
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                    new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
                };
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _khuyenMaiApiClient.GetById(id);
            if (result == null)
                return NotFound();

            // Lấy danh sách các SPCT đã áp dụng khuyến mại (giả sử là các SPCT có SPCTId nằm trong SanPhamIds)
            var appliedSpctIds = result.SanPhams?.Select(p => p.SPCTId).ToList() ?? new List<Guid>();

            var selectedProductDetails = result.SanPhamChiTiets?
                .Where(x => appliedSpctIds.Contains(x.SPCTId)) // chỉ lấy các SPCT đã áp dụng khuyến mại
                .Select(x => new SPCTViewModels
                {
                    Id = x.SPCTId,
                    TenSanPham = result.SanPhams.FirstOrDefault(sp => sp.SanPhamId == x.SanPhamId)?.TenSanPham ?? "",
                    Gia = x.Gia,
                    SoLuong = x.SoLuong,
                    NgayTao = x.NgayTao,
                    MauSacId = x.MauSacId,
                    KichThuocId = x.KichThuocId,
                    ChatLieuId = x.ChatLieuId,
                    DeGiayId = x.DeGiayId,
                    ThuongHieuId = x.ThuongHieuId,
                    SanPhamId = x.SanPhamId,
                    TrangThai = x.TrangThai
                }).ToList() ?? new List<SPCTViewModels>();

            var request = new SuaKhuyenMai()
            {
                Id = result.Id,
                TenKhuyenMai = result.TenKhuyenMai,
                MoTa = result.MoTa,
                LoaiGiamGia = result.LoaiGiamGia,
                GiaTriGiamGia = result.GiaTriGiamGia,
                ThoiGianBatDau = result.ThoiGianBatDau,
                ThoiGianKetThuc = result.ThoiGianKetThuc,
                TrangThai = result.TrangThai,
                SanPhamIds = result.SanPhams?.Select(p => p.SPCTId).ToList() ?? new List<Guid>(),
                SelectedProductDetails = selectedProductDetails
            };

            var sanPhams = await _sanPhamApiClient.GetAll();
            Guid? selectedParentProductId = null;
            if (request.SelectedProductDetails != null && request.SelectedProductDetails.Any())
            {
                selectedParentProductId = request.SelectedProductDetails.FirstOrDefault()?.SanPhamId;
            }
            ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham", selectedParentProductId);

            ViewBag.LoaiGiamGia = new List<SelectListItem>
    {
        new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), result.LoaiGiamGia == LoaiGiamGia.PhamTram),
        new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), result.LoaiGiamGia == LoaiGiamGia.SoTien)
    };
            return View(request);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SuaKhuyenMai request)
        {
            
            if (!ModelState.IsValid)
            {
                // Thêm dòng này để xem lỗi gì
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                // Có thể log ra console hoặc gán vào ViewBag để hiển thị trên view
                ViewBag.ModelErrors = errors;
                var sanPhams = await _sanPhamApiClient.GetAll();
                if (request.SanPhamIds != null && request.SanPhamIds.Any())
                {
                    request.SelectedProductDetails = await _sanPhamApiClient.GetSPCTByListIds(request.SanPhamIds);
                }
                else
                {
                    request.SelectedProductDetails = new List<SPCTViewModels>();
                }

                // Lấy sản phẩm cha đã chọn (nếu có)
                Guid? selectedParentProductId = null;
                if (request.SelectedProductDetails != null && request.SelectedProductDetails.Any())
                {
                    selectedParentProductId = request.SelectedProductDetails.FirstOrDefault()?.SanPhamId;
                }
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham", selectedParentProductId);

                ViewBag.LoaiGiamGia = new List<SelectListItem>
                {
                    new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.PhamTram),
                    new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.SoTien)
                };
                return View(request);
            }

            try
            {
                var result = await _khuyenMaiApiClient.Update(request);
                TempData["SuccessMsg"] = "Cập nhật khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Lấy dòng đầu tiên của message (tránh stacktrace)
                string errorMsg = ex.Message.Split('\n').FirstOrDefault()?.Trim() ?? "Có lỗi xảy ra";
                ModelState.AddModelError("", errorMsg);

                var sanPhams = await _sanPhamApiClient.GetAll();
                if (request.SanPhamIds != null && request.SanPhamIds.Any())
                {
                    request.SelectedProductDetails = await _sanPhamApiClient.GetSPCTByListIds(request.SanPhamIds);
                }
                else
                {
                    request.SelectedProductDetails = new List<SPCTViewModels>();
                }
                Guid? selectedParentProductId = null;
                if (request.SelectedProductDetails != null && request.SelectedProductDetails.Any())
                {
                    selectedParentProductId = request.SelectedProductDetails.FirstOrDefault()?.SanPhamId;
                }
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham", selectedParentProductId);

                ViewBag.LoaiGiamGia = new List<SelectListItem>
    {
        new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.PhamTram),
        new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.SoTien)
    };
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _khuyenMaiApiClient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("Index");
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, TrangThaiGiamGia trangThai)
        {
            try
            {
                var result = await _khuyenMaiApiClient.UpdateStatus(id, trangThai);
                if (result)
                {
                    TempData["SuccessMsg"] = "Cập nhật trạng thái khuyến mãi thành công";
                }
                else
                {
                    TempData["ErrorMsg"] = "Cập nhật trạng thái khuyến mãi thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}