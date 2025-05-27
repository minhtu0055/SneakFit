using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPham
{
    public class SanPhamService : ISanPhamService
    {
        private readonly SneakFitDbContext _context;

        public SanPhamService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request)
        {
            var query = _context.SanPham.Include(x => x.DanhMuc).AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenSanPham.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SanPhamViewModels()
                {
                    Id = x.Id,
                    TenSanPham = x.TenSanPham,
                    Mota = x.Mota,
                    DanhMucId = x.DanhMucId,
                    TenDanhMuc = x.DanhMuc.TenDanhMuc,
                    TrangThai = x.TrangThai 
                }).ToListAsync();
            var PageResult = new PagedResult<SanPhamViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<List<SanPhamViewModels>> GetAll()
        {
            var list = await _context.SanPham
                .Include(x => x.DanhMuc)
                .Select(x => new SanPhamViewModels()
                {
                    Id = x.Id,
                    TenSanPham = x.TenSanPham,
                    Mota = x.Mota,
                    DanhMucId = x.DanhMucId,
                    TenDanhMuc = x.DanhMuc.TenDanhMuc,
                    TrangThai = x.TrangThai
                })
                .ToListAsync();

            return list;
        }

        public async Task<SanPhamViewModels?> GetById(Guid id)
        {
            var entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            return new SanPhamViewModels()
            {
                Id = entity.Id,
                TenSanPham = entity.TenSanPham,
                Mota = entity.Mota,
                DanhMucId = entity.DanhMucId,
                TenDanhMuc = entity.DanhMuc.TenDanhMuc
            };
        }

        public async Task<SanPhamViewModels> Create(ThemSanPham request)
        {
            var danhMuc = await _context.DanhMuc.FindAsync(request.DanhMucId);
            if (danhMuc == null)
                throw new Exception($"Không tìm thấy danh mục với id = {request.DanhMucId}");

            var newSanPham = new Data.Entities.SanPham()
            {
                Id = Guid.NewGuid(),
                TenSanPham = request.TenSanPham,
                Mota = request.Mota,
                DanhMucId = request.DanhMucId,
                TrangThai = true  // Thêm dòng này
            };

            _context.SanPham.Add(newSanPham);

            await _context.SaveChangesAsync();

            return new SanPhamViewModels()
            {
                Id = newSanPham.Id,
                TenSanPham = newSanPham.TenSanPham,
                Mota = newSanPham.Mota,
                DanhMucId = newSanPham.DanhMucId,
                TenDanhMuc = danhMuc.TenDanhMuc
            };
        }


        public async Task<SanPhamViewModels?> Update(SuaSanPham request)
        {
            var entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (entity == null)
                return null;

            entity.TenSanPham = request.TenSanPham;
            entity.Mota = request.Mota ?? entity.Mota;
            entity.DanhMucId = request.DanhMucId;

            await _context.SaveChangesAsync();

            entity = await _context.SanPham
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return new SanPhamViewModels()
            {
                Id = entity.Id,
                TenSanPham = entity.TenSanPham,
                Mota = entity.Mota,
                DanhMucId = entity.DanhMucId,
                TenDanhMuc = entity.DanhMuc?.TenDanhMuc
            };
        }

        public async Task<bool> UpdateTrangThai(Guid id, bool trangThai)
        {
            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham == null)
                return false;

            sanPham.TrangThai = trangThai;
            _context.SanPham.Update(sanPham);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSPCT(Guid id, List<SanPhamChiTietCapNhat> updates)
        {
            var sanPham = await _context.SanPham
                .Include(x => x.SanPhamChiTiet)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sanPham == null)
                return false;

            foreach (var update in updates)
            {
                var spct = sanPham.SanPhamChiTiet.FirstOrDefault(x => x.ID == update.Id);
                if (spct != null)
                {
                    spct.SoLuong = update.SoLuong;
                    spct.Gia = update.Gia;
                    spct.TrangThai = update.TrangThai;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<SPCTViewModels>> GetSPCTByFilter(SPCTFilterRequest request)
        {
            var query = _context.SanPhamChiTiet.Include(x => x.SanPham).AsQueryable();

            if (request.SanPhamId.HasValue)
                query = query.Where(x => x.SanPhamId == request.SanPhamId);
            else if (!string.IsNullOrEmpty(request.TenSanPham))
                query = query.Where(x => x.SanPham.TenSanPham == request.TenSanPham);

            // Thêm xử lý TuKhoa
            if (!string.IsNullOrEmpty(request.TuKhoa))
                query = query.Where(x => x.SanPham.TenSanPham.Contains(request.TuKhoa));

            if (request.KichThuocId.HasValue)
                query = query.Where(x => x.KichThuocId == request.KichThuocId);
            if (request.MauSacId.HasValue)
                query = query.Where(x => x.MauSacId == request.MauSacId);
            if (request.ChatLieuId.HasValue)
                query = query.Where(x => x.ChatLieuId == request.ChatLieuId);
            if (request.DeGiayId.HasValue)
                query = query.Where(x => x.DeGiayId == request.DeGiayId);
            if (request.ThuongHieuId.HasValue)
                query = query.Where(x => x.ThuongHieuId == request.ThuongHieuId);
            if (!string.IsNullOrEmpty(request.TrangThai))
                query = query.Where(x => x.TrangThai == (request.TrangThai == "true"));
            if (request.GiaTu.HasValue)
                query = query.Where(x => x.Gia >= request.GiaTu.Value);
            if (request.GiaDen.HasValue)
                query = query.Where(x => x.Gia <= request.GiaDen.Value);

            var danhSachSPCT = await query
                .Select(spct => new SPCTViewModels
                {
                    Id = spct.ID,
                    SanPhamId = spct.SanPhamId,
                    SoLuong = spct.SoLuong,
                    Gia = spct.Gia,
                    TrangThai = spct.TrangThai,
                    KichThuocId = spct.KichThuocId,
                    MauSacId = spct.MauSacId,
                    ChatLieuId = spct.ChatLieuId,
                    DeGiayId = spct.DeGiayId,
                    ThuongHieuId = spct.ThuongHieuId,
                }).ToListAsync();

            return danhSachSPCT;
        }

        public async Task<List<SPCTViewModels>> GetSPCTByProductName(string productName)
        {
            var query = _context.SanPhamChiTiet
                .Include(x => x.SanPham)
                .Where(x => x.SanPham.TenSanPham == productName);

            var danhSachSPCT = await query
                .Select(spct => new SPCTViewModels
                {
                    Id = spct.ID,
                    SanPhamId = spct.SanPhamId,
                    SoLuong = spct.SoLuong,
                    Gia = spct.Gia,
                    TrangThai = spct.TrangThai,
                    KichThuocId = spct.KichThuocId,
                    MauSacId = spct.MauSacId,
                    ChatLieuId = spct.ChatLieuId,
                    DeGiayId = spct.DeGiayId,
                    ThuongHieuId = spct.ThuongHieuId,
                }).ToListAsync();

            return danhSachSPCT;
        }

        public async Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId)
        {
            var spct = await _context.SanPhamChiTiet
                .Include(x => x.SanPham)
                .Include(x => x.HinhAnhSanPham) // Nếu có navigation property Images
                .FirstOrDefaultAsync(x => x.ID == spctId);

            if (spct == null) return null;

            return new SPCTDetailViewModel
            {
                Id = spct.ID,
                TenSanPham = spct.SanPham.TenSanPham,
                MoTa = spct.SanPham.Mota,
                ThuongHieuId = spct.ThuongHieuId,
                TrangThai = spct.TrangThai,
                ChatLieuId = spct.ChatLieuId,
                DeGiayId = spct.DeGiayId,
                MauSacId = spct.MauSacId,
                KichThuocId = spct.KichThuocId,
                SoLuong = spct.SoLuong,
                GiaBan = spct.Gia,
                //QRCodeUrl = spct.QRCodeUrl, // Nếu có
                //Images = spct.HinhAnhSanPham?.Select(i => new ImageViewModel { Id = i.Id, Url = i.Url }).ToList() ?? new List<ImageViewModel>()
            };
        }

        public async Task<bool> UpdateSPCTDetail(SuaSPCTDetailViewModel model)
        {
            var spct = await _context.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == model.Id);
            if (spct == null) return false;

            // Nếu muốn update cả bảng SanPham thì lấy entity SanPham ra và update
            var sanPham = await _context.SanPham.FirstOrDefaultAsync(x => x.Id == spct.SanPhamId);
            if (sanPham != null)
            {
                sanPham.TenSanPham = model.TenSanPham;
                sanPham.Mota = model.MoTa;
            }

            spct.ThuongHieuId = model.ThuongHieuId;
            spct.ChatLieuId = model.ChatLieuId;
            spct.DeGiayId = model.DeGiayId;
            spct.KichThuocId = model.KichThuocId;
            spct.MauSacId = model.MauSacId;
            spct.TrangThai = model.TrangThai;
            spct.SoLuong = model.SoLuong;
            spct.Gia = model.GiaBan;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
