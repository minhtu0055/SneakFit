using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.System.DiaChi;
using SneakFit.ViewModels.System.DiaChi;
using System.Security.Claims;
namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaChiController : ControllerBase
    {
        private readonly IDiaChiService _diaChiService;

        public DiaChiController(IDiaChiService diaChiService)
        {
            _diaChiService = diaChiService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllByUser()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.GetAllByUser(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.GetById(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.Create(userId, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.Update(id, userId, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.Delete(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _diaChiService.SetDefault(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetAllByUserId(Guid userId)
        {
            var result = await _diaChiService.GetAllByUser(userId);
            return Ok(result);
        }
        [HttpGet("by-user/{userId}/{id}")]
        public async Task<IActionResult> GetByIdByUser(Guid userId, Guid id)
        {
            var result = await _diaChiService.GetById(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("by-user/{userId}")]
        public async Task<IActionResult> CreateByUser(Guid userId, [FromBody] ThemDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _diaChiService.Create(userId, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("by-user/{userId}/{id}")]
        public async Task<IActionResult> UpdateByUser(Guid userId, Guid id, [FromBody] SuaDiaChiViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _diaChiService.Update(id, userId, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpDelete("by-user/{userId}/{id}")]
        public async Task<IActionResult> DeleteByUser(Guid userId, Guid id)
        {
            var result = await _diaChiService.Delete(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("by-user/{userId}/{id}/set-default")]
        public async Task<IActionResult> SetDefaultByUser(Guid userId, Guid id)
        {
            var result = await _diaChiService.SetDefault(id, userId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
