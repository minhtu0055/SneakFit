using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class SPCTViewModels
    {
        public Guid Id { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongTon { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid MauSacId { get; set; }
        public Guid KichThuocId { get; set; }
        public Guid ChatLieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid SanPhamId { get; set; }
        public Guid DanhMucId { get; set; }
        public bool TrangThai { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? MaKichThuoc { get; set; }
        public string? TenMauSac { get; set; }
        public string? MaMauSac { get; set; }
        public string? TenChatLieu { get; set; }
        public string? TenDeGiay { get; set; }
        public string? TenThuongHieu { get; set; }


        public decimal GiaGoc { get; set; }
        public decimal GiaKhuyenMai { get; set; }
        public decimal KhuyenMaiPhanTram { get; set; }
        public Guid? KhuyenMaiId { get; set; }

        public List<string>? Images { get; set; }
    }
}
