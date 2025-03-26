using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.DeGiay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.DeGiay
{
    public class DeGiayService : IDeGiayService
    {
        private readonly SneakFitDbContext _context;

        public DeGiayService(SneakFitDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeGiayViewModels>> GetAll()
        {
            return await _context.DeGiay
                .Select(x => new DeGiayViewModels { Id = x.Id, TenDeGiay = x.TenDeGiay })
                .ToListAsync();
        }

        public async Task<DeGiayViewModels?> GetById(Guid id)
        {
            var deGiay = await _context.DeGiay.FindAsync(id);
            if (deGiay == null) throw new KeyNotFoundException("Không tìm thấy đôi giày!");

            return new DeGiayViewModels { Id = deGiay.Id, TenDeGiay = deGiay.TenDeGiay };
        }

        public async Task<DeGiayViewModels> Create(ThemDeGiay request)
        {
            if (string.IsNullOrWhiteSpace(request.TenDeGiay))
                throw new ArgumentException("Tên đế giày không được để trống.");

            var deGiay = new SneakFit.Data.Entities.DeGiay
            {
                Id = Guid.NewGuid(),
                TenDeGiay = request.TenDeGiay
            };

            _context.DeGiay.Add(deGiay);
            await _context.SaveChangesAsync();


            return new DeGiayViewModels
            {
                Id = deGiay.Id,
                TenDeGiay = deGiay.TenDeGiay
            };
        }

        public async Task<DeGiayViewModels> Update(SuaDeGiay request)
        {
            var deGiay = await _context.DeGiay.FindAsync(request.Id);
            if (deGiay == null)
                throw new KeyNotFoundException("Không tìm thấy đôi giày!");

            if (string.IsNullOrWhiteSpace(request.TenDeGiay))
                throw new ArgumentException("Tên đôi giày không được để trống.");

            deGiay.TenDeGiay = request.TenDeGiay;

            await _context.SaveChangesAsync();

            return new DeGiayViewModels
            {
                Id = deGiay.Id,
                TenDeGiay = deGiay.TenDeGiay
            };
        }


    }
}
