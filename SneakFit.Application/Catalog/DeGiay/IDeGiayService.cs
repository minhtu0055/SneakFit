using SneakFit.ViewModels.Catalog.DeGiay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.DeGiay
{
   public  interface IDeGiayService
    {
        Task<List<DeGiayViewModels>> GetAll();
        Task<DeGiayViewModels> GetById(Guid id);
        Task<DeGiayViewModels> Create(ThemDeGiay request);
        Task<DeGiayViewModels> Update(SuaDeGiay request);
    }
}
