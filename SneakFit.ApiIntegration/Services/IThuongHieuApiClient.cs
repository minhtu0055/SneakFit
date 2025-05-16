using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IThuongHieuApiClient
    {
        Task<PagedResult<ThuongHieuViewModels>> GetAllPaging(ThuongHieuPagingRequest request);
        Task<List<ThuongHieuViewModels>> GetAll();
        Task<ThuongHieuViewModels> GetById(Guid id);
        Task<ThuongHieuViewModels> Create(ThemThuongHieu request);
        Task<ThuongHieuViewModels> Update(SuaThuongHieu request);
    }
}
