using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IHoaDonChiTietApiClient
    {
        Task<PagedResult<HoaDonChiTietViewModel>> GetAllPaging(PhanTrangHoaDonChiTiet request);
        Task<List<HoaDonChiTietViewModel>> GetByHoaDonId(Guid id);
        Task<HoaDonChiTietViewModel> Create(ThemHoaDonChiTiet request);
        Task<HoaDonChiTietViewModel> Edit(SuaHoaDonChiTiet request);
        Task<HoaDonChiTietViewModel> CreateOrUpdate(ThemHoaDonChiTiet request);
        Task<bool> Delete(Guid id);
        Task<bool> UpdateQuantity(Guid hoaDonChiTietId, int newQuantity);
    }
}
