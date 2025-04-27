using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SPCTController : ControllerBase
    {
        private readonly ISanPhamChiTetService _sanPhamChiTetService;

        public SPCTController(ISanPhamChiTetService sanPhamChiTetService)
        {
            _sanPhamChiTetService = sanPhamChiTetService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _sanPhamChiTetService.GetAll();
            return Ok(list);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _sanPhamChiTetService.GetById(id);
            if (item == null)
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");
            return Ok(item);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ThemSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanPham = await _sanPhamChiTetService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = sanPham.Id }, sanPham);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] SuaSPCT request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var sanPham = await _sanPhamChiTetService.Update(request);
            if (sanPham == null)
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");

            return Ok(sanPham);
        }
    }
}
