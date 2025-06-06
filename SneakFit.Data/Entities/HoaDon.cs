using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.Enums;

namespace SneakFit.Data.Entities
{
    public class HoaDon
    {
        public Guid Id { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string GhiChu { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        // Thêm các trường mới
        public LoaiHoaDon LoaiHoaDon { get; set; } // Phân biệt đơn online và tại quầy
        public DateTime? NgayThanhToan { get; set; } // Thời gian thanh toán
        public string MaGiaoDich { get; set; } // Mã giao dịch thanh toán (đặc biệt cho VnPay)
        public decimal PhiVanChuyen { get; set; } // Phí vận chuyển
        public string DonViVanChuyen { get; set; } // Đơn vị vận chuyển
        public string MaVanDon { get; set; } // Mã vận đơn
        public TrangThaiThanhToan TrangThaiThanhToan { get; set; } // Trạng thái thanh toán
        public Guid VoucherID { get; set; }
        public Voucher Voucher { get; set; }
        public List<HoaDonChiTiet> HoaDonChiTiet { get; set; }


    }
}
