using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace SneakFit.ApiIntegration.Services
{
    public class DiaChiApiClient : IDiaChiApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DiaChiApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<List<DiaChiViewModel>>> GetAllByUserId(Guid userId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/DiaChi/by-user/{userId}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<List<DiaChiViewModel>>>(body);
                return result;
            }
            throw new Exception("Không thể lấy danh sách địa chỉ của user");
        }
    }
}
