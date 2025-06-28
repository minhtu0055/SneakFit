using Azure.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class LichSuHoaDon
    {
        public Guid Id { get; set; }
        public Guid HoaDonId { get; set; }
        public HoaDon HoaDon { get; set; } // Tham chiếu đến hóa đơn
        public Guid UserId { get; set; }
        public int TrangThaiCu { get; set; } // Trạng thái cũ
        public int TrangThaiMoi { get; set; } // Trạng thái mới
        public DateTime NgayTao { get; set; } // Ngày tạo lịch sử
        public string NguoiTao { get; set; } // Người tạo lịch sử
        public DateTime NgayChinhSua { get; set; } // Ngày chỉnh sửa lịch sử
        public string NguoiChinhSua { get; set; } // Người chỉnh sửa lịch sử
    }
}
