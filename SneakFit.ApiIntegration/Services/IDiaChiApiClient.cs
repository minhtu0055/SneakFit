using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;

namespace SneakFit.ApiIntegration.Services
{
    public interface IDiaChiApiClient
    {
        Task<ApiResult<List<DiaChiViewModel>>> GetAllByUserId(Guid userId);
        Task<List<DiaChiViewModel>> GetAllByUser();
        Task<ApiResult<DiaChiViewModel>> GetById(Guid id);
        Task<ApiResult<bool>> Create(ThemDiaChiViewModel request);
        Task<ApiResult<bool>> Update(Guid id, SuaDiaChiViewModel request);
        Task<ApiResult<bool>> Delete(Guid id);
        Task<ApiResult<bool>> SetDefault(Guid id);
    }
} 
