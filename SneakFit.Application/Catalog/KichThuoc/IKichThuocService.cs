using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KichThuoc
{
    public interface IKichThuocService
    {
        Task<PagedResult<KichThuocViewModels>> GetAllPaging(KichThuocPagingRequest request);
        Task<KichThuocViewModels> GetById(Guid id);
        Task<KichThuocViewModels> Create(ThemKichThuoc request);
        Task<KichThuocViewModels> Update(SuaKichThuoc request);
    }
}
