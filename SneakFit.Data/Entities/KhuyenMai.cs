using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.Enums;

namespace SneakFit.Data.Entities
{
    public class KhuyenMai
    {
        public Guid Id { get; set; }
        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; }   
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public LoaiGiamGia LoaiGiamGia { get; set; }
        public decimal GiaTriGiamGia { get; set; }
        public TrangThaiGiamGia TrangThai { get; set; }
        public List<KhuyenMaiChiTiet> KhuyenMaiChiTiet { get; set; }
    }
}
