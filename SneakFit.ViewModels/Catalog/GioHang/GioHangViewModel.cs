using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.GioHang
{
    public class GioHangViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime NgayTao { get; set; }
        public List<GioHangChiTietViewModel> GioHangChiTiets { get; set; }
        public decimal TongTien { get; set; }
    }
}
