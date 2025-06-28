using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IHoaDonApiClient
    {
        Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request);
        Task<HoaDonViewModel> GetById(Guid id);
        Task<HoaDonViewModel> Create(ThemHoaDon request);
        Task<HoaDonViewModel> Update(SuaHoaDon request);
        Task<bool> UpdateStatus(Guid id, TrangThaiHoaDon trangThai);
        Task<Dictionary<string, int>> GetCountByStatusAsync();
    }
}
