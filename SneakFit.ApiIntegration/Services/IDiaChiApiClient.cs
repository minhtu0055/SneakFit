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
        
        // Các phương thức mới theo pattern by-user/{userId}
        Task<ApiResult<DiaChiViewModel>> GetByIdByUser(Guid userId, Guid id);
        Task<ApiResult<bool>> CreateByUser(Guid userId, ThemDiaChiViewModel request);
        Task<ApiResult<bool>> UpdateByUser(Guid userId, Guid id, SuaDiaChiViewModel request);
        Task<ApiResult<bool>> DeleteByUser(Guid userId, Guid id);
        Task<ApiResult<bool>> SetDefaultByUser(Guid userId, Guid id);
    }
} 
