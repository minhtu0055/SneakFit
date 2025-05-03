using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.Common;

namespace SneakFit.ViewModels.System.User
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? TuKhoa { get; set; }
    }
}
