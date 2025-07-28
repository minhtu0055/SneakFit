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

        [HttpPost("vnpay-client")]
        public async Task<IActionResult> CreateVnPayClient([FromBody] VNPayPaymentRequest request)
        {
            try
            {
                var url = await _thanhToanService.CreateVnPayPaymentUrlClient(request);
                var response = new { paymentUrl = url };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("vnpay-callback-client")]
        public async Task<IActionResult> VnPayCallbackClient()
        {
            var vnp_Params = new Dictionary<string, string>();
            foreach (var key in Request.Query.Keys)
            {
                vnp_Params[key] = Request.Query[key];
            }
            
            var result = await _thanhToanService.XuLyVnPayCallBackClientAsync(vnp_Params);
            var orderId = vnp_Params.ContainsKey("vnp_TxnRef") ? vnp_Params["vnp_TxnRef"] : null;
            
            if (result && !string.IsNullOrEmpty(orderId))
            {
                // Thanh toán thành công
                return Redirect($"https://localhost:7211/ThanhToan/OrderConfirmation/{orderId}");
            }
            else if (!string.IsNullOrEmpty(orderId))
            {
                // Thanh toán thất bại nhưng vẫn có orderId - redirect về OrderConfirmation để hiển thị trạng thái đã hủy
                return Redirect($"https://localhost:7211/ThanhToan/OrderConfirmation/{orderId}");
            }
            else
            {
                // Không có orderId - redirect về trang chủ với thông báo lỗi
                return Redirect("https://localhost:7211/ThanhToan/OrderConfirmation?payment=fail");
            }
        }

        [HttpPost("momo")]
        public async Task<IActionResult> CreateMomo([FromBody] MomoPaymentRequest request)
        {
            var url = await _thanhToanService.CreateMomoPaymentUrl(request);
            return Ok(new { paymentUrl = url });
        }


        [HttpPost("momo-callback-client")]
        public async Task<IActionResult> MomoCallbackClient([FromBody] Dictionary<string, string> momoParams)
        {
            var result = await _thanhToanService.XuLyMomoCallBackClientAsync(momoParams);
            var orderId = momoParams.ContainsKey("orderId") ? momoParams["orderId"] : null;
            
            if (result && !string.IsNullOrEmpty(orderId))
            {
                // Thanh toán thành công
                return Redirect($"https://localhost:7211/ThanhToan/OrderConfirmation/{orderId}");
            }
            else if (!string.IsNullOrEmpty(orderId))
            {
                // Thanh toán thất bại nhưng vẫn có orderId - redirect về OrderConfirmation để hiển thị trạng thái đã hủy
                return Redirect($"https://localhost:7211/ThanhToan/OrderConfirmation/{orderId}");
            }
            else
            {
                // Không có orderId - redirect về trang chủ với thông báo lỗi
                return Redirect("https://localhost:7211/ThanhToan/OrderConfirmation?payment=fail");
            }
        }     
    }
}
