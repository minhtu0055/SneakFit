using Newtonsoft.Json;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class KhuyenMaiApiClient : IKhuyenMaiApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KhuyenMaiApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<KhuyenMaiViewModels>> GetAllPaging(PhanTrangKhuyenMai request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/khuyenMai?pageIndex={request.PageIndex}" +
                    $"&pageSize={request.PageSize}" +
                    $"&keyword={request.Keyword}" +
                    $"&trangthai={request.TrangThai}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<KhuyenMaiViewModels>>>(body, settings);
                    var pagedResult = apiResult?.ResultObj ?? new PagedResult<KhuyenMaiViewModels>();
                    pagedResult.Items = pagedResult.Items ?? new List<KhuyenMaiViewModels>();
                    return pagedResult;
                }

                throw new Exception("Không thể lấy danh sách khuyến mại");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khuyến mại: {ex.Message}");
            }
        }

        public async Task<KhuyenMaiViewModels> GetById(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions); // thiết lập header bearer thường được dùng với JWT

                //Gửi yều cầu get đến api 
                var response = await client.GetAsync($"/api/khuyenMai/GetById/{id}"); // await đảm bảo gọi API bất đồng bộ, không làm chặn chương trình
                var body = await response.Content.ReadAsStringAsync(); // đọc nội dung phần hồi từ API dưới dạng Json

                //Kiểm tra phản hồi
                if (response.IsSuccessStatusCode)
                {
                    var khuyenMai = JsonConvert.DeserializeObject<ApiSuccessResult<KhuyenMaiViewModels>>(body);
                    return khuyenMai.ResultObj;
                }

                throw new Exception("Không thể lấy thông tin khuyến mại");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin khuyến mại: {ex.Message}");
            }
        }

        public async Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"/api/khuyenMai/create", httpContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<KhuyenMaiViewModels>>(result);
                    return apiResult.ResultObj;
                }
                else
                {
                    // Lấy message lỗi chi tiết từ response (nếu có)
                    var errorMsg = result;
                    try
                    {
                        // Nếu API trả về lỗi dạng JSON có trường message
                        dynamic errorObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        if (errorObj != null && errorObj.message != null)
                        {
                            errorMsg = errorObj.message.ToString();
                        }
                        // Nếu có trường errors (dạng ModelState)
                        else if (errorObj != null && errorObj.errors != null)
                        {
                            errorMsg = string.Join("; ", errorObj.errors.ToObject<List<string>>());
                        }
                    }
                    catch { /* Nếu không parse được thì giữ nguyên errorMsg */ }
                    throw new Exception(errorMsg);
                }

               // throw new Exception("Không thể tạo khuyến mại");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo khuyến mại: {ex.Message}");
            }
        }

        public async Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"/api/khuyenMai/Edit/{request.Id}", httpContent);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<KhuyenMaiViewModels>>(result);
                    return apiResult.ResultObj;
                }
                else
                {
                    // Lấy message lỗi chi tiết từ response (nếu có)
                    var errorMsg = result;
                    try
                    {
                        // Nếu API trả về lỗi dạng JSON có trường message
                        dynamic errorObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        if (errorObj != null && errorObj.message != null)
                        {
                            errorMsg = errorObj.message.ToString();
                        }
                        // Nếu có trường errors (dạng ModelState)
                        else if (errorObj != null && errorObj.errors != null)
                        {
                            errorMsg = string.Join("; ", errorObj.errors.ToObject<List<string>>());
                        }
                    }
                    catch { /* Nếu không parse được thì giữ nguyên errorMsg */ }
                    throw new Exception(errorMsg);
                }

                // Lấy lỗi chi tiết từ API và ném ra
                //throw new Exception(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật khuyến mại: {ex.Message}");
            }
        }

        public async Task<bool> UpdateStatus(Guid id, TrangThaiGiamGia trangThai)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(trangThai);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PatchAsync($"/api/khuyenMai/{id}/TrangThai", httpContent);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
                    return apiResult.ResultObj;
                }

                throw new Exception("Không thể cập nhật trạng thái khuyến mãi");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái khuyến mãi: {ex.Message}");
            }
        }
    }
}
