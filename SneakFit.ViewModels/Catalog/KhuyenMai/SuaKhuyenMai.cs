using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class SuaKhuyenMai
    {
        public Guid Id { get; set; }
        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public LoaiGiamGia LoaiGiamGia { get; set; }
        public decimal GiaTriGiamGia { get; set; }
        public TrangThaiGiamGia TrangThai { get; set; }
        public List<Guid> SanPhamIds { get; set; }

        public List<SPCTViewModels> SelectedProductDetails { get; set; }
    }
}
