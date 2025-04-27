using SneakFit.ViewModels.Catalog.MauSac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.MauSac
{
    public interface IMauSacService
    {
        Task<List<MauSacViewModels>> GetAll();
        Task<MauSacViewModels> GetById(Guid id);
        Task<MauSacViewModels> Create(ThemMauSac request);
        Task<MauSacViewModels> Update(SuaMauSac request);
    }
}
