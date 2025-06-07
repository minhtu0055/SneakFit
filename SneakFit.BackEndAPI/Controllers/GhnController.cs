using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SneakFit.Application.GHN;
using System.Text;
using System.Text.Json;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly IGhnService _ghnService;

        public GhnController(IGhnService ghnService)
        {
            _ghnService = ghnService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _ghnService.GetProvincesAsync();
            return Ok(JsonDocument.Parse(result));
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts()
        {
            var result = await _ghnService.GetDistrictsAsync();
            return Ok(JsonDocument.Parse(result));
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var result = await _ghnService.GetWardsAsync(districtId);
            return Ok(JsonDocument.Parse(result));
        }
    }
}
