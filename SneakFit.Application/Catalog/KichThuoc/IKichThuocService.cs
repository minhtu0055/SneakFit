using SneakFit.ViewModels.Catalog.KichThuoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KichThuoc
{
    public interface IKichThuocService
    {
        Task<List<KichThuocViewModels>> GetAll();
        Task<KichThuocViewModels> GetById(Guid id);
        Task<KichThuocViewModels> Create(ThemKichThuoc request);
        Task<KichThuocViewModels> Update(SuaKichThuoc request);
    }
}
