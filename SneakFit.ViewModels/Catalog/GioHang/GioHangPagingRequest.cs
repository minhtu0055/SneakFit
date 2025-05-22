using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.GioHang
{
    public class GioHangPagingRequest : PagingRequestBase
    {
        public Guid? UserId { get; set; }
    }
}
