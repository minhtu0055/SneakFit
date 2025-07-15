using Microsoft.Extensions.Configuration;
using SneakFit.ViewModels.GHN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SneakFit.Application.GHN
{
    public class GhnService : IGhnService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GhnService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Add("Token", _configuration["GhnSettings:Token"]);
        }

        public async Task<string> GetProvincesAsync()
        {
            var url = $"{_configuration["GhnSettings:BaseUrl"]}{_configuration["GhnSettings:Endpoints:Provinces"]}";
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDistrictsAsync()
        {
            var url = $"{_configuration["GhnSettings:BaseUrl"]}{_configuration["GhnSettings:Endpoints:Districts"]}";
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetWardsAsync(int districtId)
        {
            var url = $"{_configuration["GhnSettings:BaseUrl"]}{_configuration["GhnSettings:Endpoints:Wards"]}";
            var content = new StringContent(JsonSerializer.Serialize(new { district_id = districtId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> CalculateShippingFeeAsync(ShippingFeeRequest request)
        {
            var url = $"{_configuration["GhnSettings:BaseUrl"]}v2/shipping-order/fee";

            var requestBody = new
            {
                from_district_id = request.FromDistrictId,
                service_id = request.ServiceId,
                to_district_id = request.ToDistrictId,
                to_ward_code = request.ToWardCode,
                weight = request.Weight,
                length = request.Length,
                width = request.Width,
                height = request.Height
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            // Lấy nội dung response
            var responseContent = await response.Content.ReadAsStringAsync();

            // Xử lý response nếu có lỗi
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GHN API lỗi ({response.StatusCode}): {responseContent}");
            }

            return responseContent;
        }

        public async Task<string> GetAvailableServicesAsync(int fromDistrict, int toDistrict)
        {
            var url = $"{_configuration["GhnSettings:BaseUrl"]}v2/shipping-order/available-services";
            var requestBody = new
            {
                shop_id = int.Parse(_configuration["GhnSettings:ShopId"]),
                from_district = fromDistrict,
                to_district = toDistrict
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"GHN API lỗi ({response.StatusCode}): {responseContent}");
            }
            return responseContent;
        }
    }
}
