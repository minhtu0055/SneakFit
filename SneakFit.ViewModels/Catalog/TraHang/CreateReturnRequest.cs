using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.TraHang
{
    public class CreateReturnRequest
    {
        public Guid OrderId { get; set; }             // Hóa đơn cần trả
        public string Reason { get; set; } = string.Empty; // Lý do
        public ReturnMethod Method { get; set; }      // 1 = Đổi, 2 = Chuyển khoản

        // Thông tin ngân hàng (bắt buộc khi Method = BankTransfer)
        public BankInfo? Bank { get; set; }

        // Nếu sau này muốn cho phép trả từng dòng chi tiết với số lượng, dùng danh sách này
        public List<ReturnItem> Items { get; set; } = new();
    }

    public class ReturnItem
    {
        public Guid OrderItemId { get; set; } // Id dòng hóa đơn chi tiết
        public int Quantity { get; set; }     // Số lượng muốn trả
    }


    public class BankInfo
    {
        public string AccountName { get; set; } = string.Empty;   // Chủ TK
        public string AccountNumber { get; set; } = string.Empty; // Số TK
        public string BankName { get; set; } = string.Empty;      // Tên Ngân hàng
    }
}
