using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.ThuongHieu;

namespace SneakFit.Application.Catalog.ThuongHieu
{
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly SneakFitDbContext _context;

        public ThuongHieuService(SneakFitDbContext context)
        {
            _context = context;
        }
        public Task<List<ThuongHieuViewModels>> GetAll()
        {
            throw new NotImplementedException(); // Code
        }
        public Task<ThuongHieuViewModels> GetById(int id)
        {
            throw new NotImplementedException(); // Code
        }
        public Task<ThuongHieuViewModels> Create(ThemThuongHieu request)
        {
            throw new NotImplementedException(); // Code
        }
        public Task<ThuongHieuViewModels> Update(SuaThuongHieu request)
        {
            throw new NotImplementedException(); // Code
        }
    }
}
