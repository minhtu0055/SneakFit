using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;

namespace SneakFit.Admin.Controllers
{
    public class ReturnsController : Controller
    {
        private readonly IAdminReturnsApiClient _api;
        public ReturnsController(IAdminReturnsApiClient api) => _api = api;

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10,
            ReturnStatus? status = null, string? keyword = null, DateTime? from = null, DateTime? to = null)
        {
            var result = await _api.GetPagingAsync(pageIndex, pageSize,
                status.HasValue ? (int?)status.Value : null, keyword, from, to);
            ViewBag.Status = status; ViewBag.Keyword = keyword; ViewBag.From = from; ViewBag.To = to;
            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await _api.GetDetailAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu.";
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, string? carrier, string? shipCode)
        {
            await _api.ApproveAsync(id, carrier, shipCode);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(Guid id)
        {
            await _api.ReceiveAsync(id);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _api.CompleteAsync(id);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            await _api.RejectAsync(id, reason);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusWithLog(Guid id, int newStatus, string ghiChu, string nguoiChinhSua)
        {
            try
            {
                var result = await _api.UpdateStatusWithLogAsync(id, newStatus, ghiChu, nguoiChinhSua);
                if (result)
                    return Ok(new { success = true });
                return BadRequest(new { success = false, message = "Không thể cập nhật trạng thái!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusHistory(Guid id)
        {
            try
            {
                var history = await _api.GetStatusHistoryAsync(id);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
