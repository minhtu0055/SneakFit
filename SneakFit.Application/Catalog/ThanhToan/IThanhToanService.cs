using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ThanhToan
{
    public interface IThanhToanService
    {
        Task<string> CreateMomoPaymentUrl(MomoPaymentRequest request);
        Task<string> CreateVnPayPaymentUrl(VNPayPaymentRequest request);
        Task<bool> XuLyVnPayCallbackAsync(Dictionary<string, string> vnp_Params);
    }
}
