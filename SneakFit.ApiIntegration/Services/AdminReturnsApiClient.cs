using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;

namespace SneakFit.ApiIntegration.Services
{
    public class AdminReturnsApiClient : IAdminReturnsApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminReturnsApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<PagedResult<ReturnViewModel>> GetPagingAsync(int pageIndex, int pageSize,
            int? status = null, string? keyword = null, DateTime? from = null, DateTime? to = null)
        {
            var client = CreateClient();
            var qs = new List<string>
            {
                $"pageIndex={pageIndex}",
                $"pageSize={pageSize}"
            };
            if (status.HasValue) qs.Add($"status={status.Value}");
            if (!string.IsNullOrWhiteSpace(keyword)) qs.Add($"keyword={Uri.EscapeDataString(keyword)}");
            if (from.HasValue) qs.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue) qs.Add($"to={to.Value:yyyy-MM-dd}");
            var url = "/api/admin/returns" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<PagedResult<ReturnViewModel>>(body)
                       ?? new PagedResult<ReturnViewModel>();
            }
            throw new Exception($"Không thể lấy danh sách yêu cầu: {body}");
        }

        public async Task<ReturnViewModel?> GetDetailAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/admin/returns/{id}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Trường hợp controller trả trực tiếp VM
                    var vm = JsonConvert.DeserializeObject<ReturnViewModel>(body);
                    if (vm != null) return vm;
                    // Trường hợp bọc trong ApiSuccessResult
                    var wrapped = JsonConvert.DeserializeObject<ApiSuccessResult<ReturnViewModel>>(body);
                    return wrapped?.ResultObj;
                }
                catch { }
            }
            return null;
        }

        public async Task<bool> ApproveAsync(Guid id, string? carrier, string? shipCode)
        {
            var client = CreateClient();
            var qs = new List<string>();
            if (!string.IsNullOrWhiteSpace(carrier)) qs.Add($"carrier={Uri.EscapeDataString(carrier)}");
            if (!string.IsNullOrWhiteSpace(shipCode)) qs.Add($"shipCode={Uri.EscapeDataString(shipCode)}");
            var url = $"/api/admin/returns/{id}/approve" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
            var response = await client.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReceiveAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.PutAsync($"/api/admin/returns/{id}/receive", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompleteAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.PutAsync($"/api/admin/returns/{id}/complete", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectAsync(Guid id, string reason)
        {
            var client = CreateClient();
            var url = $"/api/admin/returns/{id}/reject?reason={Uri.EscapeDataString(reason ?? string.Empty)}";
            var response = await client.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStatusWithLogAsync(Guid id, int newStatus, string ghiChu, string nguoiChinhSua)
        {
            var client = CreateClient();
            var request = new
            {
                newStatus = newStatus,
                ghiChu = ghiChu,
                nguoiChinhSua = nguoiChinhSua
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/admin/returns/{id}/update-status", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ReturnStatusHistoryViewModel>> GetStatusHistoryAsync(Guid id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/admin/returns/{id}/history");
            var body = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<ReturnStatusHistoryViewModel>>(body) ?? new List<ReturnStatusHistoryViewModel>();
                }
                catch
                {
                    return new List<ReturnStatusHistoryViewModel>();
                }
            }
            return new List<ReturnStatusHistoryViewModel>();
        }
    }
}
