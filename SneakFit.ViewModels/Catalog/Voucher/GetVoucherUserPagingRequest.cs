using SneakFit.ViewModels.Common;

namespace SneakFit.ViewModels.Catalog.Voucher
{
    public class GetVoucherUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
} 