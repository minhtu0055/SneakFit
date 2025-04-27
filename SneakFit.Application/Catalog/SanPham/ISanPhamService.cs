using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.SanPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPham
{
    public interface ISanPhamService
    {
        Task<List<SanPhamViewModels>> GetAll();
        Task<SanPhamViewModels> GetById(Guid id);
        Task<SanPhamViewModels> Create(ThemSanPham request);
        Task<SanPhamViewModels> Update(SuaSanPham request);
    }
}
