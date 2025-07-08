using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using SneakFit.ViewModels.System.DiaChi;
using System.Threading.Tasks;

namespace SneakFit.WebClient.Controllers
{
    [Route("[controller]/[action]")]
    public class DiaChiController : Controller
    {
        private readonly IDiaChiApiClient _diaChiApiClient;
        public DiaChiController(IDiaChiApiClient diaChiApiClient)
        {
            _diaChiApiClient = diaChiApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _diaChiApiClient.GetAllByUser();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _diaChiApiClient.GetById(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThemDiaChiViewModel request)
        {
            var result = await _diaChiApiClient.Create(request);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] SuaDiaChiViewModel request)
        {
            var result = await _diaChiApiClient.Update(id, request);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _diaChiApiClient.Delete(id);
            return Json(result);
        }

        [HttpPut]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _diaChiApiClient.SetDefault(id);
            return Json(result);
        }
    }
} 