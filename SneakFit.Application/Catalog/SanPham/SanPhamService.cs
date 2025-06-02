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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace SneakFit.Application.Catalog.SanPham
{
    public class SanPhamService : ISanPhamService
    {
        private readonly SneakFitDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SanPhamService(SneakFitDbContext context,
                              IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        // Lấy danh sách sản phẩm có phân trang
        public async Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request)
        {
            var query = _context.SanPham
                .Include(x => x.DanhMuc)
                .Include(x => x.SanPhamChiTiet)
                .AsQueryable();
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
                    TrangThai = x.TrangThai,
                    TongSoSanPham = x.SanPhamChiTiet.Where(spct => spct.TrangThai).Sum(spct => spct.SoLuong)
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

        // Lấy danh sách tất cả sản phẩm
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

        // Lấy chi tiết một sản phẩm
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

        // Thêm mới sản phẩm
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
                TrangThai = true 
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

        // Cập nhật sản phẩm
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

        // Cập nhật trạng thái sản phẩm
        public async Task<bool> UpdateTrangThai(Guid id, bool trangThai)
        {
            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham == null)
                return false;

            sanPham.TrangThai = trangThai;
            _context.SanPham.Update(sanPham);
            return await _context.SaveChangesAsync() > 0;
        }

        // Cập nhật chi tiết sản phẩm
        public async Task<bool> UpdateSPCT(List<SanPhamChiTietCapNhat> updates)
        {
            foreach (var update in updates)
            {
                var spct = await _context.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == update.Id);
                if (spct != null)
                {
                    if (update.SoLuong > 0) spct.SoLuong = update.SoLuong;
                    if (update.Gia > 0) spct.Gia = update.Gia;
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        // Lấy danh sách sản phẩm chi tiết theo tên sản phẩm
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
                    Images = spct.HinhAnhSanPham.Select(i => i.UrlHinhAnh).ToList(),
                }).ToListAsync();

            return danhSachSPCT;
        }

        // Lấy chi tiết một sản phẩm chi tiết
        public async Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId)
        {
            var spct = await _context.SanPhamChiTiet
                .Include(x => x.SanPham)
                .Include(x => x.HinhAnhSanPham)
                .FirstOrDefaultAsync(x => x.ID == spctId);

            if (spct == null) return null;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var images = spct.HinhAnhSanPham?
                .Select((img, idx) => new ImageViewModel
                {
                    Id = img.Id,
                    UrlHinhAnh = baseUrl + img.UrlHinhAnh,
                    IsDefault = idx == 0
                }).ToList() ?? new List<ImageViewModel>();

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
                Images = images
            };
        }

        // Cập nhật chi tiết sản phẩm chi tiết
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

        // Upload ảnh cho sản phẩm chi tiết
        public async Task<bool> UploadImages(UploadImageRequest request)
        {
            var spct = await _context.SanPhamChiTiet
                .Include(x => x.HinhAnhSanPham)
                .FirstOrDefaultAsync(x => x.ID == request.SanPhamChiTietId);

            if (spct == null) return false;

            int currentCount = spct.HinhAnhSanPham.Count();
            if (currentCount + request.Files.Count > 3)
                return false;

            foreach (var file in request.Files)
            {
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine("wwwroot", "images", "products", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var image = new Data.Entities.HinhAnhSanPham
                    {
                        Id = Guid.NewGuid(),
                        SanPhamChiTietId = request.SanPhamChiTietId,
                        UrlHinhAnh = $"/images/products/{fileName}"
                    };
                    _context.HinhAnhSanPham.Add(image);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa ảnh của sản phẩm chi tiết
        public async Task<bool> DeleteImage(DeleteImageRequest request)
        {
            var image = await _context.HinhAnhSanPham
                .FirstOrDefaultAsync(x => x.Id == request.ImageId && x.SanPhamChiTietId == request.SanPhamChiTietId);

            if (image == null) return false;

            var filePath = Path.Combine("wwwroot", image.UrlHinhAnh.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _context.HinhAnhSanPham.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
