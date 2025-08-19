using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Data.Enums
{
    public enum ReturnStatus
    {
        ChapNhanDuyetHangHoan = 1,    // Chấp nhận duyệt hàng hoàn
        LayHangHoan = 2,              // Lấy hàng hoàn
        HoanHang = 3,                 // Hoàn hàng (check hàng hoàn)
        ThanhCong = 4,                // Thành công
        TuChoi = 5                    // Từ chối (giữ lại cho tương thích)
    }
}
