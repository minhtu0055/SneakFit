using SneakFit.ViewModels.Common;
using SneakFit.ViewModels.System.DiaChi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.System.DiaChi
{
    public interface IDiaChiService
    {
        Task<ApiResult<List<DiaChiViewModel>>> GetAllByUser(Guid userId);
        Task<ApiResult<DiaChiViewModel>> GetById(Guid id, Guid userId);
        Task<ApiResult<bool>> Create(Guid userId, ThemDiaChiViewModel request);
        Task<ApiResult<bool>> Update(Guid id, Guid userId, SuaDiaChiViewModel request);
        Task<ApiResult<bool>> Delete(Guid id, Guid userId);
        Task<ApiResult<bool>> SetDefault(Guid id, Guid userId);
    }
}
