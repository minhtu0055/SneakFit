using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.LichSuHoaDon;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class HoaDonApiClient : IHoaDonApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HoaDonApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDon?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&trangthaihoadon={request.Trangthaihoadon}&ngayBatDau={request.NgayBatDau:yyyy-MM-dd}&ngayKetThuc={request.NgayKetThuc:yyyy-MM-dd}");

            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<HoaDonViewModel>>(body);
                return result ?? new PagedResult<HoaDonViewModel>();
            }
            throw new Exception("Không thể lấy danh sách hóa đơn");
        }

        public async Task<HoaDonViewModel> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDon/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var hoaDon = JsonConvert.DeserializeObject<HoaDonViewModel>(body);
            if (hoaDon == null)
                throw new Exception($"Không thể lấy thông tin hóa đơn hoặc dữ liệu không hợp lệ: {body}");
            return hoaDon;
        }

        public async Task<HoaDonViewModel> Create(ThemHoaDon request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/HoaDon", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonViewModel>(body);

            throw new Exception("Tạo hóa đơn thất bại");
        }

        public async Task<HoaDonViewModel> Update(SuaHoaDon request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/HoaDon/{request.Id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<HoaDonViewModel>(body);

            throw new Exception("Cập nhật hóa đơn thất bại");
        }

        public async Task<bool> UpdateStatus(Guid id, TrangThaiHoaDon trangThai)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.PutAsync($"/api/HoaDon/{id}/status/{trangThai}", null);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result, settings);
                return apiResult?.ResultObj ?? false;
            }

            throw new Exception($"Không thể cập nhật trạng thái hóa đơn. Error: {result}");
        }
        public async Task<Dictionary<string, int>> GetCountByStatusAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync("/api/hoadon/count-by-status"); // Đảm bảo API của bạn trả về số lượng theo từng trạng thái
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
        public async Task<List<HoaDonViewModel>> GetHoaDonChoByNguoiTao(string nguoiTao)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDon/cho-by-nguoitao?nguoiTao={nguoiTao}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<HoaDonViewModel>>(body);
                return result ?? new List<HoaDonViewModel>();
            }

            throw new Exception("Không thể lấy danh sách hóa đơn chờ");
        }

        public async Task<bool> Delete(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/HoaDon/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ThanhToan(SuaHoaDon request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/hoadon/thanhtoan", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(body);
                return result.success == true;
            }
            return false;
        }

        public async Task<Guid> CreateHistory(CreateLichSuHoaDonRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/HoaDon/history", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Guid>(body);
            }
            throw new Exception($"Tạo lịch sử hóa đơn thất bại: {body}");
        }

        public async Task<List<LichSuHoaDonViewModel>> GetHistoryByHoaDonId(Guid hoaDonId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/HoaDon/{hoaDonId}/history");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<LichSuHoaDonViewModel>>(body);
                return result ?? new List<LichSuHoaDonViewModel>();
            }
            throw new Exception($"Không thể lấy lịch sử hóa đơn: {body}");
        }

        public async Task<bool> RevertToPreviousStatus(Guid hoaDonId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.PostAsync($"/api/HoaDon/{hoaDonId}/revert", null);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateStatusAndLogAsync(Guid hoaDonId, TrangThaiHoaDon newStatus, Guid userId, string? nguoiChinhSua = null)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var request = new UpdateHoaDonStatusRequest
            {
                NewStatus = newStatus,
                UserId = userId,
                NguoiChinhSua = nguoiChinhSua
            };
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/HoaDon/{hoaDonId}/update-status", httpContent);
            return response.IsSuccessStatusCode;
        }
    }
}
