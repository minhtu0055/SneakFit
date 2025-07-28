using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.LichSuHoaDon
{
    public class CreateLichSuHoaDonRequest
    {
        public Guid HoaDonId { get; set; }
        public Guid UserId { get; set; }
        public TrangThaiHoaDon TrangThaiCu { get; set; } // Trạng thái cũ
        public TrangThaiHoaDon TrangThaiMoi { get; set; } // Trạng thái mới
        public DateTime NgayTao { get; set; } // Ngày tạo lịch sử
        public string NguoiChinhSua { get; set; } // Người chỉnh sửa lịch sử
    }
}
