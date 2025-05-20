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


namespace SneakFit.Admin.Controllers
{
    public class VoucherController : BaseController
    {
        private readonly IVoucherApiClient _IvoucherCLient;
        private readonly IConfiguration _configuration;

        public VoucherController(IVoucherApiClient ivoucherCLient, IConfiguration configuration)
        {
            _IvoucherCLient = ivoucherCLient;
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
            };
            var data = await _IvoucherCLient.GetAllPaging(request);
            ViewBag.Keyword = Keyword;
            ViewBag.Status = status;
            ViewBag.DanhSachTrangThai = new List<SelectListItem>()
            {
                new SelectListItem("Tất cả", "", !status.HasValue),
                new SelectListItem("Không hoạt động", TrangThaiGiamGia.KhongHoatDong.ToString(), status == TrangThaiGiamGia.KhongHoatDong),
                new SelectListItem("Đang hoạt động", TrangThaiGiamGia.HoatDong.ToString(), status == TrangThaiGiamGia.HoatDong),
                new SelectListItem("Đã hết hạn", TrangThaiGiamGia.HetHan.ToString(), status == TrangThaiGiamGia.HetHan)
            };
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.LoaiGiamGia = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateVoucher request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                    new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
                };
                return View();
            }

            // Validate discount value based on discount type
            if (request.LoaiGiamGia == LoaiGiamGia.PhamTram && request.GiaTriGiamGia > 100)
            {
                ModelState.AddModelError("GiaTriGiamGia", "Giá trị giảm giá không được vượt quá 100%");
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                    new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
                };
                return View(request);
            }

            try
            {
                var result = await _IvoucherCLient.Create(request);
                TempData["SuccessMsg"] = "Tạo voucher thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
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
            var result = await _IvoucherCLient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("Index");
            }

            var editModel = new UpdateVoucher()
            {
                Id = result.Id,
                MaVoucher = result.MaVoucher,
                LoaiGiamGia = result.LoaiGiamGia,
                GiaTriGiamGia = result.GiaTriGiamGia,
                DieuKienApDung = result.DieuKienApDung,
                SoLuong = result.SoLuong,
                NgayTao = result.NgayTao,
                ThoiGianBatDau = result.ThoiGianBatDau,
                ThoiGianKetThuc = result.ThoiGianKetThuc,
                TrangThai = result.TrangThai
            };

            ViewBag.LoaiGiamGia = new List<SelectListItem>()
            {
                new SelectListItem("Giảm theo phần trăm", ((int)LoaiGiamGia.PhamTram).ToString()),
                new SelectListItem("Giảm theo số tiền", ((int)LoaiGiamGia.SoTien).ToString())
            };

            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateVoucher request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.PhamTram),
                    new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.SoTien)
                };
                return View(request);
            }

            // Validate discount value based on discount type
            if (request.LoaiGiamGia == LoaiGiamGia.PhamTram && request.GiaTriGiamGia > 100)
            {
                ModelState.AddModelError("GiaTriGiamGia", "Giá trị giảm giá không được vượt quá 100%");
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
                {
                    new SelectListItem("Giảm theo phần trăm", LoaiGiamGia.PhamTram.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.PhamTram),
                    new SelectListItem("Giảm theo số tiền", LoaiGiamGia.SoTien.ToString("d"), request.LoaiGiamGia == LoaiGiamGia.SoTien)
                };
                return View(request);
            }

            try
            {
                var result = await _IvoucherCLient.Update(request);
                TempData["SuccessMsg"] = "Cập nhật khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.LoaiGiamGia = new List<SelectListItem>()
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
    }
}
