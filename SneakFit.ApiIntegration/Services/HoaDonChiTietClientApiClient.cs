using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class HoaDonChiTietClientApiClient : IHoaDonChiTietClientApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HoaDonChiTietClientApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<HoaDonChiTietClientViewModel>> GetAllPaging(PhanTrangHoaDonChiTietClient request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDonChiTietClient?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");

            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<HoaDonChiTietClientViewModel>>(body);
                return result ?? new PagedResult<HoaDonChiTietClientViewModel>();
            }
            throw new Exception("Không thể lấy danh sách hóa đơn");
        }

        public async Task<List<HoaDonChiTietClientViewModel>> GetByHoaDonId(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDonChiTietClient/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var hoaDon = JsonConvert.DeserializeObject<List<HoaDonChiTietClientViewModel>>(body);
            if (hoaDon == null)
                throw new Exception($"Không thể lấy thông tin hóa đơn hoặc dữ liệu không hợp lệ: {body}");
            return hoaDon;
        }

        public async Task<HoaDonChiTietClientViewModel> Create(ThemHoaDonChiTietClient request) {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/HoaDonChiTietClient", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonChiTietClientViewModel>(body);

            throw new Exception("Tạo hóa đơn thất bại");
        }

        public async Task<HoaDonChiTietClientViewModel> Edit(SuaHoaDonChiTietClient request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/HoaDonChiTietClient/{request.Id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonChiTietClientViewModel>(body);

            throw new Exception("Cập nhật hóa đơn thất bại");
        }        
    }
}
