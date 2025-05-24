using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPham
{
    public interface ISanPhamService
    {
        Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request);
        Task<List<SanPhamViewModels>> GetAll();
        Task<SanPhamViewModels> GetById(Guid id);
        Task<SanPhamViewModels> Create(ThemSanPham request);
        Task<SanPhamViewModels> Update(SuaSanPham request);
    }
}
