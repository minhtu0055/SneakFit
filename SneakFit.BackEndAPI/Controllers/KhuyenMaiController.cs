using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.KhuyenMai;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class KhuyenMaiController : ControllerBase
    {
        private readonly IKhuyenMaiService _khuyenMaiService;

        public KhuyenMaiController(IKhuyenMaiService khuyenMaiService)
        {
            _khuyenMaiService = khuyenMaiService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangKhuyenMai request)
        {
            var result = await _khuyenMaiService.GetAllPaging(request);
            return Ok(new ApiSuccessResult<PagedResult<KhuyenMaiViewModels>>(result));
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _khuyenMaiService.GetById(id);
            if (result == null)
                return NotFound(new ApiErrorResult<KhuyenMaiViewModels>($"Không tìm thấy khuyến mãi có ID: {id}"));
            return Ok(new ApiSuccessResult<KhuyenMaiViewModels>(result));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ThemKhuyenMai request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult<KhuyenMaiViewModels>());

            try
            {
                var result = await _khuyenMaiService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiSuccessResult<KhuyenMaiViewModels>(result));
            }
            catch (Exception ex)
            {
                // Trả về message lỗi ngắn gọn
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaKhuyenMai request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult<KhuyenMaiViewModels>());
            request.Id = id;
            try
            {
                var result = await _khuyenMaiService.Update(request);
                return Ok(new ApiSuccessResult<KhuyenMaiViewModels>(result));
            }
            catch (Exception ex)
            {
                // Trả về message lỗi ngắn gọn
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPatch("{id}/TrangThai")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TrangThaiGiamGia trangThai)
        {
            var result = await _khuyenMaiService.UpdateStatus(id, trangThai);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>("Cập nhật trạng thái không thành công"));

            return Ok(new ApiSuccessResult<bool>(true));
        }


    }
}
