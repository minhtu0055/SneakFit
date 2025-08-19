using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.TraHang
{
    public interface IReturnService
    {
        Task<ApiResult<Guid>> CreateAsync(CreateReturnRequest request, Guid userId);
        Task<PagedResult<ReturnViewModel>> GetMyAsync(Guid userId, int pageIndex, int pageSize);
        Task<ReturnViewModel?> GetDetailAsync(Guid id, Guid userId);
        Task<ApiResult<bool>> CancelAsync(Guid id, Guid userId);

        // Admin
        Task<ApiResult<bool>> ApproveAsync(Guid id, string? carrier, string? shipCode);
        Task<ApiResult<bool>> ReceiveAsync(Guid id);
        Task<ApiResult<bool>> CompleteAsync(Guid id);
        Task<ApiResult<bool>> RejectAsync(Guid id, string reason);

        Task<PagedResult<ReturnViewModel>> GetAdminPagingAsync(int pageIndex, int pageSize,
            ReturnStatus? status, string? keyword, DateTime? from, DateTime? to);
        Task<ReturnViewModel?> GetAdminDetailAsync(Guid id);
        
        // Methods mới cho chuyển đổi trạng thái với ghi chú
        Task<ApiResult<bool>> UpdateStatusWithLogAsync(Guid id, ReturnStatus newStatus, string ghiChu, string nguoiChinhSua);
        Task<List<ReturnStatusHistoryViewModel>> GetStatusHistoryAsync(Guid returnRequestId);

        // NEW: Kiểm tra đơn hàng đã có yêu cầu trả hàng của user chưa
        Task<bool> HasReturnAsync(Guid orderId, Guid userId);
    }
}
