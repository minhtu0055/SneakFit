using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.HoaDonClient;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonClientController : ControllerBase
    {
        private readonly IHoaDonClientService _HoaDonClientService;

        public HoaDonClientController(IHoaDonClientService HoaDonClientService)
        {
            _HoaDonClientService = HoaDonClientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangHoaDonClient request, [FromQuery] Guid? userId = null)
        {
            var result = await _HoaDonClientService.GetAllPaging(request, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var HoaDonClient = await _HoaDonClientService.GetById(id);
            if (HoaDonClient == null)
                return NotFound();
            return Ok(HoaDonClient);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemHoaDonClient request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdHoaDonClient = await _HoaDonClientService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = createdHoaDonClient.Id }, createdHoaDonClient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaHoaDonClient request)
        {
            if (id != request.Id)
                return BadRequest();

            var updatedHoaDonClient = await _HoaDonClientService.Update(request);
            if (updatedHoaDonClient == null)
                return NotFound();
            return Ok(updatedHoaDonClient);
        }
        [HttpGet("count-by-status")]
        public async Task<IActionResult> GetCountByStatusAsync()
        {
            var result = await _HoaDonClientService.GetCountByStatusAsync();
            return Ok(result);
        }

        [HttpPatch("{id}/trangthai")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] SneakFit.Data.Enums.TrangThaiHoaDon newStatus)
        {
            var hoaDon = await _HoaDonClientService.GetById(id);
            if (hoaDon == null)
                return NotFound();

            var result = await _HoaDonClientService.UpdateStatus(id, newStatus);
            if (!result)
                return BadRequest();

            return Ok(new { success = true });
        }

        [HttpPatch("{id}/trangthai-thanhtoan")]
        public async Task<IActionResult> UpdateTrangThaiThanhToan(Guid id, [FromBody] SneakFit.Data.Enums.TrangThaiThanhToan newPaymentStatus)
        {
            var hoaDon = await _HoaDonClientService.GetById(id);
            if (hoaDon == null)
                return NotFound();

            var result = await _HoaDonClientService.UpdatePaymentStatus(id, newPaymentStatus);
            if (!result)
                return BadRequest();

            return Ok(new { success = true });
        }
    }
}
