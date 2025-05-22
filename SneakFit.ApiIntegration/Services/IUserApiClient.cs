using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.User;

namespace SneakFit.ApiIntegration.Services
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);
        Task<ApiResult<bool>> Register(RegisterRequest request);
        Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request);
        Task<ApiResult<UserViewModels>> GetById(Guid id);
        Task<bool> TrangThai(Guid id, bool trangThai);
        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
    }
}
