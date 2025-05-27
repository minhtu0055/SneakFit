using Azure.Core;
using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class SanPhamApiClient : ISanPhamApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SanPhamApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/SanPham/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&tuKhoa={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<SanPhamViewModels>>(body);
                return result ?? new PagedResult<SanPhamViewModels>();
            }
            throw new Exception("Không thể lấy danh sách sản phẩm");
        }

        public async Task<SanPhamViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/SanPham/GetById/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var sanPham = JsonConvert.DeserializeObject<SanPhamViewModels>(body);
            if (sanPham == null)
                throw new Exception($"Không thể lấy thông tin sản phẩm hoặc dữ liệu không hợp lệ: {body}");
            return sanPham;
        }

        public async Task<SanPhamViewModels> Create(ThemSanPham request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/SanPham/Create", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<SanPhamViewModels>(body);

            throw new Exception("Tạo sản phẩm thất bại");
        }

        public async Task<SanPhamViewModels> Update(SuaSanPham request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/SanPham/Update/{request.Id}", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<SanPhamViewModels>(body);

            throw new Exception("Cập nhật sản phẩm thất bại");
        }

        public async Task<List<SanPhamViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/sanpham/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Nếu API trả về array
                var result = JsonConvert.DeserializeObject<List<SanPhamViewModels>>(body);
                return result ?? new List<SanPhamViewModels>();
            }
            throw new Exception("Không thể lấy danh sách sản phẩm");

        }

        public async Task<bool> UpdateTrangThai(Guid id, bool trangThai)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(trangThai);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/SanPham/{id}/trangThai", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result, settings);
                return apiResult.ResultObj;
            }

            throw new Exception($"Không thể cập nhật trạng thái. Error: {result}");
        }

        public async Task<bool> UpdateSPCT(Guid id, List<SanPhamChiTietCapNhat> updates)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(updates);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/SanPham/{id}/spct", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result, settings);
                return apiResult.ResultObj;
            }

            throw new Exception($"Không thể cập nhật SPCT. Error: {result}");
        }

        public async Task<List<SPCTViewModels>> GetSPCTByFilter(SPCTFilterRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/SanPham/GetSPCTByFilter", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<SPCTViewModels>>(body);
                return result ?? new List<SPCTViewModels>();
            }
            throw new Exception("Không thể lấy danh sách SPCT theo filter");
        }

        public async Task<List<SPCTViewModels>> GetSPCTByProductName(string productName)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/SanPham/GetSPCTByProductName/{Uri.EscapeDataString(productName)}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<SPCTViewModels>>(body);
                return result ?? new List<SPCTViewModels>();
            }
            throw new Exception("Không thể lấy danh sách SPCT theo tên sản phẩm");
        }

        public async Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/SanPham/GetSPCTDetail/{spctId}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<SPCTDetailViewModel>(body);
                return result;
            }
            throw new Exception("Không thể lấy chi tiết SPCT");
        }

        public async Task<bool> UpdateSPCTDetail(SuaSPCTDetailViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/SanPham/UpdateSPCTDetail", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
