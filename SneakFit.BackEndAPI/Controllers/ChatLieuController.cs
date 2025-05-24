using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.Voucher;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatLieuController : ControllerBase
    {
        private readonly IChatLieuService _chatLieuService;

        public ChatLieuController(IChatLieuService chatLieuService)
        {
            _chatLieuService = chatLieuService;
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] ChatLieuPagingRequest request)
        {
            var result = await _chatLieuService.GetAllPaging(request);
            return Ok(result);
        }
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _chatLieuService.GetById(id);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ThemChatLieu request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var chatlieus = await _chatLieuService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = chatlieus.Id }, chatlieus);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Update([FromBody] SuaChatLieu request)
        {
            try
            {
                var result = await _chatLieuService.Update(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _chatLieuService.GetAll();
            return Ok(result);
        }
    }
}
