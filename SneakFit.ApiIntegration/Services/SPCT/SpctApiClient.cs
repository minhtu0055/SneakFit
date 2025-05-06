using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services.SPCT
{
    public class SpctApiClient : ISpctApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SpctApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<PagedResult<SPCTViewModels>> GetAllPaging(PhanTrangSPCT request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/SPCT/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&tuKhoa={request.TuKhoa}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<SPCTViewModels>>>(body, settings);
                    var pagedResult = apiResult?.ResultObj ?? new PagedResult<SPCTViewModels>();
                    pagedResult.Items = pagedResult.Items ?? new List<SPCTViewModels>();
                    return pagedResult;
                }
                throw new Exception("Không thể lấy danh sách sản phẩm chi tiết");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sản phẩm chi tiết: {ex.Message}");
            }
        }

        public async Task<SPCTViewModels> GetById(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/SPCT/GetById/{id}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<SPCTViewModels>>(body, settings);
                    if (apiResult == null || apiResult.ResultObj == null)
                        throw new Exception($"API trả về dữ liệu không hợp lệ: {body}");
                    return apiResult.ResultObj;
                }
                throw new Exception($"API trả về lỗi - Status: {response.StatusCode}, Body: {body}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin sản phẩm chi tiết: {ex.Message}");
            }
        }

        public async Task<SPCTViewModels> Create(ThemSPCT request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var form = new MultipartFormDataContent();
                form.Add(new StringContent(request.SanPhamId.ToString()), "SanPhamId");
                form.Add(new StringContent(request.Gia.ToString()), "Gia");
                form.Add(new StringContent(request.SoLuong.ToString()), "SoLuong");
                // Thêm các trường khác nếu có...

                var response = await client.PostAsync($"/api/SPCT/Create", form);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<SPCTViewModels>>(body, settings);
                    return apiResult.ResultObj;
                }
                throw new Exception("Tạo sản phẩm chi tiết thất bại");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo sản phẩm chi tiết: {ex.Message}");
            }
        }

        public async Task<SPCTViewModels> Update(SuaSPCT request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var form = new MultipartFormDataContent();
                form.Add(new StringContent(request.Id.ToString()), "Id");
                form.Add(new StringContent(request.MauSacId.ToString()), "MauSacId");
                form.Add(new StringContent(request.KichThuocId.ToString()), "KichThuocId");
                form.Add(new StringContent(request.ChatLieuId.ToString()), "ChatLieuId");
                form.Add(new StringContent(request.DeGiayId.ToString()), "DeGiayId");
                form.Add(new StringContent(request.ThuongHieuId.ToString()), "ThuongHieuId");
                form.Add(new StringContent(request.Gia.ToString()), "Gia");
                form.Add(new StringContent(request.SoLuong.ToString()), "SoLuong");
                form.Add(new StringContent(request.TrangThai.ToString()), "TrangThai");
                if (request.Images != null)
                {
                    foreach (var image in request.Images)
                    {
                        var streamContent = new StreamContent(image.OpenReadStream());
                        form.Add(streamContent, "Images", image.FileName);
                    }
                }

                var response = await client.PutAsync($"/api/SPCT/Edit/{request.Id}", form);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<SPCTViewModels>>(body, settings);
                    return apiResult.ResultObj;
                }
                throw new Exception("Cập nhật sản phẩm chi tiết thất bại");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sản phẩm chi tiết: {ex.Message}");
            }
        }

        public async Task<bool> UpdateGia(Guid id, decimal giaMoi)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(giaMoi);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PatchAsync($"/api/SPCT/{id}/gia", httpContent);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
                    return apiResult?.ResultObj ?? false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật giá: {ex.Message}");
            }
        }

        public async Task<bool> UpdateSoLuong(Guid id, int themSoLuong)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(themSoLuong);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"/api/SPCT/{id}/soluong", httpContent);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
                    return apiResult?.ResultObj ?? false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật số lượng: {ex.Message}");
            }
        }

        public async Task<bool> UpdateTrangThai(Guid id, bool trangThai)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var json = JsonConvert.SerializeObject(trangThai);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PatchAsync($"/api/SPCT/{id}/trangThai", httpContent);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);
                    return apiResult?.ResultObj ?? false;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        public async Task<int> AddImage(Guid id, IFormFile file)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var form = new MultipartFormDataContent();
                var streamContent = new StreamContent(file.OpenReadStream());
                form.Add(streamContent, "file", file.FileName);

                var response = await client.PostAsync($"/api/SPCT/{id}/images", form);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<int>>(body);
                    return apiResult?.ResultObj ?? 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm ảnh: {ex.Message}");
            }
        }

        public async Task<int> RemoveImage(Guid imageId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.DeleteAsync($"/api/SPCT/images/{imageId}");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<int>>(body);
                    return apiResult?.ResultObj ?? 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa ảnh: {ex.Message}");
            }
        }

        public async Task<List<string>> GetListImages(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_configuration["BaseAddress"]);
                var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessions))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

                var response = await client.GetAsync($"/api/SPCT/{id}/images");
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<List<string>>>(body, settings);
                    if (apiResult != null && apiResult.ResultObj != null)
                        return apiResult.ResultObj;

                    // Nếu không phải ApiSuccessResult, thử đọc trực tiếp
                    var list = JsonConvert.DeserializeObject<List<string>>(body, settings);
                    return list ?? new List<string>();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}