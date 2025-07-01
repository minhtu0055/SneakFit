using SneakFit.Data.Enums;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.HoaDon
{
    public class PhanTrangHoaDon : PagingRequestBase
    {
        public string? Keyword { get; set; }
        public TrangThaiHoaDon? Trangthaihoadon { get; set; }
        public DateTime? NgayBatDau { get; set; }  // Ngày bắt đầu
        public DateTime? NgayKetThuc { get; set; }  // Ngày kết thúc
    }
}