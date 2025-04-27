using SneakFit.ViewModels.Catalog.KhuyenMai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KhuyenMai
{
    public interface IKhuyenMaiService
    {
        Task<List<KhuyenMaiViewModels>> GetAll();
        Task<KhuyenMaiViewModels> GetById(Guid id); 
        Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request);
        Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request);
    }
}
