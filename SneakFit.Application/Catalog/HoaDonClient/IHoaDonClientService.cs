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
    }
}
