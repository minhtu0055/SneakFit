using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.HoaDon;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly UserManager<AppUser> _userManager;
        public HoaDonController(IHoaDonService hoaDonService, UserManager<AppUser> userManager)
        {
            _hoaDonService = hoaDonService;
            _userManager = userManager;
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
            var user = await _userManager.GetUserAsync(User);
            var tenNguoiTao = user?.HoVaTen ?? User.Identity.Name;
            var createdHoaDon = await _hoaDonService.Create(request, tenNguoiTao);
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
        [HttpGet("count-by-status")]
        public async Task<IActionResult> GetCountByStatusAsync()
        {
            var result = await _hoaDonService.GetCountByStatusAsync();
            return Ok(result);
        }
       [HttpGet("cho-by-nguoitao")]
        public async Task<IActionResult> GetHoaDonChoByNguoiTao([FromQuery] string nguoiTao)
        {
            var result = await _hoaDonService.GetHoaDonChoByNguoiTao(nguoiTao);
            return Ok(result);
        }

        [HttpPost("thanhtoan")]
        public async Task<IActionResult> ThanhToan([FromBody] SuaHoaDon request)
        {
            // Khi thanh toán thành công, cập nhật trạng thái về 5 (ThanhCong)
            request.TrangThai = TrangThaiHoaDon.ThanhCong;
            request.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
            request.NgayThanhToan = DateTime.Now;

            var updated = await _hoaDonService.Update(request);
            if (updated == null)
                return NotFound(new { success = false, message = "Không tìm thấy hóa đơn" });

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _hoaDonService.Delete(id);
            if (!result)
                return NotFound();
            return Ok(new { success = true });
        }
    }
}
