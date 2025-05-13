using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class VoucherApiClient:IVoucherApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public VoucherApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public async Task<VoucherViewModels> Create(CreateVoucher request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/voucher", httpContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<VoucherViewModels>>(result);
                    return apiResult.ResultObj;
                }
                throw new Exception("Không thể tạo voucher!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo voucher: {ex.Message}");
            }
        }

        public async Task<ApiResult<PagedResult<VoucherViewModels>>> GetAllPaging(GetVoucherPagingRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/vouchers?pageIndex={request.PageIndex}" +
                    $"&pageSize={request.PageSize}" +
                    $"&keyword={request.Keyword}" +
                    $"&status={request.Status}");
                var body = await response.Content.ReadAsStringAsync();
                var voucher = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<VoucherViewModels>>>(body);
                return voucher;
                //if(response.IsSuccessStatusCode)
                //{
                //    var setting = new JsonSerializerSettings
                //    {
                //        NullValueHandling = NullValueHandling.Ignore,
                //        MissingMemberHandling = MissingMemberHandling.Ignore,
                //    };
                //    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<VoucherViewModels>>>(body, setting);
                //    var pageResult = apiResult?.ResultObj ?? new PagedResult<VoucherViewModels>();
                //    pageResult.Items = pageResult.Items ?? new List<VoucherViewModels>();
                //    return pageResult;
                //}
                //throw new Exception("Không thể lấy danh sách Voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách: {ex.Message}");
            }
        }


        public async Task<VoucherViewModels> GetByCode(string code)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/vouchers/code/{code}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var voucher = JsonConvert.DeserializeObject<ApiSuccessResult<VoucherViewModels>>(body);
                    return voucher.ResultObj;
                }
                throw new Exception("Không thể tìm thấy thông tin voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin voucher: {ex.Message}");
            }
        }

        public async Task<VoucherViewModels> GetById(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/vouchers/{id}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var voucher = JsonConvert.DeserializeObject<ApiSuccessResult<VoucherViewModels>>(body);
                    return voucher.ResultObj;
                }
                throw new Exception("Không thể tìm thấy thông tin voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin voucher: {ex.Message}");
            }
        }

        public async Task<VoucherViewModels> Update(UpdateVoucher request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"/api/vouchers/{request.Id}", httpContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<VoucherViewModels>>(result);
                    return apiResult.ResultObj;
                }

                throw new Exception("Không thể cập nhật voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật voucher: {ex.Message}");
            }
        }

        public async Task<bool> UpdateTrangThai(Guid Id, TrangThaiGiamGia status)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(status);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PatchAsync($"/api/vouchers/{Id}/status", httpContent);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
                    return apiResult.ResultObj;
                }

                throw new Exception("Không thể cập nhật trạng thái voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái voucher: {ex.Message}");
            }
        }

        public async Task<bool> UseVoucher(string code)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _contextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.PostAsync($"/api/vouchers/code/{code}/use", null);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
                    return apiResult.ResultObj;
                }

                throw new Exception("Không thể sử dụng voucher");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi sử dụng voucher: {ex.Message}");
            }
        }
    }
}
