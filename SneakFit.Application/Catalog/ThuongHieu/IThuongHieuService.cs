using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.ViewModels.Catalog.ThuongHieu;

namespace SneakFit.Application.Catalog.ThuongHieu
{
    public interface IThuongHieuService
    {
        Task<List<ThuongHieuViewModels>> GetAll();
        Task<ThuongHieuViewModels> GetById(int id);
        Task<ThuongHieuViewModels> Create(ThemThuongHieu request);
        Task<ThuongHieuViewModels> Update(SuaThuongHieu request);
    }
}
