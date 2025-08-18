using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IAdminReturnsApiClient
    {
        Task<PagedResult<ReturnViewModel>> GetPagingAsync(int pageIndex, int pageSize,
            int? status = null, string? keyword = null, DateTime? from = null, DateTime? to = null);

        Task<ReturnViewModel?> GetDetailAsync(Guid id);

        Task<bool> ApproveAsync(Guid id, string? carrier, string? shipCode);
        Task<bool> ReceiveAsync(Guid id);
        Task<bool> CompleteAsync(Guid id);
        Task<bool> RejectAsync(Guid id, string reason);
        
        // Methods mới cho chuyển đổi trạng thái với ghi chú
        Task<bool> UpdateStatusWithLogAsync(Guid id, int newStatus, string ghiChu, string nguoiChinhSua);
        Task<List<ReturnStatusHistoryViewModel>> GetStatusHistoryAsync(Guid id);
    }
}
