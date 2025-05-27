using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
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
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/spct/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&tuKhoa={request.TuKhoa}");
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

        public async Task<SPCTViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/spct/GetById/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var spct = JsonConvert.DeserializeObject<SPCTViewModels>(body);
                return spct;
            }
            throw new Exception("Không thể lấy thông tin sản phẩm chi tiết");
        }

        public async Task<ApiResult<SPCTViewModels>> Create(ThemSPCT request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.SanPhamId.ToString()), "SanPhamId");
            form.Add(new StringContent(request.MauSacId.ToString()), "MauSacId");
            form.Add(new StringContent(request.KichThuocId.ToString()), "KichThuocId");
            form.Add(new StringContent(request.ChatLieuId.ToString()), "ChatLieuId");
            form.Add(new StringContent(request.DeGiayId.ToString()), "DeGiayId");
            form.Add(new StringContent(request.ThuongHieuId.ToString()), "ThuongHieuId");
            form.Add(new StringContent(request.Gia.ToString()), "Gia");
            form.Add(new StringContent(request.SoLuong.ToString()), "SoLuong");

            // Thêm nhiều ảnh vào form nếu có
            if (request.Images != null)
            {
                foreach (var image in request.Images)
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    form.Add(streamContent, "Images", image.FileName);
                }
            }

            var response = await client.PostAsync($"/api/spct/Create", form);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
            if (response.IsSuccessStatusCode)
            {
                var spct = JsonConvert.DeserializeObject<SPCTViewModels>(body);
                return new ApiResult<SPCTViewModels>
                {
                    IsSuccessed = true,
                    Message = "Thành công",
                    ResultObj = spct
                };
            }
            // Lấy message lỗi trả về từ API nếu có
            string errorMsg = "Không thể tạo sản phẩm chi tiết";
            try
            {
                dynamic errorObj = JsonConvert.DeserializeObject(body);
                if (errorObj != null && errorObj.message != null)
                    errorMsg = errorObj.message;
            }
            catch { }
            return new ApiResult<SPCTViewModels>
            {
                IsSuccessed = false,
                Message = errorMsg,
                ResultObj = null
            };
        }

        public async Task<SPCTViewModels> Update(SuaSPCT request)
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
            form.Add(new StringContent(request.SanPhamId.ToString()), "SanPhamId");
            form.Add(new StringContent(request.DanhMucId.ToString()), "DanhMucId");

            if (request.Images != null)
            {
                foreach (var image in request.Images)
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    form.Add(streamContent, "Images", image.FileName);
                }
            }

            var response = await client.PutAsync($"/api/spct/Edit/{request.Id}", form);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var spct = JsonConvert.DeserializeObject<SPCTViewModels>(body);
                return spct;
            }
            throw new Exception("Không thể cập nhật sản phẩm chi tiết");
        }

        public async Task<bool> UpdateGia(Guid id, decimal giaMoi)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(giaMoi);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"/api/spct/{id}/gia", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<bool>(body);
                return result;
            }
            throw new Exception("Không thể cập nhật giá");
        }

        public async Task<bool> UpdateSoLuong(Guid id, int themSoLuong)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(themSoLuong);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/spct/{id}/soluong", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<bool>(body);
                return result;
            }
            throw new Exception("Không thể cập nhật số lượng");
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

            var response = await client.PutAsync($"/api/spct/{id}/trangThai", httpContent);
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

        public async Task<int> AddImage(Guid id, IFormFile file)
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
                var result = JsonConvert.DeserializeObject<int>(body);
                return result;
            }
            throw new Exception("Không thể thêm ảnh");
        }

        public async Task<int> RemoveImage(Guid imageId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/spct/images/{imageId}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<int>(body);
                return result;
            }
            throw new Exception("Không thể xóa ảnh");
        }

        public async Task<List<string>> GetListImages(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.GetAsync($"/api/spct/{id}/images");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var images = JsonConvert.DeserializeObject<List<string>>(body);
                return images ?? new List<string>();
            }
            return new List<string>();
        }

        public async Task<List<SPCTViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/spct/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<SPCTViewModels>>(body);
                return result ?? new List<SPCTViewModels>();
            }
            throw new Exception("Không thể lấy danh sách sản phẩm");
        }

        public async Task<int> CreateMultiple(ThemNhieuSPCTRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(sessions))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/spct/CreateMultiple", httpContent);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResult = JsonConvert.DeserializeObject<ApiSuccessResult<int>>(body);
                return apiResult?.ResultObj ?? 0;
            }
            // Đọc lỗi chi tiết trả về từ API
            string errorMsg = "Không thể thêm nhiều sản phẩm chi tiết";
            try
            {
                dynamic errorObj = JsonConvert.DeserializeObject(body);
                if (errorObj != null && errorObj.title != null)
                    errorMsg = errorObj.title;
                else if (errorObj != null && errorObj.message != null)
                    errorMsg = errorObj.message;
                else if (errorObj != null && errorObj.detail != null)
                    errorMsg = errorObj.detail;
            }
            catch { }
            throw new Exception(errorMsg);
        }
    }
}