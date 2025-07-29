using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonChiTiet;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Catalog.LichSuHoaDon;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDon
{
    public class HoaDonService : IHoaDonService
    {
        private readonly SneakFitDbContext _context;

        public HoaDonService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request)
        {
            var query = _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(h => h.HoTen.Contains(request.Keyword) || h.MaHoaDon.Contains(request.Keyword));
            }
            // Lọc theo trạng thái
            if (request.Trangthaihoadon.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.Trangthaihoadon.Value);
            }
            // Lọc theo ngày tạo trong khoảng thời gian từ ngày đến ngày
            if (request.NgayBatDau.HasValue && request.NgayKetThuc.HasValue)
            {
                query = query.Where(h => h.NgayTao >= request.NgayBatDau.Value && h.NgayTao <= request.NgayKetThuc.Value);
            }
            if (!string.IsNullOrEmpty(request.NguoiTao))
            {
                query = query.Where(h => h.NguoiTao == request.NguoiTao);
            }
            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(h => new HoaDonViewModel
                {
                    Id = h.Id,
                    NgayTao = h.NgayTao,
                    TongTien = h.TongTien,
                    TrangThai = h.TrangThai,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    SoDienThoai = h.SoDienThoai,
                    Email = h.Email,
                    GiaoHang = h.GiaoHang,
                    PhuongThucThanhToan = h.PhuongThucThanhToan,
                    LoaiHoaDon = h.LoaiHoaDon,
                    NgayThanhToan = h.NgayThanhToan,
                    MaHoaDon = h.MaHoaDon,
                    PhiVanChuyen = h.PhiVanChuyen,
                    TrangThaiThanhToan = h.TrangThaiThanhToan,
                    TienKhachDua = h.TienKhachDua,                  
                }).ToListAsync();
            var pagedResult = new PagedResult<HoaDonViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<HoaDonViewModel> GetById(Guid id)
        {
            var hoaDon = await _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hoaDon == null) return null;
            return new HoaDonViewModel
            {
                Id = hoaDon.Id,
                NgayTao = hoaDon.NgayTao,
                TongTien = hoaDon.TongTien,
                TrangThai = hoaDon.TrangThai,
                HoTen = hoaDon.HoTen,
                DiaChi = hoaDon.DiaChi,
                SoDienThoai = hoaDon.SoDienThoai,
                GiaoHang = hoaDon.GiaoHang,
                Email = hoaDon.Email,
                PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                LoaiHoaDon = hoaDon.LoaiHoaDon,
                NgayThanhToan = hoaDon.NgayThanhToan,
                MaHoaDon = hoaDon.MaHoaDon,
                PhiVanChuyen = hoaDon.PhiVanChuyen,
                TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                VoucherId = hoaDon.VoucherId,
                TienKhachDua = hoaDon.TienKhachDua,
                MaTinh = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.MaTinh).FirstOrDefault(),
                TenTinh = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.TenThanhPho).FirstOrDefault(),
                MaHuyen = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.MaHuyen).FirstOrDefault(),
                TenHuyen = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.TenHuyen).FirstOrDefault(),
                MaXa = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.MaXa).FirstOrDefault(),
                TenXa = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.TenXa).FirstOrDefault(),
                TenDiaChi = _context.DiaChi.Where(dc => dc.UserId == hoaDon.UserId && dc.Mac_Dinh).Select(dc => dc.TenDiaChi).FirstOrDefault()
            };
        }

        public async Task<HoaDonViewModel> Create(ThemHoaDon request, string tenNguoiTao)
        {
            var maHoaDon = $"HD{DateTime.Now:MMddHHmmss}{new Random().Next(1000, 9999)}";
            var hoaDon = new Data.Entities.HoaDon
            {
                Id = Guid.NewGuid(),
                NgayTao = DateTime.Now,
                TongTien = request.TongTien,
                TrangThai = TrangThaiHoaDon.ChoXacNhan,
                UserId = request.UserId,
                DiaChi = request.DiaChi,
                SoDienThoai = request.SoDienThoai,
                Email = request.Email,
                HoTen = request.HoTen,
                GhiChu = request.GhiChu,
                GiaoHang = request.GiaoHang,
                NguoiTao = tenNguoiTao,
                PhuongThucThanhToan = request.PhuongThucThanhToan,
                LoaiHoaDon = request.LoaiHoaDon,
                NgayThanhToan = request.NgayThanhToan,
                MaHoaDon = maHoaDon,
                PhiVanChuyen = request.PhiVanChuyen,
                TrangThaiThanhToan = request.TrangThaiThanhToan,
                VoucherId = request.VoucherId,
                TienKhachDua = request.TienKhachDua
            };
            _context.HoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();

            return await GetById(hoaDon.Id);
        }

        public async Task<HoaDonViewModel> Update(SuaHoaDon request)
        {
            var hoaDon = await _context.HoaDon.FindAsync(request.Id);
            if (hoaDon == null) return null;

            hoaDon.TongTien = request.TongTien;
            hoaDon.TrangThai = request.TrangThai;
            hoaDon.DiaChi = request.DiaChi;
            hoaDon.SoDienThoai = request.SoDienThoai;
            hoaDon.Email = request.Email;
            hoaDon.HoTen = request.HoTen;
            hoaDon.UserId = request.UserId;
            hoaDon.GiaoHang = request.GiaoHang;
            hoaDon.GhiChu = request.GhiChu;
            hoaDon.PhuongThucThanhToan = request.PhuongThucThanhToan;
            hoaDon.LoaiHoaDon = request.LoaiHoaDon;
            hoaDon.NgayThanhToan = request.NgayThanhToan;
            hoaDon.MaHoaDon = request.MaHoaDon;
            hoaDon.PhiVanChuyen = request.PhiVanChuyen;
            hoaDon.TrangThaiThanhToan = request.TrangThaiThanhToan;
            hoaDon.VoucherId = request.VoucherId;
            hoaDon.TienKhachDua = request.TienKhachDua;

            await _context.SaveChangesAsync();

            // Sau khi cập nhật hóa đơn, kiểm tra và trừ số lượng voucher nếu cần
            if (hoaDon.VoucherId != null && hoaDon.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
            {
                var voucher = await _context.Voucher.FindAsync(hoaDon.VoucherId);
                if (voucher != null)
                {
                    if (voucher.SoLuong > 0)
                    {
                        voucher.SoLuong--;
                        if (voucher.SoLuong == 0)
                            voucher.TrangThai = TrangThaiGiamGia.HetHan;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Có thể trả về lỗi: Voucher đã hết lượt sử dụng!
                        // return null hoặc throw exception tùy logic của bạn
                        return null;
                    }
                }
            }

            return await GetById(hoaDon.Id);
        }

        public async Task<Dictionary<TrangThaiHoaDon, int>> GetCountByStatusAsync()
        {
            // Lấy toàn bộ hóa đơn, group by trạng thái, đếm từng loại
            var counts = await _context.HoaDon
                .GroupBy(h => h.TrangThai)
                .Select(g => new { TrangThai = g.Key, Count = g.Count() })
                .ToListAsync();

            // Đưa về Dictionary cho dễ dùng
            return counts.ToDictionary(x => x.TrangThai, x => x.Count);
        }
        public async Task<List<HoaDonViewModel>> GetHoaDonChoByNguoiTao(string nguoiTao)
        {
            var query = _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdc => hdc.SanPhamChiTiet)
                .Include(h => h.User)
                .Include(h => h.Voucher)
                .Where(h => h.NguoiTao == nguoiTao && h.TrangThai == TrangThaiHoaDon.ChoXacNhan)
                .AsQueryable();

            var data = await query
                .Select(h => new HoaDonViewModel
                {
                    Id = h.Id,
                    NgayTao = h.NgayTao,
                    TongTien = h.TongTien,
                    TrangThai = h.TrangThai,
                    UserId = h.UserId,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    SoDienThoai = h.SoDienThoai,
                    GiaoHang = h.GiaoHang,
                    Email = h.Email,
                    PhuongThucThanhToan = h.PhuongThucThanhToan,
                    LoaiHoaDon = h.LoaiHoaDon,
                    NgayThanhToan = h.NgayThanhToan,
                    MaHoaDon = h.MaHoaDon,
                    PhiVanChuyen = h.PhiVanChuyen,
                    TrangThaiThanhToan = h.TrangThaiThanhToan,
                    VoucherId = h.VoucherId,
                    TienKhachDua = h.TienKhachDua,
                    MaTinh = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.MaTinh).FirstOrDefault(),
                    TenTinh = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.TenThanhPho).FirstOrDefault(),
                    MaHuyen = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.MaHuyen).FirstOrDefault(),
                    TenHuyen = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.TenHuyen).FirstOrDefault(),
                    MaXa = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.MaXa).FirstOrDefault(),
                    TenXa = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.TenXa).FirstOrDefault(),
                    TenDiaChi = _context.DiaChi.Where(dc => dc.UserId == h.UserId && dc.Mac_Dinh).Select(dc => dc.TenDiaChi).FirstOrDefault()
                }).ToListAsync();

            return data;
        }

        public async Task<bool> Delete(Guid id)
        {
            var hoaDon = await _context.HoaDon
                .Include(h => h.HoaDonChiTiet)
                .ThenInclude(hdct => hdct.SanPhamChiTiet)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null) return false;

            // Hoàn kho cho từng sản phẩm trong hóa đơn chi tiết
            if (hoaDon.HoaDonChiTiet != null)
            {
                foreach (var hdct in hoaDon.HoaDonChiTiet)
                {
                    if (hdct.SanPhamChiTiet != null)
                    {
                        hdct.SanPhamChiTiet.SoLuong += hdct.SoLuong;
                    }
                }
                // Xóa hết hóa đơn chi tiết
                _context.HoaDonChiTiet.RemoveRange(hoaDon.HoaDonChiTiet);
            }

            // Xóa hóa đơn
            _context.HoaDon.Remove(hoaDon);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RevertToPreviousStatusAsync(Guid hoaDonId, string nguoiThucHien)
        {
            var lastHistory = await _context.LichSuHoaDon
                .Where(x => x.HoaDonId == hoaDonId)
                .OrderByDescending(x => x.NgayTao)
                .FirstOrDefaultAsync();

            if (lastHistory == null)
                return false;

            var hoaDon = await _context.HoaDon.FindAsync(hoaDonId);
            if (hoaDon == null)
                return false;

            // Cập nhật trạng thái hóa đơn về trạng thái cũ
            TrangThaiHoaDon trangThaiHienTai = hoaDon.TrangThai;
            hoaDon.TrangThai = lastHistory.TrangThaiCu;

            // Ghi lại lịch sử thao tác quay lại
            var revertHistory = new Data.Entities.LichSuHoaDon
            {
                Id = Guid.NewGuid(),
                HoaDonId = hoaDonId,
                TrangThaiCu = trangThaiHienTai,
                TrangThaiMoi = lastHistory.TrangThaiCu,
                NgayTao = DateTime.Now,
                NguoiChinhSua = nguoiThucHien,
                UserId = lastHistory.UserId // hoặc truyền vào nếu cần
            };
            _context.LichSuHoaDon.Add(revertHistory);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<LichSuHoaDonViewModel>> GetByHoaDonIdAsync(Guid hoaDonId)
        {
            var query = _context.LichSuHoaDon
                .Where(x => x.HoaDonId == hoaDonId)
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new LichSuHoaDonViewModel
                {
                    Id = x.Id,
                    HoaDonId = x.HoaDonId,
                    TrangThaiCu = x.TrangThaiCu,
                    TrangThaiMoi = x.TrangThaiMoi,
                    NgayTao = x.NgayTao,
                    NguoiChinhSua = x.NguoiChinhSua
                });

            return await query.ToListAsync();
        }

        public async Task<Guid> CreateAsync(CreateLichSuHoaDonRequest request)
        {
            var entity = new Data.Entities.LichSuHoaDon
            {
                Id = Guid.NewGuid(),
                HoaDonId = request.HoaDonId,
                TrangThaiCu = request.TrangThaiCu,
                TrangThaiMoi = request.TrangThaiMoi,
                NgayTao = DateTime.Now,
                NguoiChinhSua = request.NguoiChinhSua,
                UserId = request.UserId
            };
            _context.LichSuHoaDon.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
                
        public async Task<bool> UpdateStatusAndLogAsync(Guid hoaDonId, TrangThaiHoaDon newStatus, Guid userId, string nguoiChinhSua)
        {
            var hoaDon = await _context.HoaDon.FindAsync(hoaDonId);
            if (hoaDon == null)
                return false;

            var oldStatus = hoaDon.TrangThai;
            if (oldStatus == newStatus)
                return true; // No change

            hoaDon.TrangThai = newStatus;

            var history = new Data.Entities.LichSuHoaDon
            {
                Id = Guid.NewGuid(),
                HoaDonId = hoaDonId,
                TrangThaiCu = oldStatus,
                TrangThaiMoi = newStatus,
                NgayTao = DateTime.Now,
                NguoiChinhSua = nguoiChinhSua,
                UserId = userId
            };
            _context.LichSuHoaDon.Add(history);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
