using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.MauSac;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MauSacController : ControllerBase
    {
        private readonly IMauSacService _mauSacService;

        public MauSacController(IMauSacService mauSacService)
        {
            _mauSacService = mauSacService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _mauSacService.GetAll();
            return Ok(list);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getid = await _mauSacService.GetById(id);
            return Ok(getid);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ThemMauSac request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var mausac = await _mauSacService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = mausac.Id }, mausac);
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaMauSac request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var mausac = await _mauSacService.Update(request);
            return Ok(mausac);
        }
    }
}
