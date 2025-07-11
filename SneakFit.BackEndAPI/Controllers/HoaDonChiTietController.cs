using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.HoaDon;
using SneakFit.Application.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonChiTietController : ControllerBase
    {
        private readonly IHoaDonChiTietService _hoaDonChiTietService;
        public HoaDonChiTietController(IHoaDonChiTietService hoaDonChiTietService)
        {
            _hoaDonChiTietService = hoaDonChiTietService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPaging([FromQuery] PhanTrangHoaDonChiTiet request)
        {
            var result = await _hoaDonChiTietService.GetAllPaging(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hoaDon = await _hoaDonChiTietService.GetById(id);
            if (hoaDon == null)
                return NotFound();
            return Ok(hoaDon);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] ThemHoaDonChiTiet request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var createdHoaDon = await _hoaDonChiTietService.Create(request);
        //    return CreatedAtAction(nameof(GetById), new { id = createdHoaDon.Id }, createdHoaDon);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaHoaDonChiTiet request)
        {
            if (id != request.Id)
                return BadRequest();

            var updatedHoaDon = await _hoaDonChiTietService.Edit(request);
            if (updatedHoaDon == null)
                return NotFound();
            return Ok(updatedHoaDon);
        }

        [HttpGet("GetByHoaDonId")]
        public async Task<IActionResult> GetByHoaDonId([FromQuery] Guid id)
        {
            var result = await _hoaDonChiTietService.GetById(id);
            return Ok(result);
        }

        [HttpPost("CreateOrUpdate")]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ThemHoaDonChiTiet request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _hoaDonChiTietService.CreateOrUpdate(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _hoaDonChiTietService.Delete(id);
            if (!result)
                return NotFound();
            return Ok(new { success = true, message = "Xóa hóa đơn chi tiết thành công" });
        }

        [HttpPut("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                var result = await _hoaDonChiTietService.UpdateQuantity(request.HoaDonChiTietId, request.NewQuantity);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
