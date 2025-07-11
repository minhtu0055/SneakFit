using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using System.Threading.Tasks;

namespace SneakFit.Admin.Controllers
{
    public class HoaDonChiTietController : Controller
    {
        private readonly IHoaDonChiTietApiClient _hoaDonChiTietApiClient;
        public HoaDonChiTietController(IHoaDonChiTietApiClient hoaDonChiTietApiClient)
        {
            _hoaDonChiTietApiClient = hoaDonChiTietApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemHoaDonChiTiet request)
        {
            try
            {
                var result = await _hoaDonChiTietApiClient.Create(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetByHoaDonId(Guid id)
        {
            try
            {
                var result = await _hoaDonChiTietApiClient.GetByHoaDonId(id);
                return Json(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ThemHoaDonChiTiet request)
        {
            try
            {
                var result = await _hoaDonChiTietApiClient.CreateOrUpdate(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _hoaDonChiTietApiClient.Delete(id);
                return Ok(new { success = true, message = "Xóa sản phẩm khỏi hóa đơn thành công" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                var result = await _hoaDonChiTietApiClient.UpdateQuantity(request.HoaDonChiTietId, request.NewQuantity);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
