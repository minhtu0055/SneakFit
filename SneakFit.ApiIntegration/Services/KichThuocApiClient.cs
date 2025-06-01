using Newtonsoft.Json;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using SneakFit.ViewModels.Catalog.KichThuoc;

namespace SneakFit.ApiIntegration.Services
{
    public class KichThuocApiClient : IKichThuocApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KichThuocApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<KichThuocViewModels> Create(ThemKichThuoc request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/kichthuoc/create", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<KichThuocViewModels>(result);

            throw new Exception("Không thể tạo kích thước");
        }

        public async Task<PagedResult<KichThuocViewModels>> GetAllPaging(KichThuocPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/kichthuoc/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var kichthuoc = JsonConvert.DeserializeObject<PagedResult<KichThuocViewModels>>(body);
            return kichthuoc;
        }

        public async Task<KichThuocViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/kichthuoc/getbyid/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var kichthuoc = JsonConvert.DeserializeObject<KichThuocViewModels>(body);
            return kichthuoc;
        }

        public async Task<KichThuocViewModels> Update(SuaKichThuoc request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/kichthuoc/edit/{request.Id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<KichThuocViewModels>(result);

            throw new Exception("Không thể cập nhật kích thước");
        }
        
        public async Task<List<KichThuocViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/kichthuoc/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Nếu API trả về array
                var result = JsonConvert.DeserializeObject<List<KichThuocViewModels>>(body);
                return result ?? new List<KichThuocViewModels>();
            }
            throw new Exception("Không thể lấy danh sách kích thước");
        }
    }
}
