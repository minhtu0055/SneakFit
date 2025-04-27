using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.DeGiay;
using SneakFit.ViewModels.Catalog.DeGiay;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeGiayController : ControllerBase
    {
        private readonly IDeGiayService _deGiayService;

        public DeGiayController(IDeGiayService deGiayService)
        {
            _deGiayService = deGiayService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _deGiayService.GetAll();
            return Ok(list);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getid = await _deGiayService.GetById(id);
            return Ok(getid);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ThemDeGiay request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var degiay = await _deGiayService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = degiay.Id }, degiay);
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] SuaDeGiay request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var degiay = await _deGiayService.Update(request);
            return Ok(degiay);       
        }
    }
}
