using SneakFit.ViewModels.GHN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.GHN
{
    public interface IGhnService
    {
        Task<string> GetProvincesAsync();
        Task<string> GetDistrictsAsync();
        Task<string> GetWardsAsync(int districtId);
        Task<string> CalculateShippingFeeAsync(ShippingFeeRequest request);
    }
}
