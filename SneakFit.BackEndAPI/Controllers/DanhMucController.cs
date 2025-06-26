using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DanhMucController : ControllerBase
    {
        private readonly IDanhMucService _danhMucService;

        public DanhMucController(IDanhMucService danhMucService)
        {
            _danhMucService = danhMucService;
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] DanhMucPagingRequest request)
        {
            var result = await _danhMucService.GetAllPaging(request);
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _danhMucService.GetAll();
            return Ok(list);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getid = await _danhMucService.GetById(id);
            return Ok(getid);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ThemDanhMuc request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var danhmuc = await _danhMucService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = danhmuc.Id }, danhmuc);
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaDanhMuc request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var danhmuc = await _danhMucService.Update(request);
            return Ok(danhmuc);
        }
        [HttpPost("UpdateProductCount/{id}")]
        public async Task<IActionResult> UpdateProductCount(Guid id)
        {
            var result = await _danhMucService.UpdateProductCount(id);
            return Ok(new ApiSuccessResult<bool>());
        }
    }
}
