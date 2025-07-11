using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class HoaDonChiTietApiClient : IHoaDonChiTietApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HoaDonChiTietApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<HoaDonChiTietViewModel>> GetAllPaging(PhanTrangHoaDonChiTiet request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDonChiTiet?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");

            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<HoaDonChiTietViewModel>>(body);
                return result ?? new PagedResult<HoaDonChiTietViewModel>();
            }
            throw new Exception("Không thể lấy danh sách hóa đơn");
        }

        public async Task<List<HoaDonChiTietViewModel>> GetByHoaDonId(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDonChiTiet/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var hoaDon = JsonConvert.DeserializeObject<List<HoaDonChiTietViewModel>>(body);
            if (hoaDon == null)
                throw new Exception($"Không thể lấy thông tin hóa đơn hoặc dữ liệu không hợp lệ: {body}");
            return hoaDon;
        }

        public async Task<HoaDonChiTietViewModel> Create(ThemHoaDonChiTiet request) {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/HoaDonChiTiet", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonChiTietViewModel>(body);

            throw new Exception("Tạo hóa đơn thất bại");
        }

        public async Task<HoaDonChiTietViewModel> Edit(SuaHoaDonChiTiet request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/HoaDonChiTiet/{request.Id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonChiTietViewModel>(body);

            throw new Exception("Cập nhật hóa đơn thất bại");
        }

        public async Task<HoaDonChiTietViewModel> CreateOrUpdate(ThemHoaDonChiTiet request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/HoaDonChiTiet/CreateOrUpdate", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonChiTietViewModel>(body);

            throw new Exception("Tạo/cập nhật hóa đơn chi tiết thất bại");
        }

        public async Task<bool> Delete(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/HoaDonChiTiet/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return true;

            throw new Exception("Xóa hóa đơn chi tiết thất bại");
        }

        public async Task<bool> UpdateQuantity(Guid hoaDonChiTietId, int newQuantity)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(new { HoaDonChiTietId = hoaDonChiTietId, NewQuantity = newQuantity });
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/HoaDonChiTiet/UpdateQuantity", httpContent);
            return response.IsSuccessStatusCode;
        }
    }
}
