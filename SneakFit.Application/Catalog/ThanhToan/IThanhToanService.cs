using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ThanhToan
{
    public interface IThanhToanService
    {
        string CreateVNPayPaymentUrl(VNPayPaymentRequest request);
        Task<string> CreateMomoPaymentUrl(MomoPaymentRequest request);
    }
}
