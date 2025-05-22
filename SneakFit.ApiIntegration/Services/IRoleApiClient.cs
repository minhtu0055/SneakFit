using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.Role;

namespace SneakFit.ApiIntegration.Services
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleViewModel>>> GetAll();
    }
}
