using Newtonsoft.Json;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using SneakFit.ViewModels.Catalog.DeGiay;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;

namespace SneakFit.ApiIntegration.Services
{
    public class DeGiayApiClient : IDeGiayApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeGiayApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<DeGiayViewModels> Create(ThemDeGiay request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/degiay/create", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<DeGiayViewModels>(result);

            throw new Exception("Không thể tạo đế giày");
        }

        public async Task<PagedResult<DeGiayViewModels>> GetAllPaging(DeGiayPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/degiay/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var degiay = JsonConvert.DeserializeObject<PagedResult<DeGiayViewModels>>(body);
            return degiay;
        }

        public async Task<DeGiayViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/DeGiay/GetById/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var degiay = JsonConvert.DeserializeObject<DeGiayViewModels>(body);
            return degiay;
        }

        public async Task<DeGiayViewModels> Update(SuaDeGiay request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/degiay/edit/{request.Id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<DeGiayViewModels>(result);

            throw new Exception("Không thể cập nhật đế giày");
        }
        public async Task<List<DeGiayViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/degiay/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Nếu API trả về array
                var result = JsonConvert.DeserializeObject<List<DeGiayViewModels>>(body);
                return result ?? new List<DeGiayViewModels>();
            }
            throw new Exception("Không thể lấy danh sách đế giày");
        }
    }
}
