using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.HoaDon;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangHoaDon request)
        {
            var result = await _hoaDonService.GetAllPaging(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hoaDon = await _hoaDonService.GetById(id);
            if (hoaDon == null)
                return NotFound();
            return Ok(hoaDon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemHoaDon request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdHoaDon = await _hoaDonService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = createdHoaDon.Id }, createdHoaDon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaHoaDon request)
        {
            if (id != request.Id)
                return BadRequest();

            var updatedHoaDon = await _hoaDonService.Update(request);
            if (updatedHoaDon == null)
                return NotFound();
            return Ok(updatedHoaDon);
        }

        [HttpPut("{id}/status/{trangThai}")]
        public async Task<IActionResult> UpdateStatus(Guid id, TrangThaiHoaDon trangThai)
        {
            var result = await _hoaDonService.UpdateStatus(id, trangThai);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
