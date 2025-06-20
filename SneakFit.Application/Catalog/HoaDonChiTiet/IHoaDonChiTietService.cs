using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDonChiTiet
{
    public interface IHoaDonChiTietService
    {
        Task<PagedResult<HoaDonChiTietViewModel>> GetAllPaging(PhanTrangHoaDonChiTiet request);
        Task<HoaDonChiTietViewModel> GetById(Guid id);
        Task<HoaDonChiTietViewModel> Create(ThemHoaDonChiTiet request);
        Task<HoaDonChiTietViewModel> Edit(SuaHoaDonChiTiet request);
    }
}
