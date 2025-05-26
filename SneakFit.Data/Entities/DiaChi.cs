using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class DiaChi
    {
        public Guid Id { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoDienThoai { get; set; }
        public string TenDiaChi { get; set; }
        public string TenThanhPho { get; set; }
        public string TenHuyen { get; set; }     
        public string TenXa { get; set; }
        public bool Mac_Dinh { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
    }
}
