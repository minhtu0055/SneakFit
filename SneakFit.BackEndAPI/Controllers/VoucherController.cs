using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.VoucherRP;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.VoucherCATA;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // API tạo voucher mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVoucher request)
        {
            if (request == null) return BadRequest("Invalid data.");

            var result = await _voucherService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // API lấy tất cả các voucher phân trang
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetVoucherPagingRequest request)
        {
            var result = await _voucherService.GetAllPaging(request);
            return Ok(result);
        }

        // API lấy thông tin voucher theo mã
        [HttpGet("get-by-code/{code}")]
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
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _voucherService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // API cập nhật thông tin voucher
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateVoucher request)
        {
            if (request == null) return BadRequest("Invalid data.");

            var result = await _voucherService.Update(request);
            return Ok(result);
        }

        // API cập nhật trạng thái voucher
        [HttpPatch("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] TrangThaiGiamGia status)
        {
            var result = await _voucherService.UpdateTrangThai(id, status);
            if (result)
                return Ok(new { message = "Cập nhật trạng thái thành công" });
            return NotFound("Voucher không tồn tại");
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
