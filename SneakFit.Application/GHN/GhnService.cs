using Microsoft.Extensions.Configuration;
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
    }
}
