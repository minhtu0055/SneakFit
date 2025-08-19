using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.TraHang;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.TraHang;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/admin/returns")]
    [ApiController]
    public class AdminReturnsController : ControllerBase
    {
        private readonly IReturnService _service;
        public AdminReturnsController(IReturnService service)
        {
            _service = service;
        }

        // Chuyển CHỜ PHÊ DUYỆT -> ĐANG VẬN CHUYỂN VỀ, và set thông tin vận chuyển (nếu có)
        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromQuery] string? carrier, [FromQuery] string? shipCode)
        {
            var r = await _service.ApproveAsync(id, carrier, shipCode);
            return r.IsSuccessed ? Ok(r) : BadRequest(r);
        }

        // Chuyển ĐANG VẬN CHUYỂN VỀ -> ĐÃ NHẬN HOÀN HÀNG
        [HttpPut("{id:guid}/receive")]
        public async Task<IActionResult> Receive(Guid id)
        {
            var r = await _service.ReceiveAsync(id);
            return r.IsSuccessed ? Ok(r) : BadRequest(r);
        }

        // Chuyển ĐÃ NHẬN HOÀN HÀNG -> ĐÃ HOÀN HÀNG THÀNH CÔNG
        // Đồng thời cộng kho + set trạng thái thanh toán/đơn hàng trong service
        [HttpPut("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var r = await _service.CompleteAsync(id);
            return r.IsSuccessed ? Ok(r) : BadRequest(r);
        }

        // Từ CHỜ PHÊ DUYỆT -> TỪ CHỐI
        [HttpPut("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromQuery] string reason)
        {
            var r = await _service.RejectAsync(id, reason);
            return r.IsSuccessed ? Ok(r) : BadRequest(r);
        }

        // Chuyển đổi trạng thái với ghi chú
        [HttpPost("{id:guid}/update-status")]
        public async Task<IActionResult> UpdateStatusWithLog(Guid id, [FromBody] UpdateReturnStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.GhiChu))
                return BadRequest("Ghi chú bắt buộc!");
            
            var r = await _service.UpdateStatusWithLogAsync(id, request.NewStatus, request.GhiChu, request.NguoiChinhSua);
            return r.IsSuccessed ? Ok(r) : BadRequest(r);
        }

        // Lấy lịch sử thay đổi trạng thái
        [HttpGet("{id:guid}/history")]
        public async Task<IActionResult> GetStatusHistory(Guid id)
        {
            var history = await _service.GetStatusHistoryAsync(id);
            return Ok(history);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10,
                                                [FromQuery] ReturnStatus? status = null,
                                                [FromQuery] string? keyword = null,
                                                [FromQuery] DateTime? from = null,
                                                [FromQuery] DateTime? to = null)
        {
            var r = await _service.GetAdminPagingAsync(pageIndex, pageSize, status, keyword, from, to);
            return Ok(r);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var vm = await _service.GetAdminDetailAsync(id);
            if (vm == null) return NotFound();
            return Ok(vm);
        }
    }
}
