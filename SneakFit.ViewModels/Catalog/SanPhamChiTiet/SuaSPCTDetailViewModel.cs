namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class SuaSPCTDetailViewModel
    {
        public Guid Id { get; set; }
        public string TenSanPham { get; set; }
        public string MoTa { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid ChatLieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public Guid KichThuocId { get; set; }
        public Guid MauSacId { get; set; }
        public bool TrangThai { get; set; }
        public int SoLuong { get; set; }
        public int? SoLuongHangTra { get; set; }
        public decimal GiaBan { get; set; }
    }
}
