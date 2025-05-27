using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Common;
using System.Net.Http.Headers;
using System.Text;

namespace SneakFit.ApiIntegration.Services
{
    public class DanhMucApiClient : IDanhMucApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DanhMucApiClient(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DanhMucViewModels> Create(ThemDanhMuc request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/danhmuc/create", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<DanhMucViewModels>(result);

            throw new Exception("Không thể tạo danh mục");
        }

        public async Task<PagedResult<DanhMucViewModels>> GetAllPaging(DanhMucPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/danhmuc/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var danhmuc = JsonConvert.DeserializeObject<PagedResult<DanhMucViewModels>>(body);
            return danhmuc;
        }

        public async Task<DanhMucViewModels> GetById(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/danhmuc/getbyid/{id}");
            var body = await response.Content.ReadAsStringAsync();
            var danhmuc = JsonConvert.DeserializeObject<DanhMucViewModels>(body);
            return danhmuc;
        }

        public async Task<DanhMucViewModels> Update(SuaDanhMuc request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/danhmuc/edit/{request.Id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<DanhMucViewModels>(result);

            throw new Exception("Không thể cập nhật danh mục");
        }

        public async Task<List<DanhMucViewModels>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/danhmuc/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<DanhMucViewModels>>(body);
                return result ?? new List<DanhMucViewModels>();
            }
            throw new Exception("Không thể lấy danh sách danh mục");
        }

        public async Task<ApiResult<bool>> UpdateProductCount(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.PostAsync($"/api/danhmuc/UpdateProductCount/{id}", null);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResult<bool>>(result);

            return new ApiResult<bool>() { IsSuccessed = false, Message = "Không thể cập nhật số lượng sản phẩm" };
        }
    }
}