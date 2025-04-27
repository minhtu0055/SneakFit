using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System;

namespace SneakFit.Application.System
{
    public interface IUserService
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);
        Task<ApiResult<bool>> Register(RegisterRequest request);
        Task<ApiResult<PagedResult<UserViewModels>>> GetUsersPaging(GetUserPagingRequest request);
        Task<ApiResult<UserViewModels>> GetById(Guid id);
        Task<ApiResult<bool>> TrangThai(Guid id, bool trangThai);

    }
}
