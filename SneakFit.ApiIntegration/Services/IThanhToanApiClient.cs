using System.Collections.Generic;
using System.Threading.Tasks;
using SneakFit.Application.Catalog.ThanhToan;

namespace SneakFit.ApiIntegration.Services
{
    public interface IThanhToanApiClient
    {
        Task<string> CreateMomoPaymentUrl(MomoPaymentRequest request);
        Task<string> CreateVnPayPaymentUrl(VNPayPaymentRequest request);
        Task<bool> XuLyVnPayCallbackAsync(Dictionary<string, string> vnp_Params);
        // Client methods
        Task<string> CreateVnPayPaymentUrlClient(VNPayPaymentRequest request);
        Task<bool> XuLyVnPayCallBackClientAsync(Dictionary<string, string> vnp_Params);
    }
} 