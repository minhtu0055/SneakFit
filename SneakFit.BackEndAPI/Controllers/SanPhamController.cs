using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

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
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] SanPhamPagingRequest request)
        {
            var result = await _sanPhamService.GetAllPaging(request);
            return Ok(result);
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
        public async Task<IActionResult> Create([FromBody] ThemSanPham request)
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

        [HttpPut("{id}/trangThai")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] bool trangThai)
        {
            var result = await _sanPhamService.UpdateTrangThai(id, trangThai);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>($"Không tìm thấy sản phẩm có ID: {id}"));

            return Ok(new ApiSuccessResult<bool>(true));
        }
        [HttpPut("{id}/spct")]
        public async Task<IActionResult> UpdateSPCT(Guid id, [FromBody] List<SanPhamChiTietCapNhat> updates)
        {
            var result = await _sanPhamService.UpdateSPCT(id, updates);
            if (!result)
                return BadRequest(new ApiErrorResult<bool>($"Không tìm thấy sản phẩm có ID: {id}"));

            return Ok(new ApiSuccessResult<bool>(true));
        }

        [HttpPost("GetSPCTByFilter")]
        public async Task<IActionResult> GetSPCTByFilter([FromBody] SPCTFilterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request model", errors = ModelState });
            }

            try
            {
                var result = await _sanPhamService.GetSPCTByFilter(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetSPCTByProductName/{productName}")]
        public async Task<IActionResult> GetSPCTByProductName(string productName)
        {
            try
            {
                var result = await _sanPhamService.GetSPCTByProductName(productName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetSPCTDetail/{spctId}")]
        public async Task<IActionResult> GetSPCTDetail(Guid spctId)
        {
            var detail = await _sanPhamService.GetSPCTDetail(spctId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }
    }
}
