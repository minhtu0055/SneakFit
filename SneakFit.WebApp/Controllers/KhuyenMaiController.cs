using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using System.Threading.Tasks;

namespace SneakFit.Admin.Controllers
{
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
                // Lấy SanPhamIds từ kết quả API. result.SanPhams có thể là một danh sách các đối tượng, mỗi đối tượng có một SanPhamId.
                SanPhamIds = result.SanPhams?.Select(p => p.SanPhamId).ToList() ?? new List<Guid>()
            };

            // KHAI BÁO BIẾN sanPhams Ở ĐÂY ĐỂ CÓ THỂ SỬ DỤNG TRONG TOÀN BỘ PHƯƠNG THỨC
            var sanPhams = await _sanPhamApiClient.GetAll(); // GỌI API MỘT LẦN để lấy danh sách sản phẩm cha cho dropdown

            List<SPCTViewModels> selectedProductDetails = new List<SPCTViewModels>();
            if (request.SanPhamIds != null && request.SanPhamIds.Any())
            {
                // SỬ DỤNG PHƯƠNG THỨC ĐÃ CÓ SẴN TRONG SERVICE CỦA BẠN ĐỂ LẤY CHI TIẾT SP
                selectedProductDetails = await _sanPhamApiClient.GetSPCTByListIds(request.SanPhamIds);
            }
            request.SelectedProductDetails = selectedProductDetails; // Gán danh sách chi tiết sản phẩm đã chọn vào ViewModel

            // KHẮC PHỤC VẤN ĐỀ COMBOBOX "Tên sản phẩm"
            // Lấy ID sản phẩm cha của các SPCT đã chọn
            // Giả định: tất cả SPCT trong khuyến mãi thuộc về cùng một sản phẩm cha
            Guid? selectedParentProductId = null;
            if (request.SelectedProductDetails != null && request.SelectedProductDetails.Any())
            {
                selectedParentProductId = request.SelectedProductDetails.FirstOrDefault()?.SanPhamId;
            }

            // Cập nhật ViewBag.SanPhams với selectedValue để combobox tự động chọn
            ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham", selectedParentProductId);


            ViewBag.LoaiGiamGia = new List<SelectListItem>()
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
                var sanPhams = await _sanPhamApiClient.GetAll();
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham");
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
                ModelState.AddModelError("", ex.Message);
                var sanPhams = await _sanPhamApiClient.GetAll();
                ViewBag.SanPhams = new SelectList(sanPhams, "Id", "TenSanPham");
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