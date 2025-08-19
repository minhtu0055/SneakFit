using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;

namespace SneakFit.ApiIntegration.Services
{
    public class TraHangApiClient : ITraHangApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraHangApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext?.Session?.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            }
            return client;
        }

        public async Task<ApiResult<Guid>> CreateAsync(CreateReturnRequest request, List<IFormFile>? images = null)
        {
            var client = CreateClient();
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(request.OrderId.ToString()), "OrderId");
            form.Add(new StringContent(request.Reason ?? string.Empty), "Reason");
            form.Add(new StringContent(((int)request.Method).ToString()), "Method");

            if (request.Bank != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Bank.AccountName))
                    form.Add(new StringContent(request.Bank.AccountName), "Bank.AccountName");
                if (!string.IsNullOrWhiteSpace(request.Bank.AccountNumber))
                    form.Add(new StringContent(request.Bank.AccountNumber), "Bank.AccountNumber");
                if (!string.IsNullOrWhiteSpace(request.Bank.BankName))
                    form.Add(new StringContent(request.Bank.BankName), "Bank.BankName");
            }

            if (request.Items != null && request.Items.Count > 0)
            {
                for (int i = 0; i < request.Items.Count; i++)
                {
                    form.Add(new StringContent(request.Items[i].OrderItemId.ToString()), $"Items[{i}].OrderItemId");
                    form.Add(new StringContent(request.Items[i].Quantity.ToString()), $"Items[{i}].Quantity");
                }
            }

            if (images != null)
            {
                foreach (var file in images)
                {
                    if (file?.Length > 0)
                    {
                        var stream = file.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                        form.Add(fileContent, "EvidenceImages", file.FileName);
                    }
                }
            }

            var response = await client.PostAsync("/api/returns", form);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // API nên trả ApiSuccessResult<Guid>, nhưng ta chấp nhận ApiResult<Guid> chung
                var ok = JsonConvert.DeserializeObject<ApiResult<Guid>>(body);
                return ok ?? new ApiResult<Guid> { IsSuccessed = true, ResultObj = Guid.Empty, Message = "Tạo yêu cầu thành công" };
            }

            // lỗi: cố gắng parse ApiErrorResult
            try
            {
                var err = JsonConvert.DeserializeObject<ApiErrorResult<Guid>>(body);
                return new ApiResult<Guid> { IsSuccessed = false, Message = err?.Message ?? "Tạo yêu cầu thất bại" };
            }
            catch
            {
                return new ApiResult<Guid> { IsSuccessed = false, Message = $"Lỗi server: {body}" };
            }
        }

        public async Task<PagedResult<ReturnViewModel>> GetMyReturnsAsync(int pageIndex, int pageSize)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/returns/my?pageIndex={pageIndex}&pageSize={pageSize}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<ReturnViewModel>>(body);
                return result ?? new PagedResult<ReturnViewModel>();
            }

            throw new Exception($"Không thể lấy danh sách yêu cầu trả hàng: {body}");
        }

        public async Task<PagedResult<ReturnViewModel>> GetMyAsync(Guid userId, int pageIndex, int pageSize)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/Returns/GetMyAsync?userId={userId}&pageIndex={pageIndex}&pageSize={pageSize}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<ReturnViewModel>>(body);
                return result ?? new PagedResult<ReturnViewModel>();
            }

            throw new Exception($"Không thể lấy danh sách yêu cầu trả hàng: {body}");
        }

        public async Task<ApiSuccessResult<ReturnViewModel>> GetDetailAsync(Guid returnId)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/returns/{returnId}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Có thể API trả trực tiếp ReturnViewModel hoặc bọc trong ApiSuccessResult
                try
                {
                    var wrapped = JsonConvert.DeserializeObject<ApiSuccessResult<ReturnViewModel>>(body);
                    if (wrapped?.ResultObj != null) return wrapped;
                }
                catch { }

                var plain = JsonConvert.DeserializeObject<ReturnViewModel>(body);
                if (plain != null)
                {
                    return new ApiSuccessResult<ReturnViewModel>(plain);
                }
            }

            return new ApiSuccessResult<ReturnViewModel>(null) { IsSuccessed = false, Message = $"Không thể lấy chi tiết yêu cầu: {body}" };
        }

        public async Task<ReturnViewModel?> GetDetailAsync(Guid id, Guid userId)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/Returns/GetDetailAsync?id={id}&userId={userId}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<ReturnViewModel>(body);
                return result;
            }

            return null;
        }

        public async Task<ApiResult<bool>> CancelAsync(Guid returnId)
        {
            var client = CreateClient();
            var response = await client.PutAsync($"/api/returns/{returnId}/cancel", null);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var ok = JsonConvert.DeserializeObject<ApiResult<bool>>(body);
                return ok ?? new ApiResult<bool> { IsSuccessed = true, ResultObj = true, Message = "Đã hủy yêu cầu" };
            }

            try
            {
                var err = JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
                return new ApiResult<bool> { IsSuccessed = false, Message = err?.Message ?? "Hủy thất bại" };
            }
            catch
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = $"Lỗi server: {body}" };
            }
        }
        public async Task<bool> HasAsync(Guid orderId)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"/api/returns/has?orderId={orderId}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Body có thể là "true"/"false" hoặc JSON boolean
                if (bool.TryParse(body, out var simple)) return simple;
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(body);
                }
                catch { return false; }
            }
            return false;
        }
    }
}
