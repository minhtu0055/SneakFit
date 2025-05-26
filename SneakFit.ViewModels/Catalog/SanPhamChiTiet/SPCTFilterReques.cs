using System;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class SPCTFilterRequest
    {
        public Guid? SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public Guid? ThuongHieuId { get; set; }
        public string TuKhoa { get; set; }
        public Guid? ChatLieuId { get; set; }
        public Guid? DeGiayId { get; set; }
        public Guid? KichThuocId { get; set; }
        public Guid? MauSacId { get; set; }
        public string TrangThai { get; set; }
        public decimal? GiaTu { get; set; }
        public decimal? GiaDen { get; set; }
    }
}