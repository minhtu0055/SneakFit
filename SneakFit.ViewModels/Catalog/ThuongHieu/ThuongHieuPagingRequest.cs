using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.ThuongHieu
{
    public class ThuongHieuPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
