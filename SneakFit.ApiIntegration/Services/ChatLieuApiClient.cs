using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using SneakFit.ViewModels.System.User;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;

namespace SneakFit.ApiIntegration.Services
{
    public class ChatLieuApiClient : IChatLieuApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatLieuApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ChatLieuViewModels> Create(ThemChatLieu request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/chatlieu/create", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ChatLieuViewModels>(result);

            throw new Exception("Không thể tạo chất liệu");
        }

        public async Task<PagedResult<ChatLieuViewModels>> GetAllPaging(ChatLieuPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/chatlieu/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<PagedResult<ChatLieuViewModels>>(body);
            return users;
        }

        public async Task<ChatLieuViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/chatlieu/getbyid/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var chatlieu = JsonConvert.DeserializeObject<ChatLieuViewModels>(body);
            return chatlieu;
        }

        public async Task<ChatLieuViewModels> Update(SuaChatLieu request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/chatlieu/edit/{request.Id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ChatLieuViewModels>(result);

            throw new Exception("Không thể cập nhật chất liệu");
        }
        public async Task<List<ChatLieuViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var response = await client.GetAsync($"/api/chatlieu/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Nếu API trả về array
                var result = JsonConvert.DeserializeObject<List<ChatLieuViewModels>>(body);
                return result ?? new List<ChatLieuViewModels>();
            }
            throw new Exception("Không thể lấy danh sách chất liệu");
        }
    }
}
