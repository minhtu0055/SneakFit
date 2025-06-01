using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDon
{
    public interface IHoaDonService
    {
        Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request);
        Task<HoaDonViewModel> GetById(Guid id);
        Task<HoaDonViewModel> Create(ThemHoaDon request);
        Task<HoaDonViewModel> Update(SuaHoaDon request);
        Task<bool> UpdateStatus(Guid id, TrangThaiHoaDon trangThai);
    }
}
