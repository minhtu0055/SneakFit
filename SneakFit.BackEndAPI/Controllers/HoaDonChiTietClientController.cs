using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.HoaDon;
using SneakFit.Application.Catalog.HoaDonChiTietClients;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonChiTietClientController : ControllerBase
    {
        private readonly IHoaDonChiTietClientService _HoaDonChiTietClientService;

        public HoaDonChiTietClientController(IHoaDonChiTietClientService HoaDonChiTietClientService)
        {
            _HoaDonChiTietClientService = HoaDonChiTietClientService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangHoaDonChiTietClient request)
        {
            var result = await _HoaDonChiTietClientService.GetAllPaging(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hoaDon = await _HoaDonChiTietClientService.GetById(id);
            if (hoaDon == null)
                return NotFound();
            return Ok(hoaDon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemHoaDonChiTietClient request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdHoaDon = await _HoaDonChiTietClientService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = createdHoaDon.Id }, createdHoaDon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaHoaDonChiTietClient request)
        {
            if (id != request.Id)
                return BadRequest();

            var updatedHoaDon = await _HoaDonChiTietClientService.Edit(request);
            if (updatedHoaDon == null)
                return NotFound();
            return Ok(updatedHoaDon);
        }
    }
}
