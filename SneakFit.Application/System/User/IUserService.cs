using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.System.User
{
    public interface IUserService
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);
        Task<ApiResult<bool>> Register(RegisterRequest request);
        Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request);
        Task<ApiResult<UserViewModels>> GetById(Guid id);
        Task<bool> TrangThai(Guid id, bool trangThai);
        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
        Task<ApiResult<bool>> Update(UserUpdateRequest request);
        Task<ApiResult<bool>> QuenMatKhau(string email);
        Task<ApiResult<bool>> DoiMatKhau(Guid id, DoiMatKhauRequest request);
    }
}
