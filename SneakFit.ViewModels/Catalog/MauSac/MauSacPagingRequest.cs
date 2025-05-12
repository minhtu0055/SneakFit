using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.ViewModels.Catalog.MauSac
{
    public class MauSacPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
