using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.LichSuHoaDon;
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
        Task<List<HoaDonViewModel>> GetHoaDonChoByNguoiTao(string nguoiTao);
        Task<bool> Delete(Guid id);
        Task<bool> ThanhToan(SuaHoaDon request);

        Task<List<LichSuHoaDonViewModel>> GetHistoryByHoaDonId(Guid hoaDonId);
        Task<bool> RevertToPreviousStatus(Guid hoaDonId);
        Task<Guid> CreateHistory(CreateLichSuHoaDonRequest request);
        Task<bool> UpdateStatusAndLogAsync(Guid hoaDonId, TrangThaiHoaDon newStatus, Guid userId, string nguoiChinhSua);
    }
}
