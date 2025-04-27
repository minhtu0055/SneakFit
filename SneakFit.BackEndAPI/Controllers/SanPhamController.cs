using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.ThuongHieu;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _sanPhamService.GetAll();
            return Ok(list);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _sanPhamService.GetById(id);
            if (item == null)
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");
            return Ok(item);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ThemSanPham request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanPham = await _sanPhamService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = sanPham.Id }, sanPham);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] SuaSanPham request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var sanPham = await _sanPhamService.Update(request);
            if (sanPham == null)
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");

            return Ok(sanPham);
        }
    }
}
