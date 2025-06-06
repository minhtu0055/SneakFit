using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class HoaDonChiTietViewModels
    {
        public string MaHoaDon { get; set; }
        public string TrangThai { get; set; }
        public string LoaiDon { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? ThoiGianDuKienNhan { get; set; }
        public List<TrangThaiStep> TrangThaiSteps { get; set; }
        public List<LichSuThanhToanViewModels> LichSuThanhToan { get; set; }
        public List<SanPhamMuaViewModels> SanPhamMua { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public decimal VoucherGiamGia { get; set; }
        public decimal TongTienGiam { get; set; }
        public decimal TongTienThanhToan { get; set; }
    }
}
