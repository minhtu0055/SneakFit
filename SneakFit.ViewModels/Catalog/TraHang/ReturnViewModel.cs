using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.TraHang
{
    public class ReturnViewModel
    {
        public Guid ReturnId { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public string MaHoaDon { get; set; } = string.Empty;
        public ReturnStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public ReturnMethod Method { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? ShippingCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Thêm thông tin chi tiết đơn hàng
        public List<ReturnOrderDetailViewModel> OrderDetails { get; set; } = new List<ReturnOrderDetailViewModel>();
    }

    public class ReturnOrderDetailViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
