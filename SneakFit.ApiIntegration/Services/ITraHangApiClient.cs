using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface ITraHangApiClient
    {
        Task<ApiResult<Guid>> CreateAsync(CreateReturnRequest request, List<IFormFile>? images = null);
        Task<PagedResult<ReturnViewModel>> GetMyReturnsAsync(int pageIndex, int pageSize);
        Task<PagedResult<ReturnViewModel>> GetMyAsync(Guid userId, int pageIndex, int pageSize);
        Task<ApiSuccessResult<ReturnViewModel>> GetDetailAsync(Guid returnId);
        Task<ReturnViewModel?> GetDetailAsync(Guid id, Guid userId);
        Task<ApiResult<bool>> CancelAsync(Guid returnId);
    }
}
