using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SneakFit.ApiIntegration.Services
{
    public class GioHangApiClient : IGioHangApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangApiClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<GioHangViewModel>> GetAllPaging(GioHangPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"/api/giohang/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}";
            if (request.UserId.HasValue)
            {
                url += $"&userId={request.UserId}";
            }
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);
            return JsonConvert.DeserializeObject<PagedResult<GioHangViewModel>>(body);
        }

        public async Task<GioHangViewModel> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/giohang/getbyid/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);
            return JsonConvert.DeserializeObject<GioHangViewModel>(body);
        }

        public async Task<GioHangViewModel> GetByUserId(Guid userId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/giohang/user/{userId}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);
            return JsonConvert.DeserializeObject<GioHangViewModel>(body);
        }

        public async Task<GioHangViewModel> ThemVaoGioHang(ThemVaoGioHangRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/giohang/themvaogiohang", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(result);
            return JsonConvert.DeserializeObject<GioHangViewModel>(result);
        }

        public async Task<GioHangViewModel> CapNhatGioHang(CapNhatGioHangRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/giohang/capnhat", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(result);
            return JsonConvert.DeserializeObject<GioHangViewModel>(result);
        }

        public async Task<bool> XoaSanPhamKhoiGioHang(Guid gioHangChiTietId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/giohang/xoasanpham/{gioHangChiTietId}");
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(result);
            return true;
        }

        public async Task<bool> XoaGioHang(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/giohang/{id}");
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(result);
            return true;
        }
        public async Task<ApiResult<bool>> CapNhatSoLuong(CapNhatGioHang request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/giohang/cap-nhat-so-luong", content);
            var body = await response.Content.ReadAsStringAsync();

            // Có thể kiểm tra lỗi và throw cho debug luôn
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonConvert.DeserializeObject<ApiResult<bool>>(body);
        }
    }
}
