using System;
using System.Collections.Generic;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class ThemNhieuSPCTRequest
    {
        public Guid SanPhamId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public Guid ChatLieuId { get; set; }
        public bool TrangThai { get; set; }
        public List<SPCTItem> Items { get; set; }
    }
    public class SPCTItem
    {
        public Guid MauSacId { get; set; }
        public Guid KichThuocId { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }

    }
} 