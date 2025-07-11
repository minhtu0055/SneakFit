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
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        // Khách hàng
        public Guid? UserId { get; set; }
        public AppUser? User { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        // Người tạo hóa đơn
        public string NguoiTao { get; set; }
        public string? MaHoaDon { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public string? GhiChu { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        public LoaiHoaDon LoaiHoaDon { get; set; }
        public bool? GiaoHang { get; set; }
        public DateTime? NgayThanhToan { get; set; }      
        public Guid? VoucherId { get; set; }
        public Voucher? Voucher { get; set; }
        public List<HoaDonChiTiet>? HoaDonChiTiet { get; set; }
        public List<LichSuHoaDon>? LichSuHoaDon { get; set; }
        public decimal? TienKhachDua { get; set; }
    }
}
