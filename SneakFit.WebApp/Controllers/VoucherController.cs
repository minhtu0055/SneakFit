using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.Catalog.Voucher;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Core;
using SneakFit.Data.Entities;
using System.Net.NetworkInformation;
using SneakFit.ViewModels.System.User;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;


namespace SneakFit.Admin.Controllers
{
    public class VoucherController : BaseController
    {
        private readonly IVoucherApiClient _IvoucherCLient;
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public VoucherController(IVoucherApiClient ivoucherCLient, IUserApiClient userApiClient, IConfiguration configuration)
        {
            _IvoucherCLient = ivoucherCLient;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string Keyword, TrangThaiGiamGia? status = null, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetVoucherPagingRequest()
            {
                Keyword = Keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Status = status,
                SortBy = "NgayTao", // sắp xếp theo ngày tạo
                IsDescending = true // từ mới nhất đến cũ nhất
            };
            var data = await _IvoucherCLient.GetAllPaging(request);
            ViewBag.Keyword = Keyword;
            ViewBag.Status = status;
            ViewBag.DanhSachTrangThai = new List<SelectListItem>()
            {
                new SelectListItem("Tất cả", "", !status.HasValue),
                new SelectListItem("Chưa hoạt động", TrangThaiGiamGia.KhongHoatDong.ToString(), status == TrangThaiGiamGia.KhongHoatDong),
                new SelectListItem("Đang hoạt động", TrangThaiGiamGia.HoatDong.ToString(), status == TrangThaiGiamGia.HoatDong),
                new SelectListItem("Đã hết hạn", TrangThaiGiamGia.HetHan.ToString(), status == TrangThaiGiamGia.HetHan)
            };
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.LoaiVoucher = new SelectList(new[]
            {
                new { Id = (int)LoaiVoucher.CongKhai, Name = "Công khai" },
                new { Id = (int)LoaiVoucher.RiengTu, Name = "Riêng tư" }
            }, "Id", "Name");

            ViewBag.LoaiGiamGia = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };
            var getUserPagingRequest = new GetUserPagingRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Role = "KHÁCH HÀNG",
                TrangThai = true  // Chỉ lấy khách hàng đang hoạt động
            };
            // load danh sách khách hàng
            var rs = await _userApiClient.GetUsersPaging(getUserPagingRequest);
            var khachHangs = new PagedResult<UserViewModels>();

            if (rs.IsSuccessed)
            {
                khachHangs = rs.ResultObj;
            }
            ViewBag.KhachHangs = khachHangs;

            // 🆕 Tạo mã tự động: VC + số thứ tự
            var newCode = await _IvoucherCLient.GetNextVoucherCode();

            // Gán mã này vào model để hiển thị ở giao diện
            var model = new CreateVoucher
            {
                MaVoucher = newCode
            };

            return View(model);

            //return View(new CreateVoucher());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateVoucher request)
        {
            // ✅ Validate số lượng chỉ khi là voucher công khai
            if (request.LoaiVoucher == LoaiVoucher.CongKhai)
            {
                if (!request.SoLuong.HasValue || request.SoLuong <= 0)
                {
                    return Json(new { success = false, message = "Số lượng là bắt buộc và phải lớn hơn 0 đối với voucher công khai." });
                }
            }
            try
            {
                var result = await _IvoucherCLient.Create(request);
                return Json(new { success = true, message = "Tạo voucher thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _IvoucherCLient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy voucher.";
                return RedirectToAction("Index");
            }

            var editModel = new UpdateVoucher()
            {
                Id = result.Id,
                MaVoucher = result.MaVoucher,
                LoaiGiamGia = result.LoaiGiamGia,
                GiaTriGiamGia = result.GiaTriGiamGia,
                DieuKienApDung = result.DieuKienApDung,
                GiaTriToiDa = result.GiaTriToiDa,
                SoLuong = result.SoLuong,
                NgayTao = result.NgayTao,
                ThoiGianBatDau = result.ThoiGianBatDau,
                ThoiGianKetThuc = result.ThoiGianKetThuc,
                TrangThai = result.TrangThai,
                LoaiVoucher = result.loaiVoucher
            };
            // Gán danh sách khách hàng từ DB đã gắn với voucher
            if (result.loaiVoucher == LoaiVoucher.RiengTu)
            {
                var voucherUsers = await _IvoucherCLient.GetUsersForVoucher(id);
                if (voucherUsers != null)
                {
                    var userIdsFromDb = voucherUsers.Select(u => u.Id).ToList();
                    editModel.SelectedUserIds = userIdsFromDb;

                    // Truyền thêm danh sách để JS xử lý disable
                    ViewBag.ExistingUserIds = userIdsFromDb;
                }
            }

            ViewBag.LoaiGiamGia = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };

            // Load danh sách khách hàng
            var getUserPagingRequest = new GetUserPagingRequest
            {
                PageIndex = 1,
                PageSize = 10,
                Role = "KHÁCH HÀNG"
            };
            var rs = await _userApiClient.GetUsersPaging(getUserPagingRequest);
            var khachHangs = new PagedResult<UserViewModels>();
            if (rs.IsSuccessed)
            {
                khachHangs = rs.ResultObj;
            }
            ViewBag.KhachHangs = khachHangs;

            // Load danh sách khách hàng của voucher hiện tại
            if (result.loaiVoucher == LoaiVoucher.RiengTu)
            {
                var voucherUsers = await _IvoucherCLient.GetUsersForVoucher(id);
                if (voucherUsers != null)
                {
                    editModel.SelectedUserIds = voucherUsers.Select(u => u.Id).ToList();
                }
            }

            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateVoucher request)
        {
            request.SelectedUserIds = request.ParsedSelectedUserIds;

            ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.PhamTram),
                    new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.SoTien)
                };

            // Validate discount value based on discount type
            if (request.LoaiGiamGia == LoaiGiamGia.PhamTram && request.GiaTriGiamGia > 100)
            {
                ModelState.AddModelError("GiaTriGiamGia", "Giá trị giảm giá không được vượt quá 100%");
                return Json(new { success = false, message = "Giá trị giảm giá không được vượt quá 100%" });
            }

            try
            {
                var result = await _IvoucherCLient.Update(request);
                return Json(new { success = true, message = "Cập nhật voucher thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _IvoucherCLient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy voucher.";
                return RedirectToAction("Index");
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, TrangThaiGiamGia trangThai)
        {
            try
            {
                var result = await _IvoucherCLient.UpdateTrangThai(id, trangThai);
                if (result)
                {
                    TempData["SuccessMsg"] = "Cập nhật trạng thái voucher thành công";
                }
                else
                {
                    TempData["ErrorMsg"] = "Cập nhật trạng thái voucher thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersForVoucher()
        {
            try
            {
                var users = await _IvoucherCLient.GetUsersForVoucher();
                return Json(new ApiSuccessResult<List<VoucherUserViewModel>>(users));
            }
            catch (Exception ex)
            {
                return Json(new ApiErrorResult<List<VoucherUserViewModel>>(ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersForVoucherPaging([FromQuery] GetVoucherUserPagingRequest request)
        {
            try
            {
                var result = await _IvoucherCLient.GetUsersForVoucherPaging(request);
                return Json(new ApiSuccessResult<PagedResult<VoucherUserViewModel>>(result));
            }
            catch (Exception ex)
            {
                return Json(new ApiErrorResult<PagedResult<VoucherUserViewModel>>(ex.Message));
            }
        }
    }
}
