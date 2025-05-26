using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
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
        Task<bool> UpdateSPCT(Guid id, List<SanPhamChiTietCapNhat> updates);
        Task<List<SPCTViewModels>> GetSPCTByFilter(SPCTFilterRequest request);
        Task<List<SPCTViewModels>> GetSPCTByProductName(string productName);
        Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId);
    }
}
