using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.KichThuoc;
using SneakFit.ViewModels.Catalog.KichThuoc;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KichThuocController : ControllerBase
    {
        private readonly IKichThuocService _kichThuocService;

        public KichThuocController(IKichThuocService kichThuocService)
        {
            _kichThuocService = kichThuocService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _kichThuocService.GetAll();
            return Ok(list);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getid = await _kichThuocService.GetById(id);
            return Ok(getid);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ThemKichThuoc request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var kichthuoc = await _kichThuocService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = kichthuoc.Id }, kichthuoc);
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaKichThuoc request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var kichthuoc = await _kichThuocService.Update(request);
            return Ok(kichthuoc);
        }
    }
}
