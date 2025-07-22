using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IVoucherApiClient
    {
        Task<VoucherViewModels> Create(CreateVoucher request); // Thêm 
        Task<VoucherViewModels> Update(UpdateVoucher request); // Sửa
        Task<VoucherViewModels> GetById(Guid id); //Lấy theo ID
        Task<VoucherViewModels> GetByCode(string code); // Lấy theo mã
        Task<bool> UpdateTrangThai(Guid Id, TrangThaiGiamGia status);
        Task<PagedResult<VoucherViewModels>> GetAllPaging(GetVoucherPagingRequest request); // lấy dang sách voucher phân trang 
        Task<bool> UseVoucher(string code, Guid userId);
        Task<List<VoucherUserViewModel>> GetUsersForVoucher(Guid? voucherId = null);
        Task<PagedResult<VoucherUserViewModel>> GetUsersForVoucherPaging(GetVoucherUserPagingRequest request);
        Task<string> GetNextVoucherCode();
        Task<bool> GiamSoLuongVoucher(Guid id, int soLuong);
    }
}
