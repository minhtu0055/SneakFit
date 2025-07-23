using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SneakFit.ViewModels.GHN;

namespace SneakFit.ApiIntegration.Services
{
    public class GhnApiClient : IGhnApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GhnApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetProvinces()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync("/api/ghn/provinces");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDistricts()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync("/api/ghn/districts");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetWards(int districtId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/ghn/wards/{districtId}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CalculateShippingFee(ShippingFeeRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("/api/ghn/shipping-fee", content);
            return await response.Content.ReadAsStringAsync();
        }
        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var token = _configuration["Ghn:Token"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Add("Token", token);
            }
            return client;
        }
        public async Task<string> GetAvailableServicesAsync(AvailableServiceRequest request)
        {
            var client = CreateClient();
            var shopId = _configuration.GetValue<int>("Ghn:ShopId");
            var extendedBody = new
            {
                shop_id = shopId,
                from_district = request.FromDistrict,
                to_district = request.ToDistrict
            };
            var json = JsonSerializer.Serialize(extendedBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/ghn/available-services", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}