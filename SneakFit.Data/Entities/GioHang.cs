using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Entities
{
    public class GioHang
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime NgayTao { get; set; }
        public AppUser User { get; set; }
        public List<GioHangChiTiet> GioHangChiTiet { get; set; }
    }
}
