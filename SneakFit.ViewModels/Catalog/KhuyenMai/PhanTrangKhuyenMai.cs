using SneakFit.Data.Enums;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.KhuyenMai
{
    public class PhanTrangKhuyenMai : PagingRequestBase
    {
        public string? Keyword { get; set; }
        public TrangThaiGiamGia? TrangThai { get; set; } 
    }

}
