using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IHoaDonClientApiClient
    {
        Task<PagedResult<HoaDonClientViewModel>> GetAllPaging(PhanTrangHoaDonClient request, Guid? userId = null);
        Task<HoaDonClientViewModel> GetById(Guid id);
        Task<HoaDonClientViewModel> Create(ThemHoaDonClient request);
        Task<HoaDonClientViewModel> Update(SuaHoaDonClient request);
        Task<bool> UpdateStatus(Guid id, SneakFit.Data.Enums.TrangThaiHoaDon newStatus);
        Task<bool> UpdatePaymentStatus(Guid id, SneakFit.Data.Enums.TrangThaiThanhToan newPaymentStatus);
        Task<Dictionary<string, int>> GetCountByStatusAsync();
        
        // Các method mới để xử lý hủy hóa đơn và hoàn lại số lượng
        Task<ApiResult<bool>> CancelOrderWithRollback(Guid id);
        Task<ApiResult<bool>> ReturnOrderWithRollback(Guid id);
        Task<ApiResult<bool>> CancelOrderOnPaymentFailure(Guid id);
    }
}
