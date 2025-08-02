using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonClient
{
    public interface IHoaDonClientService
    {
        Task<PagedResult<HoaDonClientViewModel>> GetAllPaging(PhanTrangHoaDonClient request, Guid? userId = null);
        Task<HoaDonClientViewModel> GetById(Guid id);
        Task<HoaDonClientViewModel> Create(ThemHoaDonClient request);
        Task<HoaDonClientViewModel> Update(SuaHoaDonClient request);
        Task<Dictionary<TrangThaiHoaDon, int>> GetCountByStatusAsync();
        Task<bool> UpdateStatus(Guid id, SneakFit.Data.Enums.TrangThaiHoaDon newStatus);
        Task<bool> UpdatePaymentStatus(Guid id, SneakFit.Data.Enums.TrangThaiThanhToan newPaymentStatus);
        
        // Các method mới để xử lý hủy hóa đơn và hoàn lại số lượng
        // Hủy hóa đơn với hoàn lại số lượng
        Task<ApiResult<bool>> CancelOrderWithRollback(Guid id);

        // Trả hàng với hoàn lại số lượng  
        Task<ApiResult<bool>> ReturnOrderWithRollback(Guid id);

        // Hủy hóa đơn khi thanh toán thất bại
        Task<ApiResult<bool>> CancelOrderOnPaymentFailure(Guid id);
    }
}
