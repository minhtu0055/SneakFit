using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SneakFit.Application.GHN;
using SneakFit.ViewModels.GHN;
using System.Text;
using System.Text.Json;

namespace SneakFit.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly IGhnService _ghnService;
        private readonly IConfiguration _configuration;

        public GhnController(IGhnService ghnService, IConfiguration configuration)
        {
            _ghnService = ghnService;
            _configuration = configuration;
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
        [HttpPost("shipping-fee")]
        public async Task<IActionResult> CalculateShippingFee([FromBody] ShippingFeeRequest request)
        {
            try
            {
                if (request.FromDistrictId == 0)
                {
                    request.FromDistrictId = int.Parse(_configuration["GhnSettings:FromDistrictId"]);
                }
                var result = await _ghnService.CalculateShippingFeeAsync(request);
                return Ok(JsonDocument.Parse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("available-services")]
        public async Task<IActionResult> GetAvailableServices([FromBody] AvailableServiceRequest request)
        {
            try
            {
                var result = await _ghnService.GetAvailableServicesAsync(request.FromDistrict, request.ToDistrict);
                return Ok(JsonDocument.Parse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
