using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.Voucher;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Common;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // API tạo voucher mới
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateVoucher request)
        {
            if (request == null) return BadRequest("Invalid data.");

            try
            {
                var result = await _voucherService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiSuccessResult<VoucherViewModels>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<VoucherViewModels>(ex.Message));
            }
        }

        // API lấy tất cả các voucher phân trang
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetVoucherPagingRequest request)
        {
            var result = await _voucherService.GetAllPaging(request);
            return Ok(new ApiSuccessResult<PagedResult<VoucherViewModels>>(result));
        }

        // API lấy thông tin voucher theo mã
        [HttpGet("getbycode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var result = await _voucherService.GetByCode(code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // API lấy thông tin voucher theo ID
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _voucherService.GetById(id);
            if (result == null)
                return NotFound(new ApiErrorResult<VoucherViewModels>($"Không tìm thấy Voucher có ID: {id}"));
            return Ok(new ApiSuccessResult<VoucherViewModels>(result));
        }

        // API cập nhật thông tin voucher
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Update(Guid Id, [FromBody] UpdateVoucher request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult<VoucherViewModels>());
            
            try
            {
                request.Id = Id;
                var result = await _voucherService.Update(request);
                if (result == null)
                    return NotFound(new ApiErrorResult<VoucherViewModels>($"Không tìm thấy Voucher có ID: {Id}"));
                return Ok(new ApiSuccessResult<VoucherViewModels>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<VoucherViewModels>(ex.Message));
            }
        }

        // API cập nhật trạng thái voucher
        [HttpPatch("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] TrangThaiGiamGia status)
        {
            var result = await _voucherService.UpdateTrangThai(id, status);
            if (result)
                return BadRequest(new ApiErrorResult<bool>("Cập nhật trạng thái không thành công"));
            return Ok(new ApiSuccessResult<bool>(true));
        }

        // API sử dụng voucher
        [HttpPost("use-voucher/{code}")]
        public async Task<IActionResult> UseVoucher(string code)
        {
            var result = await _voucherService.UseVoucher(code);
            if (result)
                return Ok(new { message = "Sử dụng voucher thành công" });
            return BadRequest("Voucher không hợp lệ hoặc đã hết hạn");
        }
    }
}
