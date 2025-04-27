using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakFit.Application.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.KhuyenMai;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhuyenMaiController : ControllerBase
    {
        private readonly IKhuyenMaiService _khuyenMaiService;

        public KhuyenMaiController(IKhuyenMaiService khuyenMaiService)
        {
            _khuyenMaiService = khuyenMaiService;
        }

        // GET: api/KhuyenMai
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _khuyenMaiService.GetAll();
            return Ok(result);
        }

        // GET: api/KhuyenMai/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _khuyenMaiService.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/KhuyenMai
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemKhuyenMai request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _khuyenMaiService.Create(request);
            return Ok(result);
        }

        // PUT: api/KhuyenMai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaKhuyenMai request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != request.Id) return BadRequest("ID không khớp");

            var result = await _khuyenMaiService.Update(request);
            return Ok(result);
        }

       
    }
}
