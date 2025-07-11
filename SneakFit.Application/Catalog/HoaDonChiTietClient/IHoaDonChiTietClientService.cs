using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.HoaDonChiTietClient;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonChiTietClients
{
    public interface IHoaDonChiTietClientService
    {
        Task<PagedResult<HoaDonChiTietClientViewModel>> GetAllPaging(PhanTrangHoaDonChiTietClient request);
        Task<List<HoaDonChiTietClientViewModel>> GetById(Guid id);
        Task<HoaDonChiTietClientViewModel> Create(ThemHoaDonChiTietClient request);
        Task<HoaDonChiTietClientViewModel> Edit(SuaHoaDonChiTietClient request);
    }
}
