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
        public IActionResult CreateVNPay([FromBody] VNPayPaymentRequest request)
        {
            var url = _thanhToanService.CreateVNPayPaymentUrl(request);
            return Ok(new { paymentUrl = url });
        }

        [HttpPost("momo")]
        public async Task<IActionResult> CreateMomo([FromBody] MomoPaymentRequest request)
        {
            var url = await _thanhToanService.CreateMomoPaymentUrl(request);
            return Ok(new { paymentUrl = url });
        }
    }
}
