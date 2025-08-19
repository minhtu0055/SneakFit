using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.TraHang;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnsController : ControllerBase
    {
        private readonly IReturnService _service;
        public ReturnsController(IReturnService service) { _service = service; }

        private Guid GetUserIdOrThrow()
        {
            var idStr = User?.FindFirstValue("UserId") ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idStr)) throw new UnauthorizedAccessException("Vui lòng đăng nhập.");
            return Guid.Parse(idStr);
        }

        [HttpPost] // multipart/form-data
        public async Task<IActionResult> Create([FromForm] CreateReturnRequest request)
        {
            var userId = GetUserIdOrThrow();
            var result = await _service.CreateAsync(request, userId);
            if (!result.IsSuccessed) return BadRequest(result);
            return Ok(result);
        }

        // NEW: Kiểm tra đơn hàng đã có yêu cầu trả hàng chưa (theo user đang đăng nhập)
        [HttpGet("has")]
        public async Task<IActionResult> Has([FromQuery] Guid orderId)
        {
            var userId = GetUserIdOrThrow();
            var has = await _service.HasReturnAsync(orderId, userId);
            return Ok(has);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserIdOrThrow();
            var result = await _service.GetMyAsync(userId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var userId = GetUserIdOrThrow();
            var vm = await _service.GetDetailAsync(id, userId);
            if (vm == null) return NotFound(new ApiErrorResult<ReturnViewModel>("Không tìm thấy yêu cầu"));
            return Ok(new ApiSuccessResult<ReturnViewModel>(vm));
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = GetUserIdOrThrow();
            var result = await _service.CancelAsync(id, userId);
            if (!result.IsSuccessed) return BadRequest(result);
            return Ok(result);
        }
    }
}