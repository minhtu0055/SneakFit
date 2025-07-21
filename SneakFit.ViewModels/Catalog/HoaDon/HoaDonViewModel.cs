using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class HoaDonViewModel
    {
        public Guid Id { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        public Guid? UserId { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string? GhiChu { get; set; }
        public bool? GiaoHang { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        public LoaiHoaDon LoaiHoaDon { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string MaHoaDon { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public string DonViVanChuyen { get; set; }
        public TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public List<HoaDonChiTietViewModel> HoaDonChiTiet { get; set; }
        public Guid? VoucherId { get; set; }
        public decimal? TienKhachDua { get; set; }
        public string MaTinh { get; set; }
        public string TenTinh { get; set; }
        public string MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public string MaXa { get; set; }
        public string TenXa { get; set; }
        public string TenDiaChi { get; set; }
    }
}
