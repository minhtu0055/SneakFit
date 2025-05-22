using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IKhuyenMaiApiClient
    {
        Task<PagedResult<KhuyenMaiViewModels>> GetAllPaging(PhanTrangKhuyenMai request);
        Task<KhuyenMaiViewModels> GetById(Guid id);
        Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request);
        Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request);
        Task<bool> UpdateStatus(Guid id, TrangThaiGiamGia trangThai);
    }
}
