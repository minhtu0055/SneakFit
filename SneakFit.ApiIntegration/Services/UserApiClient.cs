using Newtonsoft.Json;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.User;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/user/authenticate", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }
        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            // Sử dụng MultipartFormDataContent thay vì StringContent để xử lý tải lên hình ảnh
            var form = new MultipartFormDataContent();

            // Thêm các trường thông tin cơ bản
            form.Add(new StringContent(request.HoVaTen), "HoVaTen");
            form.Add(new StringContent(request.GioiTinh.ToString()), "GioiTinh");
            form.Add(new StringContent(request.UserName), "UserName");
            form.Add(new StringContent(request.NgaySinh.ToString("yyyy-MM-dd")), "NgaySinh");
            form.Add(new StringContent(request.Email), "Email");
            form.Add(new StringContent(request.TrangThai.ToString()), "TrangThai");
            form.Add(new StringContent(request.SoDienThoai), "SoDienThoai");

            // Thêm thông tin địa chỉ
            if (request.DiaChi != null)
            {
                // Thêm thông tin địa chỉ
                if (request.DiaChi != null)
                {
                    if (request.DiaChi.TenNguoiNhan != null)
                        form.Add(new StringContent(request.DiaChi.TenNguoiNhan), "DiaChi.TenNguoiNhan");
                    if (request.DiaChi.SoDienThoai != null)
                        form.Add(new StringContent(request.DiaChi.SoDienThoai), "DiaChi.SoDienThoai");
                    if (request.DiaChi.TenDiaChi != null)
                        form.Add(new StringContent(request.DiaChi.TenDiaChi), "DiaChi.TenDiaChi");
                    if (request.DiaChi.TenThanhPho != null)
                        form.Add(new StringContent(request.DiaChi.TenThanhPho), "DiaChi.TenThanhPho");
                    if (request.DiaChi.TenHuyen != null)
                        form.Add(new StringContent(request.DiaChi.TenHuyen), "DiaChi.TenHuyen");
                    if (request.DiaChi.TenXa != null)
                        form.Add(new StringContent(request.DiaChi.TenXa), "DiaChi.TenXa");
                    form.Add(new StringContent(request.DiaChi.MacDinh.ToString()), "DiaChi.MacDinh");
                    if (request.DiaChi.MaTinh != null)
                        form.Add(new StringContent(request.DiaChi.MaTinh), "DiaChi.MaTinh");
                    if (request.DiaChi.MaHuyen != null)
                        form.Add(new StringContent(request.DiaChi.MaHuyen), "DiaChi.MaHuyen");
                    if (request.DiaChi.MaXa != null)
                        form.Add(new StringContent(request.DiaChi.MaXa), "DiaChi.MaXa");
                }
            }

            // Thêm danh sách roles nếu có
            if (request.Roles != null && request.Roles.Count > 0)
            {
                for (int i = 0; i < request.Roles.Count; i++)
                {
                    form.Add(new StringContent(request.Roles[i]), $"Roles[{i}]");
                }
            }

            // Thêm hình ảnh nếu có
            if (request.HinhAnh != null)
            {
                var streamContent = new StreamContent(request.HinhAnh.OpenReadStream());
                form.Add(streamContent, "HinhAnh", request.HinhAnh.FileName);
            }

            var response = await client.PostAsync($"/api/user/register", form);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/user/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&tukhoa={request.TuKhoa}&role={request.Role}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserViewModels>>>(body);
            return users;
        }
        public async Task<ApiResult<UserViewModels>> GetById(Guid id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token"); // lấy chuỗi session từ token
            var client = _httpClientFactory.CreateClient(sessions);
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions); // thiết lập header bearer thường được dùng với JWT
            //Gửi yều cầu get đến api 
            var response = await client.GetAsync($"/api/user/{id}"); // await đảm bảo gọi API bất đồng bộ, không làm chặn chương trình
            var body = await response.Content.ReadAsStringAsync(); // đọc nội dung phần hồi từ API dưới dạng Json
            //Kiểm tra phản hồi
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModels>>(body);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModels>>(body);
        }

        public async Task<bool> TrangThai(Guid id, bool trangThai)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(trangThai);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/User/{id}/trangthai", httpContent);
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
        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/user/{id}/role", httpContent);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<ApiResult<bool>> Update(UserUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var multipartContent = new MultipartFormDataContent();

            // Thêm các trường thông tin cơ bản
            multipartContent.Add(new StringContent(request.Id.ToString()), "Id");
            multipartContent.Add(new StringContent(request.HoVaTen ?? ""), "HoVaTen");
            multipartContent.Add(new StringContent(request.Email ?? ""), "Email");
            multipartContent.Add(new StringContent(request.SoDienThoai ?? ""), "SoDienThoai");
            multipartContent.Add(new StringContent(request.NgaySinh.ToString("yyyy-MM-ddTHH:mm:ss")), "NgaySinh");
            multipartContent.Add(new StringContent(request.GioiTinh.ToString()), "GioiTinh");
            multipartContent.Add(new StringContent(request.TrangThai.ToString()), "TrangThai");

            // Thêm file hình ảnh nếu có
            if (request.HinhAnh != null)
            {
                var fileContent = new StreamContent(request.HinhAnh.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.HinhAnh.ContentType);
                multipartContent.Add(fileContent, "HinhAnh", request.HinhAnh.FileName);
            }

            // Thêm thông tin địa chỉ nếu có
            if (request.DiaChi != null)
            {
                multipartContent.Add(new StringContent(request.DiaChi.TenDiaChi ?? ""), "DiaChi.TenDiaChi");
                multipartContent.Add(new StringContent(request.DiaChi.TenThanhPho ?? ""), "DiaChi.TenThanhPho");
                multipartContent.Add(new StringContent(request.DiaChi.TenHuyen ?? ""), "DiaChi.TenHuyen");
                multipartContent.Add(new StringContent(request.DiaChi.TenXa ?? ""), "DiaChi.TenXa");
                multipartContent.Add(new StringContent(request.DiaChi.SoDienThoai ?? ""), "DiaChi.SoDienThoai");
                multipartContent.Add(new StringContent(request.DiaChi.TenNguoiNhan ?? ""), "DiaChi.TenNguoiNhan");
                multipartContent.Add(new StringContent(request.DiaChi.MaTinh ?? ""), "DiaChi.MaTinh");
                multipartContent.Add(new StringContent(request.DiaChi.MaHuyen ?? ""), "DiaChi.MaHuyen");
                multipartContent.Add(new StringContent(request.DiaChi.MaXa ?? ""), "DiaChi.MaXa");
            }

            var response = await client.PutAsync($"/api/user/{request.Id}", multipartContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
    }
}
