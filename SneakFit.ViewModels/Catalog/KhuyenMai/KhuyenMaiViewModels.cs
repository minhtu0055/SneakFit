using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.Enums;


namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class KhuyenMaiViewModels
    {
        public Guid Id { get; set; }
        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public LoaiGiamGia LoaiGiamGia { get; set; }
        public decimal GiaTriGiamGia { get; set; }
        public TrangThaiGiamGia TrangThai { get; set; }
        public List<Guid> SanPhamIds { get; set; }
        public List<KhuyenMaiSanPhamViewModels> SanPhams { get; set; }
    }
}
