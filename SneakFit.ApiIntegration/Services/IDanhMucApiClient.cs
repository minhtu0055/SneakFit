using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IDanhMucApiClient
    {
        Task<PagedResult<DanhMucViewModels>> GetAllPaging(DanhMucPagingRequest request);
        Task<DanhMucViewModels> GetById(Guid id);
        Task<DanhMucViewModels> Create(ThemDanhMuc request);
        Task<DanhMucViewModels> Update(SuaDanhMuc request);
        Task<List<DanhMucViewModels>> GetAll();
        Task<ApiResult<bool>> UpdateProductCount(Guid id);
    }
}