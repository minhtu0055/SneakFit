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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deGiayService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _deGiayService.GetById(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemDeGiay request)
        {
            var result = await _deGiayService.Create(request);
            if (result.Id == Guid.Empty) return BadRequest();



            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SuaDeGiay request)
        {
            var result = await _deGiayService.Update(request);
            if (result == null) return NotFound();

            return Ok();
        }

    }
}
