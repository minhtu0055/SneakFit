using SneakFit.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.Voucher
{
    public class UpdateVoucher
    {
        public Guid Id { get; set; }
        public string MaVoucher { get; set; }
        public LoaiGiamGia LoaiGiamGia { get; set; }
        public decimal GiaTriGiamGia { get; set; }
        public decimal DieuKienApDung { get; set; }
        public DateTime NgayTao { get; set; }
        public int SoLuong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public TrangThaiGiamGia TrangThai { get; set; }
    }
}
