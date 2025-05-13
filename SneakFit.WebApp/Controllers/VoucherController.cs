using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;


namespace SneakFit.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : BaseController
    {
        private readonly IVoucherApiClient _IvoucherCLient;
        private readonly IConfiguration _configuration;

        public VoucherController(IVoucherApiClient ivoucherCLient, IConfiguration configuration)
        {
            _IvoucherCLient = ivoucherCLient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string Keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetVoucherPagingRequest()
            {
                Keyword = Keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
            var data = await _IvoucherCLient.GetAllPaging(request);
            ViewBag.Keyword = Keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Create(CreateVoucher request)
        {
            if (!ModelState.IsValid)
            {
                return View();
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
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> update(Guid id)
        {
            var result = await _IvoucherCLient.GetById(id);
            if (result == null)
            {
                TempData["ErrorMsg"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("Index");
            }

            var editModel = new UpdateVoucher()
            {
                Id = Guid.NewGuid(),
                MaVoucher = result.MaVoucher,
                LoaiGiamGia = result.LoaiGiamGia,
                GiaTriGiamGia = result.GiaTriGiamGia,
                DieuKienApDung = result.DieuKienApDung,
                SoLuong = result.SoLuong,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = result.ThoiGianBatDau,
                ThoiGianKetThuc = result.ThoiGianKetThuc,
                TrangThai = DateTime.Now >= result.ThoiGianBatDau ? TrangThaiGiamGia.HoatDong : TrangThaiGiamGia.HetHan,
            };
            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateVoucher result)
        {
            try
            {
                var vcup = await _IvoucherCLient.Update(result);
                TempData["SuccessMsg"] = "Cập nhật khuyến mãi thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(result);
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
