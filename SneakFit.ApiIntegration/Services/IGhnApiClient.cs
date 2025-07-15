using System.Collections.Generic;
using System.Threading.Tasks;
using SneakFit.ViewModels.GHN;

namespace SneakFit.ApiIntegration.Services
{
    public interface IGhnApiClient
    {
        Task<string> GetProvinces();
        Task<string> GetDistricts();
        Task<string> GetWards(int districtId);
        Task<string> CalculateShippingFee(ShippingFeeRequest request);
        Task<string> GetAvailableServices(AvailableServiceRequest request);
    }
}