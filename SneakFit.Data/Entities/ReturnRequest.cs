using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class ReturnRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid OrderId { get; set; }                 // HoaDon.Id
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public ReturnMethod Method { get; set; }
        public ReturnStatus Status { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? ShippingCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ReturnRequestItem> Items { get; set; } = new();
    }
}
