using Azure.Core;
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
    //[Authorize]
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
        public async Task<IActionResult> UseVoucher(string code, [FromBody] UseVoucherRequest request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest("Thiếu thông tin người dùng.");

            var result = await _voucherService.UseVoucher(code, request.UserId);
            if (result)
                return Ok(new { message = "Sử dụng voucher thành công" });

            return BadRequest("Voucher không hợp lệ, đã hết hạn, hoặc bạn không đủ điều kiện sử dụng.");
        }

        // API lấy danh sách khách hàng cho voucher
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersForVoucher([FromQuery] Guid? voucherId = null)
        {
            try
            {
                var result = await _voucherService.GetUsersForVoucher(voucherId);
                if (result == null || !result.Any())
                {
                    return NotFound(new ApiErrorResult<List<VoucherUserViewModel>>("Không tìm thấy khách hàng nào"));
                }
                return Ok(new ApiSuccessResult<List<VoucherUserViewModel>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<List<VoucherUserViewModel>>(ex.Message));
            }
        }

        // API lấy danh sách khách hàng cho voucher có phân trang
        [HttpGet("users/paging")]
        public async Task<IActionResult> GetUsersForVoucherPaging([FromQuery] GetVoucherUserPagingRequest request)
        {
                var result = await _voucherService.GetUsersForVoucherPaging(request);
                return Ok(new ApiSuccessResult<PagedResult<VoucherUserViewModel>>(result));
        }

        // API lấy danh sách voucher công khai đang hoạt động
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicVouchers([FromQuery] decimal tongTienHoaDon)
        {
            var result = await _voucherService.GetPublicVouchers(tongTienHoaDon);
            return Ok(new ApiSuccessResult<List<VoucherViewModels>>(result));
        }

        // API lấy danh sách voucher riêng tư đang hoạt động cho user
        [HttpGet("private/{userId}")]
        public async Task<IActionResult> GetPrivateVouchersForUser(Guid userId, [FromQuery] decimal tongTienHoaDon)
        {
            var privateVouchers = await _voucherService.GetVouchersForUser(userId, tongTienHoaDon);
            // Chỉ lấy voucher riêng tư
            privateVouchers = privateVouchers.Where(x => x.loaiVoucher == LoaiVoucher.RiengTu).ToList();
            return Ok(new ApiSuccessResult<List<VoucherViewModels>>(privateVouchers));
        }

        [HttpGet("getnextcode")]
        public async Task<IActionResult> GetNextCode()
        {
            try
            {
                var nextCode = await _voucherService.GetNextVoucherCode();
                return Ok(nextCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<string>(ex.Message));
            }
        }
    }
}
