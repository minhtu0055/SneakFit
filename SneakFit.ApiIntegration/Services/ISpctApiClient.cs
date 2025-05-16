using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface ISpctApiClient
    {
        Task<List<SPCTViewModels>> GetAll();
        Task<SPCTViewModels> GetById(Guid id);
        Task<SPCTViewModels> Create(ThemSPCT request);
        Task<SPCTViewModels> Update(SuaSPCT request);
        Task<PagedResult<SPCTViewModels>> GetAllPaging(PhanTrangSPCT request);
        Task<bool> UpdateTrangThai(Guid id, bool trangThai);
        Task<bool> UpdateGia(Guid id, decimal giaMoi);
        Task<bool> UpdateSoLuong(Guid productId, int themSoLuong);
        Task<int> AddImage(Guid idSanPham, IFormFile image);
        Task<int> RemoveImage(Guid imageId);
        Task<List<string>> GetListImages(Guid idSanPham);
    }
}
