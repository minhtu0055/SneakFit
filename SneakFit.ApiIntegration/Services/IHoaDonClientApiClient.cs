using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IHoaDonClientApiClient
    {
        Task<PagedResult<HoaDonClientViewModel>> GetAllPaging(PhanTrangHoaDonClient request);
        Task<HoaDonClientViewModel> GetById(Guid id);
        Task<HoaDonClientViewModel> Create(ThemHoaDonClient request);
        Task<HoaDonClientViewModel> Update(SuaHoaDonClient request);
        Task<bool> UpdateStatus(Guid id, TrangThaiHoaDon trangThai);
        Task<Dictionary<string, int>> GetCountByStatusAsync();
    }
}
