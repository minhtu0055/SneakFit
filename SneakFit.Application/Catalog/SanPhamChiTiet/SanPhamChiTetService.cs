using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using SneakFit.Data.Entities;
using SneakFit.ViewModels.Common;
using SneakFit.Data.Enums;
using Microsoft.Extensions.Configuration;

namespace SneakFit.Application.Catalog.SanPhamChiTiet
{
    public class SanPhamChiTietService : ISanPhamChiTetService
    {
        private readonly SneakFitDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _baseAddress;

        public SanPhamChiTietService(SneakFitDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _baseAddress = _configuration["BaseAddress"]?.TrimEnd('/') ?? "";
        }

        // Lấy tất cả sản phẩm chi tiết
        public async Task<List<SPCTViewModels>> GetAll()
        {
            var list = await _context.SanPhamChiTiet
                .Include(x => x.HinhAnhSanPham)
                .Include(x => x.SanPham)
                .Include(x => x.MauSac)
                .Include(x => x.KichThuoc)
                .Include(x => x.ChatLieu)
                .Include(x => x.DeGiay)
                .Include(x => x.ThuongHieu)
                .Include(x => x.SanPham.DanhMuc)
                .Select(x => new SPCTViewModels()
                {
                    Id = x.ID,
                    Gia = x.Gia,
                    SoLuong = x.SoLuong,
                    MauSacId = x.MauSacId,
                    KichThuocId = x.KichThuocId,
                    ChatLieuId = x.ChatLieuId,
                    DeGiayId = x.DeGiayId,
                    ThuongHieuId = x.ThuongHieuId,
                    SanPhamId = x.SanPhamId,
                    DanhMucId = x.SanPham.DanhMucId,
                    TrangThai = x.TrangThai,
                    TenDanhMuc = x.SanPham.DanhMuc.TenDanhMuc,
                    NgayTao = x.NgayTao,
                    Images = x.HinhAnhSanPham.Select(h => h.UrlHinhAnh).ToList()
                })
                .ToListAsync();

            return list;
        }

        // Lấy thông tin chi tiết một sản phẩm chi tiết
        public async Task<SPCTViewModels> GetById(Guid id)
        {
            var entity = await _context.SanPhamChiTiet
                .Include(x => x.SanPham)
                .Include(x => x.SanPham.DanhMuc)
                .Include(x => x.HinhAnhSanPham)
                .Include(x => x.MauSac)
                .Include(x => x.KichThuoc)
                .Include(x => x.ChatLieu)
                .Include(x => x.DeGiay)
                .Include(x => x.ThuongHieu)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (entity == null) throw new Exception($"Không tìm thấy sản phẩm có id: {id}");

            return new SPCTViewModels()
            {
                Id = entity.ID,
                Gia = entity.Gia,
                SoLuong = entity.SoLuong,
                MauSacId = entity.MauSacId,
                KichThuocId = entity.KichThuocId,
                ChatLieuId = entity.ChatLieuId,
                DeGiayId = entity.DeGiayId,

                ThuongHieuId = entity.ThuongHieuId,
                SanPhamId = entity.SanPhamId,
                DanhMucId = entity.SanPham.DanhMucId,
                TrangThai = entity.TrangThai,
                TenDanhMuc = entity.SanPham.DanhMuc.TenDanhMuc,
                NgayTao = entity.NgayTao,
                Images = entity.HinhAnhSanPham.Select(h => h.UrlHinhAnh).ToList()
            };
        }

        // Lấy danh sách sản phẩm chi tiết với phân trang và tìm kiếm
        public async Task<PagedResult<SPCTViewModels>> GetAllPaging(PhanTrangSPCT request)
        {
            var query = from spct in _context.SanPhamChiTiet
                        join sp in _context.SanPham on spct.SanPhamId equals sp.Id
                        join dm in _context.DanhMuc on sp.DanhMucId equals dm.Id
                        select new { spct, sp, dm };

            if (!string.IsNullOrEmpty(request.TuKhoa))
                query = query.Where(x => x.sp.TenSanPham.Contains(request.TuKhoa));

            if (request.DanhMucId.HasValue)
                query = query.Where(x => x.sp.DanhMucId == request.DanhMucId);

            if (request.GiaThapNhat.HasValue)
                query = query.Where(x => x.spct.Gia >= request.GiaThapNhat.Value);

            if (request.GiaCaoNhat.HasValue)
                query = query.Where(x => x.spct.Gia <= request.GiaCaoNhat.Value);

            if (request.LocTrangthai == true)
                query = query.Where(x => x.spct.TrangThai == request.TrangThai);

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SPCTViewModels()
                {
                    Id = x.spct.ID,
                    Gia = x.spct.Gia,
                    SoLuong = x.spct.SoLuong,
                    MauSacId = x.spct.MauSacId,
                    KichThuocId = x.spct.KichThuocId,
                    ChatLieuId = x.spct.ChatLieuId,
                    DeGiayId = x.spct.DeGiayId,
                    ThuongHieuId = x.spct.ThuongHieuId,
                    SanPhamId = x.spct.SanPhamId,
                    DanhMucId = x.sp.DanhMucId,
                    TrangThai = x.spct.TrangThai,
                    TenDanhMuc = x.dm.TenDanhMuc,
                    NgayTao = x.spct.NgayTao,
                    Images = x.spct.HinhAnhSanPham.Select(h => h.UrlHinhAnh).ToList()
                }).ToListAsync();

            var pagedResult = new PagedResult<SPCTViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        // Cập nhật trạng thái sản phẩm chi tiết
        public async Task<bool> UpdateTrangThai(Guid id, bool trangThai)
        {
            var spct = await _context.SanPhamChiTiet.FindAsync(id);
            if (spct == null)
                return false;

            spct.TrangThai = trangThai;
            _context.SanPhamChiTiet.Update(spct);
            return await _context.SaveChangesAsync() > 0;
        }

        // Thêm mới sản phẩm chi tiết
        public async Task<ApiResult<SPCTViewModels>> Create(ThemSPCT request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!_context.SanPham.Any(x => x.Id == request.SanPhamId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Sản phẩm không tồn tại" };
            if (!_context.MauSac.Any(x => x.Id == request.MauSacId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Màu sắc không tồn tại" };
            if (!_context.KichThuoc.Any(x => x.Id == request.KichThuocId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Kích thước không tồn tại" };
            if (!_context.DeGiay.Any(x => x.Id == request.DeGiayId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Đế giày không tồn tại" };
            if (!_context.ThuongHieu.Any(x => x.Id == request.ThuongHieuId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Thương hiệu không tồn tại" };
            if (!_context.ChatLieu.Any(x => x.Id == request.ChatLieuId))
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Chất liệu không tồn tại" };

            // Kiểm tra trùng lặp
            bool isExist = _context.SanPhamChiTiet.Any(x =>
                x.SanPhamId == request.SanPhamId &&
                x.MauSacId == request.MauSacId &&
                x.KichThuocId == request.KichThuocId
            );
            if (isExist)
                return new ApiResult<SPCTViewModels> { IsSuccessed = false, Message = "Sản phẩm chi tiết này đã tồn tại" };

            // Tạo mới
            var newSanPhamChiTiet = new Data.Entities.SanPhamChiTiet()
            {
                ID = Guid.NewGuid(),
                SanPhamId = request.SanPhamId,
                MauSacId = request.MauSacId,
                KichThuocId = request.KichThuocId,
                ThuongHieuId = request.ThuongHieuId,
                ChatLieuId = request.ChatLieuId,
                DeGiayId = request.DeGiayId,
                Gia = request.Gia,
                SoLuong = request.SoLuong,
                TrangThai = request.TrangThai,
                NgayTao = DateTime.Now,
                HinhAnhSanPham = new List<HinhAnhSanPham>()
            };
            _context.SanPhamChiTiet.Add(newSanPhamChiTiet);

            await _context.SaveChangesAsync();
            var viewModel = await GetById(newSanPhamChiTiet.ID);
            // Đảm bảo trả về URL ảnh đầy đủ
            if (viewModel.Images != null && viewModel.Images.Count > 0)
            {
                viewModel.Images = viewModel.Images.Select(img => img.StartsWith("http") ? img : $"{_baseAddress}/images/products/{img}").ToList();
            }
            return new ApiResult<SPCTViewModels> { IsSuccessed = true, ResultObj = viewModel };
        }

        // Cập nhật sản phẩm chi tiết
        public async Task<SPCTViewModels?> Update(SuaSPCT request)
        {
            var entity = await _context.SanPhamChiTiet
                .Include(x => x.HinhAnhSanPham)
                .FirstOrDefaultAsync(x => x.ID == request.Id);

            if (entity == null)
                return null;

            entity.MauSacId = request.MauSacId;
            entity.KichThuocId = request.KichThuocId;
            entity.ChatLieuId = request.ChatLieuId;
            entity.DeGiayId = request.DeGiayId;
            entity.ThuongHieuId = request.ThuongHieuId;
            entity.Gia = request.Gia;
            entity.SoLuong = request.SoLuong;
            entity.TrangThai = request.TrangThai;
            entity.SanPhamId = request.SanPhamId;
            // entity.DanhMucId = request.DanhMucId; // Nếu có

            // Xử lý upload ảnh mới nếu có
            if (request.Images != null && request.Images.Count > 0)
            {
                // Xóa ảnh cũ
                var oldImages = await _context.HinhAnhSanPham
                    .Where(x => x.SanPhamChiTietId == request.Id)
                    .ToListAsync();

                foreach (var image in oldImages)
                {
                    try
                    {
                        DeleteFile(image.UrlHinhAnh);
                        _context.HinhAnhSanPham.Remove(image);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xóa ảnh cũ {image.UrlHinhAnh}: {ex.Message}");
                    }
                }

                // Thêm ảnh mới
                foreach (var image in request.Images)
                {
                    if (image.Length > 0)
                    {
                        var url = await SaveFile(image);
                        var hinhAnhSanPham = new Data.Entities.HinhAnhSanPham()
                        {
                            Id = Guid.NewGuid(),
                            SanPhamChiTietId = entity.ID,
                            UrlHinhAnh = url
                        };
                        _context.HinhAnhSanPham.Add(hinhAnhSanPham);
                    }
                }
            }

            await _context.SaveChangesAsync();
            var updated = await GetById(entity.ID);
            if (updated.Images != null && updated.Images.Count > 0)
            {
                updated.Images = updated.Images.Select(img => img.StartsWith("http") ? img : $"{_baseAddress}/images/products/{img}").ToList();
            }
            return updated;
        }   
        
        // Cập nhật giá sản phẩm chi tiết
        public async Task<bool> UpdateGia(Guid id, decimal gia)
        {
            var entity = await _context.SanPhamChiTiet.FindAsync(id);
            if (entity == null) return false;
            entity.Gia = gia;
            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật số lượng sản phẩm chi tiết
        //public async Task<bool> UpdateSoLuong(Guid id, int soLuong)
        //{
        //    var entity = await _context.SanPhamChiTiet.FindAsync(id);
        //    if (entity == null) return false;
        //    entity.SoLuong = soLuong;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}
        public async Task<ApiResult<bool>> UpdateSoLuong(Guid id, int deltaSoLuong)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.SanPhamChiTiet.FindAsync(id);
                if (entity == null)
                {
                    await transaction.RollbackAsync();
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Sản phẩm chi tiết không tồn tại." };
                }

                int newSoLuong = entity.SoLuong + deltaSoLuong;
                if (newSoLuong < 0)
                {
                    await transaction.RollbackAsync();
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Số lượng không thể âm sau khi cập nhật." };
                }

                entity.SoLuong = newSoLuong;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //_logger.LogError(ex, $"Lỗi khi cập nhật số lượng cho sản phẩm {id}");
                return new ApiResult<bool> { IsSuccessed = false, Message = $"Lỗi server: {ex.Message}" };
            }
        }

        // Lưu file
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = file.FileName;
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về URL đầy đủ
            return $"{_baseAddress}/images/products/{fileName}";
        }

        // Xóa file
        private void DeleteFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Thêm ảnh cho sản phẩm chi tiết
        public async Task<int> AddImage(Guid id, IFormFile image)
        {
            var url = await SaveFile(image);
            var hinhAnhSanPham = new Data.Entities.HinhAnhSanPham()
            {
                Id = Guid.NewGuid(),
                SanPhamChiTietId = id,
                UrlHinhAnh = url
            };

            _context.HinhAnhSanPham.Add(hinhAnhSanPham);
            return await _context.SaveChangesAsync();
        }

        // Xóa ảnh của sản phẩm chi tiết
        public async Task<int> RemoveImage(Guid imageId)
        {
            try
            {
                var image = await _context.HinhAnhSanPham.FindAsync(imageId);
                if (image == null) return 0;

                try 
                {
                    DeleteFile(image.UrlHinhAnh);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi xóa file {image.UrlHinhAnh}: {ex.Message}");
                }
                
                _context.HinhAnhSanPham.Remove(image);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong RemoveImage: {ex.Message}");
                throw;
            }
        }

        // Lấy danh sách ảnh của sản phẩm chi tiết
        public async Task<List<string>> GetListImages(Guid id)
        {
            var images = await _context.HinhAnhSanPham
                .Where(x => x.SanPhamChiTietId == id)
                .Select(i => i.UrlHinhAnh)
                .ToListAsync();
            return images.Select(img => img.StartsWith("http") ? img : $"{_baseAddress}/images/products/{img}").ToList();
        }

        // Tạo nhiều sản phẩm chi tiết cùng lúc
        public async Task<int> CreateMultiple(ThemNhieuSPCTRequest request)
        {
            int count = 0;
            foreach (var item in request.Items)
            {
                var newSanPhamChiTiet = new Data.Entities.SanPhamChiTiet()
                {
                    ID = Guid.NewGuid(),
                    SanPhamId = request.SanPhamId,
                    ThuongHieuId = request.ThuongHieuId,
                    DeGiayId = request.DeGiayId,
                    ChatLieuId = request.ChatLieuId,
                    TrangThai = request.TrangThai,
                    MauSacId = item.MauSacId,
                    KichThuocId = item.KichThuocId,
                    SoLuong = item.SoLuong,
                    Gia = item.Gia,
                    NgayTao = DateTime.Now,
                    HinhAnhSanPham = new List<HinhAnhSanPham>()
                };
                _context.SanPhamChiTiet.Add(newSanPhamChiTiet);
                count++;
            }
            await _context.SaveChangesAsync();
            return count;
        }
    }
}
