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
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] DeGiayPagingRequest request)
        {
            var result = await _deGiayService.GetAllPaging(request);
            return Ok(result);
        }
        [HttpGet("GetById/{id}")]
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
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deGiayService.GetAll();
            return Ok(result);
        }
    }
}
