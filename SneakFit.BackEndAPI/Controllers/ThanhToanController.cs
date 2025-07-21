using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.ThanhToan;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IThanhToanService _thanhToanService;
        public ThanhToanController(IThanhToanService thanhToanService)
        {
            _thanhToanService = thanhToanService;
        }

        [HttpPost("momo")]
        public async Task<IActionResult> CreateMomo([FromBody] MomoPaymentRequest request)
        {
            var url = await _thanhToanService.CreateMomoPaymentUrl(request);
            return Ok(new { paymentUrl = url });
        }
        [HttpPost("vnpay")]
        public async Task<IActionResult> CreateVnPay([FromBody] VNPayPaymentRequest request)
        {
            // Cập nhật hóa đơn sang trạng thái chờ thanh toán trước khi tạo link VNPay
            // (Chỉ cập nhật các trường cần thiết, không để client tự cập nhật)
            // Có thể gọi service hóa đơn ở đây nếu cần, hoặc chuyển logic này vào ThanhToanService
            // Ví dụ:
            // await _hoaDonService.UpdateTrangThaiChoThanhToan(request.OrderId);

            var url = await _thanhToanService.CreateVnPayPaymentUrl(request);
            return Ok(new { paymentUrl = url });
        }
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var vnp_Params = new Dictionary<string, string>();
            foreach (var key in Request.Query.Keys)
            {
                vnp_Params[key] = Request.Query[key];
            }
            var result = await _thanhToanService.XuLyVnPayCallbackAsync(vnp_Params);
            if (result)
            {
                return Redirect("https://localhost:7039/BanHang/Index?payment=success");
            }
            else
            {
                return Redirect("https://localhost:7039/BanHang/Index?payment=fail");
            }
        }
        
    }
}
