using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.Enums;

namespace SneakFit.Data.Entities
{
    public class HoaDon
    {
        public Guid Id { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string GhiChu { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public Guid VoucherID { get; set; }
        public Voucher Voucher { get; set; }
        public List<HoaDonChiTiet> HoaDonChiTiet { get; set; }


    }
}
