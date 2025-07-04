namespace SneakFit.WebClient.Models
{
    public class GioHangItemViewModel
    {
        public Guid SanPhamChiTietId { get; set; } 
        public string TenSanPham { get; set; }
        public string AnhSanPham { get; set; }
        public string MauSac { get; set; }
        public string KichThuoc { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaKhuyenMai { get; set; }
        public int PhanTramGiamGia { get; set; }
        public int SoLuong { get; set; }
        public int? SoLuongTon { get; set; }
        public decimal ThanhTien => (GiaKhuyenMai > 0 && GiaKhuyenMai < GiaGoc ? GiaKhuyenMai : GiaGoc) * SoLuong;
    }
}
