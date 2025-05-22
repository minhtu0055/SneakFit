using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.GioHang
{
    public class GioHangService : IGioHangService
    {
        private readonly SneakFitDbContext _context;

        public GioHangService(SneakFitDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<GioHangViewModel>> GetAllPaging(GioHangPagingRequest request)
        {
            var query = from gh in _context.GioHang
                        join u in _context.Users on gh.UserId equals u.Id
                        select new { gh, u };

            if (request.UserId.HasValue)
            {
                query = query.Where(x => x.gh.UserId == request.UserId.Value);
            }

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GioHangViewModel()
                {
                    Id = x.gh.Id,
                    UserId = x.gh.UserId,
                    UserName = x.u.UserName,
                    NgayTao = x.gh.NgayTao,
                    TongTien = _context.GioHangChiTiet.Where(ghct => ghct.GioHangId == x.gh.Id).Sum(ghct => ghct.Gia * ghct.SoLuong)
                }).ToListAsync();

            foreach (var item in data)
            {
                item.GioHangChiTiets = await GetGioHangChiTietsByGioHangId(item.Id);
            }

            var pagedResult = new PagedResult<GioHangViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<GioHangViewModel> GetById(Guid id)
        {
            var gioHang = await _context.GioHang.FindAsync(id);
            if (gioHang == null)
                return null;

            var user = await _context.Users.FindAsync(gioHang.UserId);

            var gioHangViewModel = new GioHangViewModel()
            {
                Id = gioHang.Id,
                UserId = gioHang.UserId,
                UserName = user.UserName,
                NgayTao = gioHang.NgayTao,
                GioHangChiTiets = await GetGioHangChiTietsByGioHangId(gioHang.Id),
                TongTien = _context.GioHangChiTiet.Where(ghct => ghct.GioHangId == gioHang.Id).Sum(ghct => ghct.Gia * ghct.SoLuong)
            };

            return gioHangViewModel;
        }

        public async Task<GioHangViewModel> GetByUserId(Guid userId)
        {
            var gioHang = await _context.GioHang.FirstOrDefaultAsync(x => x.UserId == userId);
            if (gioHang == null)
                return null;

            var user = await _context.Users.FindAsync(userId);

            var gioHangViewModel = new GioHangViewModel()
            {
                Id = gioHang.Id,
                UserId = gioHang.UserId,
                UserName = user.UserName,
                NgayTao = gioHang.NgayTao,
                GioHangChiTiets = await GetGioHangChiTietsByGioHangId(gioHang.Id),
                TongTien = _context.GioHangChiTiet.Where(ghct => ghct.GioHangId == gioHang.Id).Sum(ghct => ghct.Gia * ghct.SoLuong)
            };

            return gioHangViewModel;
        }

        public async Task<GioHangViewModel> ThemVaoGioHang(ThemVaoGioHangRequest request)
        {
            var gioHang = await _context.GioHang.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            // Nếu giỏ hàng chưa tồn tại, tạo mới
            if (gioHang == null)
            {
                gioHang = new Data.Entities.GioHang()
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    NgayTao = DateTime.Now
                };
                _context.GioHang.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var sanPhamChiTiet = await _context.SanPhamChiTiet.FindAsync(request.SanPhamChiTietId);
            if (sanPhamChiTiet == null)
                throw new Exception("Sản phẩm không tồn tại");

            var gioHangChiTiet = await _context.GioHangChiTiet
                .FirstOrDefaultAsync(x => x.GioHangId == gioHang.Id && x.SanPhamChiTietId == request.SanPhamChiTietId);

            if (gioHangChiTiet != null)
            {
                // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
                gioHangChiTiet.SoLuong += request.SoLuong;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                gioHangChiTiet = new Data.Entities.GioHangChiTiet()
                {
                    Id = Guid.NewGuid(),
                    GioHangId = gioHang.Id,
                    SanPhamChiTietId = request.SanPhamChiTietId,
                    SoLuong = request.SoLuong,
                    Gia = sanPhamChiTiet.Gia
                };
                _context.GioHangChiTiet.Add(gioHangChiTiet);
            }

            await _context.SaveChangesAsync();
            return await GetById(gioHang.Id);
        }

        public async Task<GioHangViewModel> CapNhatGioHang(CapNhatGioHangRequest request)
        {
            var gioHangChiTiet = await _context.GioHangChiTiet.FindAsync(request.Id);
            if (gioHangChiTiet == null)
                throw new Exception("Sản phẩm không tồn tại trong giỏ hàng");

            gioHangChiTiet.SoLuong = request.SoLuong;
            await _context.SaveChangesAsync();

            return await GetById(gioHangChiTiet.GioHangId);
        }

        public async Task<bool> XoaSanPhamKhoiGioHang(Guid gioHangChiTietId)
        {
            var gioHangChiTiet = await _context.GioHangChiTiet.FindAsync(gioHangChiTietId);
            if (gioHangChiTiet == null)
                throw new Exception("Sản phẩm không tồn tại trong giỏ hàng");

            _context.GioHangChiTiet.Remove(gioHangChiTiet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> XoaGioHang(Guid id)
        {
            var gioHang = await _context.GioHang.FindAsync(id);
            if (gioHang == null)
                throw new Exception("Giỏ hàng không tồn tại");

            var gioHangChiTiets = _context.GioHangChiTiet.Where(x => x.GioHangId == id);
            _context.GioHangChiTiet.RemoveRange(gioHangChiTiets);
            _context.GioHang.Remove(gioHang);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<GioHangChiTietViewModel>> GetGioHangChiTietsByGioHangId(Guid gioHangId)
        {
            var query = from ghct in _context.GioHangChiTiet
                        join spct in _context.SanPhamChiTiet on ghct.SanPhamChiTietId equals spct.ID
                        join sp in _context.SanPham on spct.SanPhamId equals sp.Id
                        join ms in _context.MauSac on spct.MauSacId equals ms.Id
                        join kt in _context.KichThuoc on spct.KichThuocId equals kt.Id
                        where ghct.GioHangId == gioHangId
                        select new { ghct, spct, sp, ms, kt };

            return await query.Select(x => new GioHangChiTietViewModel()
            {
                Id = x.ghct.Id,
                GioHangId = x.ghct.GioHangId,
                SanPhamChiTietId = x.ghct.SanPhamChiTietId,
                TenSanPham = x.sp.TenSanPham,
                HinhAnh = _context.HinhAnhSanPham.FirstOrDefault(h => h.SanPhamChiTietId == x.sp.Id).UrlHinhAnh,
                MauSac = x.ms.TenMauSac,
                KichThuoc = x.kt.MaKichThuoc,
                DonGia = x.ghct.Gia,
                SoLuong = x.ghct.SoLuong,
                ThanhTien = x.ghct.Gia * x.ghct.SoLuong
            }).ToListAsync();
        }
    }
}
