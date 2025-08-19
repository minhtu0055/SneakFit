using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.TraHang;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.TraHang
{
    public class ReturnService : IReturnService
    {
        private readonly SneakFitDbContext _db;
        public ReturnService(SneakFitDbContext db) { _db = db; }

        private static ReturnViewModel Map(ReturnRequest e) => new()
        {
            ReturnId = e.Id,
            Code = e.Code,
            OrderId = e.OrderId,
            Status = e.Status,
            Reason = e.Reason,
            Method = e.Method,
            ShippingCarrier = e.ShippingCarrier,
            ShippingCode = e.ShippingCode,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };

        public async Task<ApiResult<Guid>> CreateAsync(CreateReturnRequest request, Guid userId)
        {
            var order = await _db.HoaDon.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == userId);
            if (order == null) return new ApiResult<Guid> { IsSuccessed = false, Message = "Không tìm thấy hóa đơn của bạn." };
            if (order.TrangThai != TrangThaiHoaDon.ThanhCong) return new ApiResult<Guid> { IsSuccessed = false, Message = "Chỉ tạo yêu cầu khi đơn đã thành công." };

            // NEW: Chặn tạo trùng cho cùng đơn hàng, NHƯNG CHO PHÉP nếu yêu cầu cũ đã bị TỪ CHỐI
            var existed = await _db.ReturnRequests.AsNoTracking()
                .AnyAsync(x => x.OrderId == request.OrderId && x.UserId == userId && x.Status != ReturnStatus.TuChoi);
            if (existed)
                return new ApiResult<Guid> { IsSuccessed = false, Message = "Bạn đã yêu cầu trả hàng/hoàn tiền cho đơn hàng này." };

            var entity = new ReturnRequest
            {
                Id = Guid.NewGuid(),
                Code = $"RR{DateTime.Now:yyyyMMddHHmmss}",
                OrderId = request.OrderId,
                UserId = userId,
                Reason = request.Reason ?? string.Empty,
                Method = ReturnMethod.BankTransfer,
                Status = ReturnStatus.ChapNhanDuyetHangHoan,
                BankAccountName = request.Bank?.AccountName,
                BankAccountNumber = request.Bank?.AccountNumber,
                BankName = request.Bank?.BankName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            if (request.Items != null && request.Items.Count > 0)
            {
                entity.Items = request.Items.Select(i => new ReturnRequestItem
                {
                    Id = Guid.NewGuid(),
                    ReturnRequestId = entity.Id,
                    OrderItemId = i.OrderItemId,
                    Quantity = i.Quantity
                }).ToList();
            }

            _db.ReturnRequests.Add(entity);
            await _db.SaveChangesAsync();
            return new ApiResult<Guid> { IsSuccessed = true, ResultObj = entity.Id, Message = "Đã tạo yêu cầu." };
        }

        // NEW: Kiểm tra tồn tại yêu cầu trả hàng cho một đơn (loại trừ TuChoi)
        public async Task<bool> HasReturnAsync(Guid orderId, Guid userId)
        {
            return await _db.ReturnRequests.AsNoTracking()
                .AnyAsync(x => x.OrderId == orderId && x.UserId == userId && x.Status != ReturnStatus.TuChoi);
        }

        public async Task<PagedResult<ReturnViewModel>> GetMyAsync(Guid userId, int pageIndex, int pageSize)
        {
            var q = _db.ReturnRequests.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt);
            var total = await q.CountAsync();
            var items = await q.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize)
                                       .Select(x => new
                    {
                        Return = x,
                        OrderCode = _db.HoaDon.AsNoTracking()
                            .Where(h => h.Id == x.OrderId)
                            .Select(h => h.MaHoaDon)
                            .FirstOrDefault()
                    })
                    .Select(t => new ReturnViewModel
                    {
                        ReturnId = t.Return.Id,
                        Code = t.Return.Code,
                        OrderId = t.Return.OrderId,
                        MaHoaDon = t.OrderCode ?? string.Empty,
                        Status = t.Return.Status,
                        Reason = t.Return.Reason,
                        Method = t.Return.Method,
                        ShippingCarrier = t.Return.ShippingCarrier,
                        ShippingCode = t.Return.ShippingCode,
                        CreatedAt = t.Return.CreatedAt,
                        UpdatedAt = t.Return.UpdatedAt
                    })
                   .ToListAsync();
            return new PagedResult<ReturnViewModel> { PageIndex = pageIndex, PageSize = pageSize, TotalRecords = total, Items = items };
        }

        public async Task<ReturnViewModel?> GetDetailAsync(Guid id, Guid userId)
        {
            var e = await _db.ReturnRequests.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (e == null) return null;
            
            var result = Map(e);
            // Bổ sung mã hóa đơn
            result.MaHoaDon = await _db.HoaDon.AsNoTracking()
                .Where(h => h.Id == e.OrderId)
                .Select(h => h.MaHoaDon)
                .FirstOrDefaultAsync() ?? string.Empty;
            
            // Lấy chi tiết đơn hàng để hiển thị sản phẩm
            var orderDetails = await _db.HoaDonChiTiet
                .Where(hd => hd.HoaDonId == e.OrderId)
                .Include(hd => hd.SanPhamChiTiet)
                    .ThenInclude(spct => spct.SanPham)
                .Include(hd => hd.SanPhamChiTiet)
                    .ThenInclude(spct => spct.MauSac)
                .Include(hd => hd.SanPhamChiTiet)
                    .ThenInclude(spct => spct.KichThuoc)
                .Include(hd => hd.SanPhamChiTiet)
                    .ThenInclude(spct => spct.HinhAnhSanPham)
                .Select(hd => new ReturnOrderDetailViewModel
                {
                    Id = hd.Id,
                    ProductName = hd.SanPhamChiTiet.SanPham.TenSanPham ?? "Không xác định",
                    ImageUrl = hd.SanPhamChiTiet.HinhAnhSanPham.FirstOrDefault().UrlHinhAnh ?? "/images/Default_Logo.png",
                    Size = hd.SanPhamChiTiet.KichThuoc.MaKichThuoc.ToString(),
                    Color = hd.SanPhamChiTiet.MauSac.TenMauSac ?? "Không xác định",
                    Quantity = hd.SoLuong,
                    Price = hd.GiaBan
                })
                .ToListAsync();
            
            result.OrderDetails = orderDetails;
            return result;
        }

        public async Task<ApiResult<bool>> CancelAsync(Guid id, Guid userId)
        {
            var e = await _db.ReturnRequests.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };
            if (e.Status != ReturnStatus.ChapNhanDuyetHangHoan) return new ApiResult<bool> { IsSuccessed = false, Message = "Chỉ hủy khi còn Chấp nhận duyệt hàng hoàn." };
            e.Status = ReturnStatus.TuChoi;
            e.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
        }

        // Admin ops - Cập nhật trạng thái theo luồng mới
        public async Task<ApiResult<bool>> ApproveAsync(Guid id, string? carrier, string? shipCode)
        {
            var e = await _db.ReturnRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };
            if (e.Status != ReturnStatus.ChapNhanDuyetHangHoan) return new ApiResult<bool> { IsSuccessed = false, Message = "Trạng thái không hợp lệ." };
            e.Status = ReturnStatus.LayHangHoan;
            e.ShippingCarrier = carrier;
            e.ShippingCode = shipCode;
            e.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
        }

        public async Task<ApiResult<bool>> ReceiveAsync(Guid id)
        {
            var e = await _db.ReturnRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };
            if (e.Status != ReturnStatus.LayHangHoan) return new ApiResult<bool> { IsSuccessed = false, Message = "Trạng thái không hợp lệ." };
            e.Status = ReturnStatus.HoanHang;
            e.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
        }

        public async Task<ApiResult<bool>> CompleteAsync(Guid id)
        {
            using var trx = await _db.Database.BeginTransactionAsync();
            var e = await _db.ReturnRequests.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };
            if (e.Status != ReturnStatus.HoanHang) return new ApiResult<bool> { IsSuccessed = false, Message = "Trạng thái không hợp lệ." };

            var order = await _db.HoaDon.Include(h => h.HoaDonChiTiet).FirstOrDefaultAsync(h => h.Id == e.OrderId);
            if (order == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy hóa đơn." };

            // Cộng kho theo Items (nếu trống => toàn bộ)
            var items = e.Items.Any() ? e.Items : order.HoaDonChiTiet.Select(hd => new ReturnRequestItem { OrderItemId = hd.Id, Quantity = hd.SoLuong }).ToList();
            foreach (var it in items)
            {
                var hdct = order.HoaDonChiTiet.FirstOrDefault(x => x.Id == it.OrderItemId);
                if (hdct == null) continue;
                var spct = await _db.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == hdct.SanPhamChiTietId);
                if (spct != null) spct.SoLuong += it.Quantity;
            }

            order.TrangThaiThanhToan = TrangThaiThanhToan.HoanTien;
            order.TrangThai = TrangThaiHoaDon.TraHang;

            e.Status = ReturnStatus.ThanhCong;
            e.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            await trx.CommitAsync();
            return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
        }

        public async Task<ApiResult<bool>> RejectAsync(Guid id, string reason)
        {
            var e = await _db.ReturnRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };
            if (e.Status != ReturnStatus.ChapNhanDuyetHangHoan) return new ApiResult<bool> { IsSuccessed = false, Message = "Trạng thái không hợp lệ." };
            e.Status = ReturnStatus.TuChoi;
            e.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
        }

        public async Task<PagedResult<ReturnViewModel>> GetAdminPagingAsync(int pageIndex, int pageSize,
            ReturnStatus? status, string? keyword, DateTime? from, DateTime? to)
        {
            var q = _db.ReturnRequests.AsNoTracking();

            if (status.HasValue) q = q.Where(x => x.Status == status.Value);
            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(x => x.Code.Contains(keyword) || x.Reason.Contains(keyword));
            if (from.HasValue) q = q.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) q = q.Where(x => x.CreatedAt <= to.Value);

            var total = await q.CountAsync();

            var items = await q.OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ReturnViewModel {
                    ReturnId = x.Id,
                    Code = x.Code,
                    OrderId = x.OrderId,
                    Status = x.Status,
                    Reason = x.Reason,
                    Method = x.Method,
                    ShippingCarrier = x.ShippingCarrier,
                    ShippingCode = x.ShippingCode,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<ReturnViewModel> {
                PageIndex = pageIndex, PageSize = pageSize, TotalRecords = total, Items = items
            };
        }

        public async Task<ReturnViewModel?> GetAdminDetailAsync(Guid id)
        {
            var x = await _db.ReturnRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (x == null) return null;
            var orderCode = await _db.HoaDon.AsNoTracking()
                .Where(h => h.Id == x.OrderId)
                .Select(h => h.MaHoaDon)
                .FirstOrDefaultAsync();
            return new ReturnViewModel {
                ReturnId = x.Id,
                Code = x.Code,
                OrderId = x.OrderId,
                MaHoaDon = orderCode ?? string.Empty,
                Status = x.Status,
                Reason = x.Reason,
                Method = x.Method,
                ShippingCarrier = x.ShippingCarrier,
                ShippingCode = x.ShippingCode,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }

        // Method mới để chuyển đổi trạng thái với ghi chú
        public async Task<ApiResult<bool>> UpdateStatusWithLogAsync(Guid id, ReturnStatus newStatus, string ghiChu, string nguoiChinhSua)
        {
            if (string.IsNullOrWhiteSpace(ghiChu))
                return new ApiResult<bool> { IsSuccessed = false, Message = "Ghi chú không được để trống!" };

            var e = await _db.ReturnRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy yêu cầu." };

            var oldStatus = e.Status;
            if (oldStatus == newStatus)
                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true, Message = "Trạng thái không thay đổi." };

            // Kiểm tra luồng trạng thái hợp lệ
            if (!IsValidStatusTransition(oldStatus, newStatus))
                return new ApiResult<bool> { IsSuccessed = false, Message = "Chuyển đổi trạng thái không hợp lệ." };

            // Nếu chuyển đến trạng thái thành công, thực hiện logic cộng kho
            if (newStatus == ReturnStatus.ThanhCong)
            {
                using var trx = await _db.Database.BeginTransactionAsync();
                try
                {
                    var order = await _db.HoaDon.Include(h => h.HoaDonChiTiet).FirstOrDefaultAsync(h => h.Id == e.OrderId);
                    if (order == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy hóa đơn." };

                    // Cộng kho theo Items (nếu trống => toàn bộ)
                    var items = e.Items.Any() ? e.Items : order.HoaDonChiTiet.Select(hd => new ReturnRequestItem { OrderItemId = hd.Id, Quantity = hd.SoLuong }).ToList();
                    foreach (var it in items)
                    {
                        var hdct = order.HoaDonChiTiet.FirstOrDefault(x => x.Id == it.OrderItemId);
                        if (hdct == null) continue;
                        var spct = await _db.SanPhamChiTiet.FirstOrDefaultAsync(x => x.ID == hdct.SanPhamChiTietId);
                        if (spct != null) spct.SoLuong += it.Quantity;
                    }

                    order.TrangThaiThanhToan = TrangThaiThanhToan.HoanTien;
                    order.TrangThai = TrangThaiHoaDon.TraHang;

                    e.Status = newStatus;
                    e.UpdatedAt = DateTime.Now;

                    // Lưu lịch sử thay đổi trạng thái
                    var history = new ReturnStatusHistory
                    {
                        Id = Guid.NewGuid(),
                        ReturnRequestId = e.Id,
                        TrangThaiCu = oldStatus,
                        TrangThaiMoi = newStatus,
                        GhiChu = ghiChu,
                        NguoiChinhSua = nguoiChinhSua,
                        NgayTao = DateTime.Now
                    };
                    _db.ReturnStatusHistories.Add(history);

                    await _db.SaveChangesAsync();
                    await trx.CommitAsync();
                    return new ApiResult<bool> { IsSuccessed = true, ResultObj = true, Message = "Cập nhật trạng thái thành công." };
                }
                catch (Exception ex)
                {
                    await trx.RollbackAsync();
                    return new ApiResult<bool> { IsSuccessed = false, Message = $"Lỗi khi cập nhật trạng thái: {ex.Message}" };
                }
            }
            else
            {
                e.Status = newStatus;
                e.UpdatedAt = DateTime.Now;

                // Lưu lịch sử thay đổi trạng thái
                var history = new ReturnStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ReturnRequestId = e.Id,
                    TrangThaiCu = oldStatus,
                    TrangThaiMoi = newStatus,
                    GhiChu = ghiChu,
                    NguoiChinhSua = nguoiChinhSua,
                    NgayTao = DateTime.Now
                };
                _db.ReturnStatusHistories.Add(history);

                await _db.SaveChangesAsync();
                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true, Message = "Cập nhật trạng thái thành công." };
            }
        }

        // Kiểm tra luồng trạng thái hợp lệ
        private bool IsValidStatusTransition(ReturnStatus currentStatus, ReturnStatus newStatus)
        {
            switch (currentStatus)
            {
                case ReturnStatus.ChapNhanDuyetHangHoan:
                    return newStatus == ReturnStatus.LayHangHoan || newStatus == ReturnStatus.TuChoi;
                case ReturnStatus.LayHangHoan:
                    return newStatus == ReturnStatus.HoanHang;
                case ReturnStatus.HoanHang:
                    return newStatus == ReturnStatus.ThanhCong;
                case ReturnStatus.ThanhCong:
                    return false; // Không thể chuyển từ thành công
                case ReturnStatus.TuChoi:
                    return false; // Không thể chuyển từ từ chối
                default:
                    return false;
            }
        }

        // Lấy lịch sử thay đổi trạng thái
        public async Task<List<ReturnStatusHistoryViewModel>> GetStatusHistoryAsync(Guid returnRequestId)
        {
            var history = await _db.ReturnStatusHistories
                .Where(x => x.ReturnRequestId == returnRequestId)
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new ReturnStatusHistoryViewModel
                {
                    Id = x.Id,
                    ReturnRequestId = x.ReturnRequestId,
                    TrangThaiCu = x.TrangThaiCu,
                    TrangThaiMoi = x.TrangThaiMoi,
                    GhiChu = x.GhiChu,
                    NguoiChinhSua = x.NguoiChinhSua,
                    NgayTao = x.NgayTao
                })
                .ToListAsync();

            return history;
        }
    }
}
