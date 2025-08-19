using SneakFit.Data.Enums;
using Microsoft.AspNetCore.Html;
using System.Text;

namespace SneakFit.WebClient.Helpers
{
    public static class ViewHelpers
    {
        public static IHtmlContent GetReturnStatusText(ReturnStatus status)
        {
            string text = status switch
            {
                ReturnStatus.ChapNhanDuyetHangHoan => "Chờ duyệt",
                ReturnStatus.LayHangHoan => "Lấy hàng hoàn",
                ReturnStatus.HoanHang => "Hoàn hàng (check hàng hoàn)",
                ReturnStatus.ThanhCong => "Thành công",
                ReturnStatus.TuChoi => "Từ chối",
                _ => "Không xác định",
            };
            return new HtmlString(text);
        }

        public static IHtmlContent GetReturnStatusColor(ReturnStatus status)
        {
            string color = status switch
            {
                ReturnStatus.ChapNhanDuyetHangHoan => "warning",
                ReturnStatus.LayHangHoan => "info",
                ReturnStatus.HoanHang => "primary",
                ReturnStatus.ThanhCong => "success",
                ReturnStatus.TuChoi => "danger",
                _ => "secondary",
            };
            return new HtmlString(color);
        }
    }
}
