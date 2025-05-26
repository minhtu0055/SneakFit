using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;

namespace SneakFit.Application.Catalog.ThuongHieu
{
    public interface IThuongHieuService
    {
        Task<PagedResult<ThuongHieuViewModels>> GetAllPaging(ThuongHieuPagingRequest request);
        Task<List<ThuongHieuViewModels>> GetAll();
        Task<ThuongHieuViewModels> GetById(Guid id);
        Task<ThuongHieuViewModels> Create(ThemThuongHieu request);
        Task<ThuongHieuViewModels> Update(SuaThuongHieu request);
    }
}
