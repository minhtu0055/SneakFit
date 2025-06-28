using System.Collections.Generic;
using System.Threading.Tasks;

namespace SneakFit.ApiIntegration.Services
{
    public interface IGhnApiClient
    {
        Task<string> GetProvinces();
        Task<string> GetDistricts();
        Task<string> GetWards(int districtId);
    }
}