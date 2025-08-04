using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.DeGiay;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SneakFit.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SPCTController : BaseController
    {
        private readonly ISpctApiClient _spctApiClient;
        private readonly IConfiguration _configuration;
        private readonly IMauSacApiClient _mauSacApiClient;
        private readonly IKichThuocApiClient _kichThuocApiClient;
        private readonly IChatLieuApiClient _chatLieuApiClient;
        private readonly IDeGiayApiClient _deGiayApiClient;
        private readonly IThuongHieuApiClient _thuongHieuApiClient;
        private readonly ISanPhamApiClient _sanPhamApiClient;
        private readonly ILogger<SPCTController> _logger;

        public SPCTController(
            ISpctApiClient spctApiClient,
            IConfiguration configuration,
            IMauSacApiClient mauSacApiClient,
            IKichThuocApiClient kichThuocApiClient,
            IChatLieuApiClient chatLieuApiClient,
            IDeGiayApiClient deGiayApiClient,
            IThuongHieuApiClient thuongHieuApiClient,
            ISanPhamApiClient sanPhamApiClient,
            ILogger<SPCTController> logger)
        {
            _spctApiClient = spctApiClient;
            _configuration = configuration;
            _mauSacApiClient = mauSacApiClient;
            _kichThuocApiClient = kichThuocApiClient;
            _chatLieuApiClient = chatLieuApiClient;
            _deGiayApiClient = deGiayApiClient;
            _thuongHieuApiClient = thuongHieuApiClient;
            _sanPhamApiClient = sanPhamApiClient;
            _logger = logger;
        }

        // Hiển thị danh sách sản phẩm chi tiết với phân trang và tìm kiếm
        public async Task<IActionResult> Index(string tuKhoa, int pageIndex = 1, int pageSize = 10)
        {
            var request = new PhanTrangSPCT()
            {
                TuKhoa = tuKhoa,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _spctApiClient.GetAllPaging(request);
            ViewBag.TuKhoa = tuKhoa;

            // Load all data cần thiết song song
            var mauSacs = await _mauSacApiClient.GetAll();
            var kichThuocs = await _kichThuocApiClient.GetAll();
            var chatLieus = await _chatLieuApiClient.GetAll();
            var deGiays = await _deGiayApiClient.GetAll();
            var thuongHieus = await _thuongHieuApiClient.GetAll();
            var sanPhams = await _sanPhamApiClient.GetAll();

            // Sử dụng Dictionary để truy xuất nhanh hơn vì mỗi lần truy xuất sẽ không phải gọi lại API
            // 1 Dictionary gồm 1 key và 1 value
            ViewBag.MauSacs = mauSacs.ToDictionary(x => x.Id, x => x.TenMauSac);
            ViewBag.KichThuocs = kichThuocs.ToDictionary(x => x.Id, x => x.MaKichThuoc.ToString());
            ViewBag.ChatLieus = chatLieus.ToDictionary(x => x.Id, x => x.TenChatLieu);
            ViewBag.DeGiays = deGiays.ToDictionary(x => x.Id, x => x.TenDeGiay);
            ViewBag.ThuongHieus = thuongHieus.ToDictionary(x => x.Id, x => x.TenThuongHieu);
            ViewBag.SanPhams = sanPhams.ToDictionary(x => x.Id, x => x.TenSanPham);

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        private async Task LoadCombobox()
        {
            var mauSacs = await _mauSacApiClient.GetAll();
            var kichThuocs = await _kichThuocApiClient.GetAll();
            var chatLieus = await _chatLieuApiClient.GetAll();
            var deGiays = await _deGiayApiClient.GetAll();
            var thuongHieus = await _thuongHieuApiClient.GetAll();
            var sanPhams = await _sanPhamApiClient.GetAll();

            ViewBag.MauSacs = mauSacs.Select(x => new SelectListItem()
            {
                Text = x.TenMauSac,
                Value = x.Id.ToString()
            });
            ViewBag.KichThuocs = kichThuocs.Select(x => new SelectListItem()
            {
                Text = x.MaKichThuoc.ToString(),
                Value = x.Id.ToString()
            });
            ViewBag.ChatLieus = chatLieus.Select(x => new SelectListItem()
            {
                Text = x.TenChatLieu,
                Value = x.Id.ToString()
            });
            ViewBag.DeGiays = deGiays.Select(x => new SelectListItem()
            {
                Text = x.TenDeGiay,
                Value = x.Id.ToString()
            });
            ViewBag.ThuongHieus = thuongHieus.Select(x => new SelectListItem()
            {
                Text = x.TenThuongHieu,
                Value = x.Id.ToString()
            });
            ViewBag.SanPhams = sanPhams.Select(x => new SelectListItem()
            {
                Text = x.TenSanPham,
                Value = x.Id.ToString()
            });
        }

        // Hiển thị form thêm mới sản phẩm chi tiết
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try 
            {
                await LoadCombobox();
                return PartialView("Create");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi load form thêm mới");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải form" });
            }
        }

        // Xử lý thêm mới sản phẩm chi tiết
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemSPCT request)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombobox();
                return View(request);
            }
            // Gọi qua ApiClient, không xử lý ảnh ở đây
            var result = await _spctApiClient.Create(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Thêm mới sản phẩm chi tiết thành công";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message ?? "Thêm sản phẩm chi tiết thất bại");
                await LoadCombobox();
                return View(request);
            }
        }

        // Xử lý thêm mới nhiều sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> CreateMultiple([FromBody] List<SPCTViewModels> items)
        {
            if (items == null || !items.Any())
                return Json(new { success = false, message = "Dữ liệu không hợp lệ hoặc chưa chọn màu/kích thước!" });

            var errorList = new List<string>();
            var createdItems = new List<SPCTViewModels>(); // Lưu danh sách sản phẩm đã tạo
            int resultCount = 0; // Đếm số sản phẩm chi tiết tạo thành công
            int duplicateCount = 0; // Đếm số sản phẩm chi tiết bị trùng lặp

            foreach (var item in items)
            {
                var request = new ThemSPCT
                {
                    SanPhamId = item.SanPhamId,
                    ThuongHieuId = item.ThuongHieuId,
                    ChatLieuId = item.ChatLieuId,
                    DeGiayId = item.DeGiayId,
                    MauSacId = item.MauSacId,
                    KichThuocId = item.KichThuocId,
                    SoLuong = item.SoLuong,
                    Gia = item.Gia,
                    TrangThai = item.TrangThai
                };

                var result = await _spctApiClient.Create(request);
                if (result.IsSuccessed)
                {
                    resultCount++;
                    createdItems.Add(result.ResultObj);
                }
                else
                {
                    // Kiểm tra nếu lỗi là do trùng lặp
                    if (result.Message.ToLower().Contains("tồn tại") || result.Message.ToLower().Contains("already exists"))
                    {
                        duplicateCount++;
                    }
                    else
                    {
                        errorList.Add(result.Message); // Giữ lỗi khác (không phải trùng lặp)
                    }
                }
            }

            // Xử lý thông báo
            if (resultCount > 0)
            {
                string message = $"Thêm mới {resultCount} sản phẩm chi tiết thành công.";
                if (duplicateCount > 0)
                {
                    message += $"\n {duplicateCount} sản phẩm chi tiết đã tồn tại.";
                }
                if (errorList.Count > 0)
                {
                    message += $"\n {string.Join("; ", errorList)}";
                }
                return Json(new { success = true, message = message, data = createdItems, duplicateCount = duplicateCount });
            }
            else
            {
                if (duplicateCount > 0)
                {
                    return Json(new { success = false, message = $"{duplicateCount} sản phẩm chi tiết đã tồn tại." });
                }
                else if (errorList.Count > 0)
                {
                    return Json(new { success = false, message = string.Join("; ", errorList) });
                }
                return Json(new { success = false, message = "Không thể thêm sản phẩm chi tiết!" });
            }
        }

        // Hiển thị form chỉnh sửa sản phẩm chi tiết
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var spct = await _spctApiClient.GetById(id);
            var danhSachSPCT = await _spctApiClient.GetAll(); // hoặc GetAllPaging nếu nhiều
            var viewModel = new SuaSPCT
            {
                Id = spct.Id,
                Gia = spct.Gia,
                SoLuong = spct.SoLuong,
                MauSacId = spct.MauSacId,
                KichThuocId = spct.KichThuocId,
                ChatLieuId = spct.ChatLieuId,
                DeGiayId = spct.DeGiayId,
                ThuongHieuId = spct.ThuongHieuId,
                SanPhamId = spct.SanPhamId,
                DanhMucId = spct.DanhMucId,
                TrangThai = spct.TrangThai,
                DanhSachSPCT = danhSachSPCT
            };
            await LoadCombobox();
            return View(viewModel);
        }

        // Xử lý cập nhật sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> Edit(SuaSPCT request)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombobox();
                return View(request);
            }
            var result = await _spctApiClient.Update(request);
            if (result != null)
            {
                TempData["result"] = "Cập nhật sản phẩm chi tiết thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Cập nhật sản phẩm chi tiết thất bại");
            await LoadCombobox();
            return View(request);
        }

        // Cập nhật trạng thái sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, bool trangThai)
        {
            try
            {
                var result = await _spctApiClient.UpdateTrangThai(id, trangThai);
                if (result)
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

        // Cập nhật giá sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> CapNhatGia(Guid id, decimal giaMoi)
        {
            try
            {
                var result = await _spctApiClient.UpdateGia(id, giaMoi);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật giá thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giá");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật giá" });
            }
        }

        // Cập nhật số lượng sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(Guid id, int soLuongThem)
        {
            try
            {
                var result = await _spctApiClient.UpdateSoLuong(id, soLuongThem);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật số lượng thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật số lượng");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
            }
        }

        // Upload ảnh cho sản phẩm chi tiết
        [HttpPost]
        public async Task<IActionResult> UploadImages(Guid sanPhamChiTietId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
                return Json(new { success = false, message = "Không có file để upload" });

            // Kiểm tra số lượng file
            if (files.Count > 3)
                return Json(new { success = false, message = "Chỉ được upload tối đa 3 ảnh" });

            int successCount = 0;
            var errors = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    int result = await _spctApiClient.AddImage(sanPhamChiTietId, file);
                    if (result > 0)
                        successCount++;
                    else
                        errors.Add($"Không thể upload file {file.FileName}");
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi upload file {file.FileName}: {ex.Message}");
                }
            }

            if (successCount > 0)
            {
                string message = $"Upload thành công {successCount}/{files.Count} file.";
                if (errors.Any())
                    message += $"\nLỗi: {string.Join("; ", errors)}";
                return Json(new { success = true, message = message });
            }
            return Json(new { success = false, message = $"Upload thất bại: {string.Join("; ", errors)}" });
        }

        // Action để lấy dữ liệu SPCT cho modal bán hàng
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSPCTForModal(string tuKhoa = "",Guid? kichThuocId = null, Guid? mauSacId = null, Guid? danhMucId = null, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var request = new PhanTrangSPCT()
                {
                    TuKhoa = tuKhoa,
                    MauSacId = mauSacId,
                    KichThuocId = kichThuocId,
                    DanhMucId = danhMucId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                
                var data = await _spctApiClient.GetAllPagings(request);
                
                // Load các dữ liệu cần thiết để map tên
                var mauSacs = await _mauSacApiClient.GetAll();
                var kichThuocs = await _kichThuocApiClient.GetAll();
                var sanPhams = await _sanPhamApiClient.GetAll();
                
                var mauSacsDict = mauSacs.ToDictionary(x => x.Id, x => x.TenMauSac);
                var kichThuocsDict = kichThuocs.ToDictionary(x => x.Id, x => x.MaKichThuoc.ToString());
                var sanPhamsDict = sanPhams.ToDictionary(x => x.Id, x => x.TenSanPham);
                
                // Map thêm thông tin tên cho mỗi sản phẩm
                var result = new
                {
                    Items = data.Items.Select(sp => new
                    {
                        sp.Id,
                        sp.SanPhamId,
                        sp.MauSacId,
                        sp.KichThuocId,
                        sp.DanhMucId,
                        sp.SoLuong,
                        sp.Gia,
                        sp.TrangThai,
                        sp.GiaKhuyenMai,
                        sp.KhuyenMaiId,
                        TenSanPham = sanPhamsDict.ContainsKey(sp.SanPhamId) ? sanPhamsDict[sp.SanPhamId] : "N/A",
                        TenMauSac = mauSacsDict.ContainsKey(sp.MauSacId) ? mauSacsDict[sp.MauSacId] : "N/A",
                        TenKichThuoc = kichThuocsDict.ContainsKey(sp.KichThuocId) ? kichThuocsDict[sp.KichThuocId] : "N/A",
                        Images = sp.Images ?? new List<string>()
                    }),
                    PageIndex = data.PageIndex,
                    PageSize = data.PageSize,
                    TotalRecords = data.TotalRecords,
                    PageCount = data.PageCount
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu SPCT cho modal");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải dữ liệu" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var spct = await _spctApiClient.GetById(id);
            if (spct == null)
                return Json(null);
            return Json(spct);
        }
        
    }
}
