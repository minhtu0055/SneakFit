using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class DiaChiApiClient : IDiaChiApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DiaChiApiClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<DiaChiViewModel>> GetAllByUser()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/diachi");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);
            // Deserialize theo ApiResult<List<DiaChiViewModel>>
            var apiResult = JsonConvert.DeserializeObject<ApiResult<List<DiaChiViewModel>>>(body);
            return apiResult?.ResultObj ?? new List<DiaChiViewModel>();
        }

        public async Task<ApiResult<DiaChiViewModel>> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/diachi/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<DiaChiViewModel>>(body);
        }

        public async Task<ApiResult<bool>> Create(ThemDiaChiViewModel request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/diachi", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> Update(Guid id, SuaDiaChiViewModel request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/diachi/{id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/diachi/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(body);
        }

        public async Task<ApiResult<bool>> SetDefault(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/diachi/{id}/set-default", null);
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResult<bool>>(body);
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
        // Các phương thức mới theo pattern by-user/{userId}
        public async Task<ApiResult<DiaChiViewModel>> GetByIdByUser(Guid userId, Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/DiaChi/by-user/{userId}/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<DiaChiViewModel>>(body);
                return result;
            }
            throw new Exception("Không thể lấy thông tin địa chỉ của user");
        }

        public async Task<ApiResult<bool>> CreateByUser(Guid userId, ThemDiaChiViewModel request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/DiaChi/by-user/{userId}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<bool>>(body);
                return result;
            }
            throw new Exception("Không thể tạo địa chỉ cho user");
        }

        public async Task<ApiResult<bool>> UpdateByUser(Guid userId, Guid id, SuaDiaChiViewModel request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/DiaChi/by-user/{userId}/{id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<bool>>(body);
                return result;
            }
            throw new Exception("Không thể cập nhật địa chỉ cho user");
        }

        public async Task<ApiResult<bool>> DeleteByUser(Guid userId, Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/DiaChi/by-user/{userId}/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<bool>>(body);
                return result;
            }
            throw new Exception("Không thể xóa địa chỉ của user");
        }

        public async Task<ApiResult<bool>> SetDefaultByUser(Guid userId, Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"/api/DiaChi/by-user/{userId}/{id}/set-default", null);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ApiResult<bool>>(body);
                return result;
            }
            throw new Exception("Không thể đặt địa chỉ mặc định cho user");
        }
       
    }
} 
