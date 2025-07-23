using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class HoaDonClientApiClient : IHoaDonClientApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HoaDonClientApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<HoaDonClientViewModel>> GetAllPaging(PhanTrangHoaDonClient request, Guid? userId = null)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var url = $"/api/HoaDonClient?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&trangthaihoadon={request.Trangthaihoadon}&ngayBatDau={request.NgayBatDau:yyyy-MM-dd}&ngayKetThuc={request.NgayKetThuc:yyyy-MM-dd}";
            if (userId.HasValue)
            {
                url += $"&userId={userId}";
            }
            var response = await client.GetAsync(url);

            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<HoaDonClientViewModel>>(body);
                return result ?? new PagedResult<HoaDonClientViewModel>();
            }
            throw new Exception("Không thể lấy danh sách hóa đơn");
        }

        public async Task<HoaDonClientViewModel> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDonClient/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var hoaDon = JsonConvert.DeserializeObject<HoaDonClientViewModel>(body);
            if (hoaDon == null)
                throw new Exception($"Không thể lấy thông tin hóa đơn hoặc dữ liệu không hợp lệ: {body}");
            return hoaDon;
        }

        public async Task<HoaDonClientViewModel> Create(ThemHoaDonClient request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/HoaDonClient", httpContent);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Tạo hóa đơn thất bại");
            return JsonConvert.DeserializeObject<HoaDonClientViewModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<HoaDonClientViewModel> Update(SuaHoaDonClient request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/HoaDonClient/{request.Id}", httpContent);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Cập nhật hóa đơn thất bại");
            return JsonConvert.DeserializeObject<HoaDonClientViewModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> UpdateStatus(Guid id, SneakFit.Data.Enums.TrangThaiHoaDon newStatus)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sessions);

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(newStatus), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"/api/HoaDonClient/{id}/trangthai", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<Dictionary<string, int>> GetCountByStatusAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync("/api/hoadonClient/count-by-status"); // Đảm bảo API của bạn trả về số lượng theo từng trạng thái
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, int>>(body);
                return result; // Trả về một dictionary với key là tên trạng thái, value là số lượng
            }
            else
            {
                throw new Exception($"Không thể lấy dữ liệu số lượng theo trạng thái: {body}");
            }
        }
    }
}
