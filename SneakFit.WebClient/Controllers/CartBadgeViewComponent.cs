using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using SneakFit.WebClient.Models;
using SneakFit.ViewModels.Catalog.GioHang; // Added this import for GioHangViewModel

namespace SneakFit.WebClient.Controllers
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly IGioHangApiClient _gioHangApiClient;
        public CartBadgeViewComponent(IGioHangApiClient gioHangApiClient)
        {
            _gioHangApiClient = gioHangApiClient;
        }

        private Guid? TryGetUserId()
        {
            var userIdStr = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return null;
            }
            return Guid.Parse(userIdStr);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = 0;
            var userId = TryGetUserId();
            if (userId != null)
            {
                GioHangViewModel gioHang = null;

                try
                {
                    gioHang = await _gioHangApiClient.GetByUserId(userId.Value);
                }
                catch
                {
                    // Nếu không có giỏ hàng thì tạo mới
                    gioHang = await _gioHangApiClient.TaoGioHangMoi(userId.Value);
                }

                if (gioHang?.GioHangChiTiets != null)
                {
                    count = gioHang.GioHangChiTiets.Count;
                }
            }
            ViewBag.Count = count;
            return View();
        }
    }
} 