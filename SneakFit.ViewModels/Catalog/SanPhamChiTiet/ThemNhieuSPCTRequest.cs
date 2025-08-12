using System;
using System.Collections.Generic;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using Microsoft.AspNetCore.Http;

namespace SneakFit.ViewModels.Catalog.SanPhamChiTiet
{
    public class ThemNhieuSPCTRequest
    {
        public Guid SanPhamId { get; set; }
        public Guid ThuongHieuId { get; set; }
        public Guid ChatLieuId { get; set; }
        public Guid DeGiayId { get; set; }
        public bool TrangThai { get; set; }
        public List<ThemSPCTItem> Items { get; set; }
        public List<IFormFile>? Images { get; set; } // Thêm trường Images để xử lý ảnh
    }

    public class ThemSPCTItem
    {
        public Guid MauSacId { get; set; }
        public Guid KichThuocId { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
    }
}