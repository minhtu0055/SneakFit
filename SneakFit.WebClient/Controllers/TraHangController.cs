using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System.Security.Claims;

namespace SneakFit.WebClient.Controllers
{
    public class TraHangController : Controller
    {
        private readonly ITraHangApiClient _traHangApiClient;

        public TraHangController(ITraHangApiClient traHangApiClient)
        {
            _traHangApiClient = traHangApiClient;
        }

        private Guid GetUserId()
        {
            var userIdStr = User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để tiếp tục.");
            }
            return Guid.Parse(userIdStr);
        }

        // Danh sách yêu cầu trả hàng của tôi
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                var result = await _traHangApiClient.GetMyReturnsAsync( pageIndex, pageSize);
                return View(result);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách yêu cầu: {ex.Message}";
                return View(new PagedResult<ReturnViewModel>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = 0,
                    Items = new List<ReturnViewModel>()
                });
            }
        }

        // Chi tiết 1 yêu cầu trả hàng
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _traHangApiClient.GetDetailAsync(id);
            if (!result.IsSuccessed || result.ResultObj == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Không tìm thấy yêu cầu trả hàng.";
                return RedirectToAction("Index");
            }
            return View(result.ResultObj);
        }

        // Gửi yêu cầu (POST từ modal tại trang Hóa đơn chi tiết)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReturnRequest request, List<IFormFile>? EvidenceImages)
        {
            var result = await _traHangApiClient.CreateAsync(request, EvidenceImages?.Count > 0 ? EvidenceImages : null);
            if (!result.IsSuccessed)
            {
                TempData["ErrorMessage"] = result.Message ?? "Gửi yêu cầu trả hàng thất bại.";
                return RedirectToAction("Details", "HoaDon", new { id = request.OrderId });
            }

            TempData["SuccessMessage"] = "Đã gửi yêu cầu trả hàng.";
            return RedirectToAction("Detail", new { id = result.ResultObj });
        }

        // Hủy yêu cầu (chỉ cho phép khi còn Chờ phê duyệt)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, Guid orderId)
        {
            var result = await _traHangApiClient.CancelAsync(id);
            TempData[result.IsSuccessed ? "SuccessMessage" : "ErrorMessage"] = result.Message ?? (result.IsSuccessed ? "Đã hủy yêu cầu." : "Hủy yêu cầu thất bại.");
            return RedirectToAction("Detail", new { id });
        }
    }
}
