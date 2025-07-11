using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ThanhToan
{
    public class VNPayPaymentRequest
    {
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
