using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.SanPham
{
    public class SanPhamPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
        public Guid? DanhMucId { get; set; }
        public bool? TrangThai { get; set; }
    }
}
