using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IHoaDonChiTietClientApiClient
    {
        Task<PagedResult<HoaDonChiTietClientViewModel>> GetAllPaging(PhanTrangHoaDonChiTietClient request);
        Task<List<HoaDonChiTietClientViewModel>> GetByHoaDonId(Guid id);
        Task<HoaDonChiTietClientViewModel> Create(ThemHoaDonChiTietClient request);
        Task<HoaDonChiTietClientViewModel> Edit(SuaHoaDonChiTietClient request);
    }
}
