using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface ISanPhamApiClient
    {
        Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request);
        Task<SanPhamViewModels> GetById(Guid id);
        Task<SanPhamViewModels> Create(ThemSanPham request);
        Task<SanPhamViewModels> Update(SuaSanPham request);
        Task<List<SanPhamViewModels>> GetAll();
        Task<bool> UpdateTrangThai(Guid id, bool trangThai);

    }
}
