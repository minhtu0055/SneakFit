using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.LichSuHoaDon
{
    public class UpdateHoaDonStatusRequest
    {
        public TrangThaiHoaDon NewStatus { get; set; }
        public Guid UserId { get; set; }
        public string? NguoiChinhSua { get; set; }
        public string GhiChu { get; set; } // Ghi chú bắt buộc khi đổi trạng thái
    }
}
