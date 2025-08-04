using Azure.Core.Pipeline;
using SneakFit.Data.Enums;
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
        public TrangThaiHoaDon TrangThaiCu { get; set; } // Trạng thái cũ
        public TrangThaiHoaDon TrangThaiMoi { get; set; } // Trạng thái mới
        public DateTime NgayTao { get; set; } // Ngày chỉnh sửa lịch sử
        public string NguoiChinhSua { get; set; } // Người chỉnh sửa lịch sử
        public string GhiChu { get; set; } // Người chỉnh sửa lịch sử
    }
}
