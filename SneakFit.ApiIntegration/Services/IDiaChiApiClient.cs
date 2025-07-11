using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;

namespace SneakFit.ApiIntegration.Services
{
    public interface IDiaChiApiClient
    {
        Task<ApiResult<List<DiaChiViewModel>>> GetAllByUserId(Guid userId);
    }
}
