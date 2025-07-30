using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SneakFit.ViewModels.Catalog.ThongKe;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SneakFit.ApiIntegration.Services
{
    public class ThongKeApiClient : IThongKeApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ThongKeApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<TopSanPhamBanChayViewModel>> GetTopSanPhamBanChayAsync(int top = 10)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/ThongKe/top-san-pham-ban-chay?top={top}");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TopSanPhamBanChayViewModel>>(body);
        }

        public async Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietBanChayThongKeAsync(Guid sanPhamId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/ThongKe/chi-tiet-ban-chay?sanPhamId={sanPhamId}");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SanPhamChiTietThongKeViewModel>>(body);
        }
    }
} 