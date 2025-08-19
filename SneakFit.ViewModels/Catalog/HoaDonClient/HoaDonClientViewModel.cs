using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDonClient
{
    public class HoaDonClientViewModel
    {
        public Guid Id { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        public LoaiHoaDon LoaiHoaDon { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string MaHoaDon { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public string DonViVanChuyen { get; set; }
        public TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public List<HoaDonChiTietViewModel> HoaDonChiTiet { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? UserId { get; set; }
        public string GhiChu { get; set; }
        public decimal? VoucherDiscount { get; set; } // Số tiền giảm từ voucher
        // Sửa lại: property này để service set đúng tổng tiền sản phẩm (đã áp dụng khuyến mãi, chưa trừ voucher, chưa cộng phí ship)
        public decimal TongTienSanPham { get; set; }
        // NEW: cờ để ẩn/disable nút yêu cầu trả hàng/hoàn tiền
        public bool HasReturnRequest { get; set; }
    }
}
