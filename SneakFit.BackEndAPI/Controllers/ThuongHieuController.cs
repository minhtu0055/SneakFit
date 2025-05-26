using System.Net.WebSockets;
using System.Threading.Tasks.Sources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.ThuongHieu;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.ThuongHieu;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThuongHieuController : ControllerBase
    {
        private readonly IThuongHieuService _thuongHieuService;

        public ThuongHieuController(IThuongHieuService thuongHieuService)
        {
            _thuongHieuService = thuongHieuService;
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] ThuongHieuPagingRequest request)
        {
            var result = await _thuongHieuService.GetAllPaging(request);
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _thuongHieuService.GetAll();
            return Ok(list);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var getid = await _thuongHieuService.GetById(id);
            return Ok(getid);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody]ThemThuongHieu request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var thuonghieu = await _thuongHieuService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = thuonghieu.Id }, thuonghieu);
        }
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody]SuaThuongHieu request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = id;
            var thuonghieu = await _thuongHieuService.Update(request);
            return Ok(thuonghieu);
        }
    }
}
