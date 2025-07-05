using Microsoft.AspNetCore.Mvc;
using SneakFit.ApiIntegration.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SneakFit.WebClient.Controllers
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly IGioHangApiClient _gioHangApiClient;
        public CartBadgeViewComponent(IGioHangApiClient gioHangApiClient)
        {
            _gioHangApiClient = gioHangApiClient;
        }

        private Guid GetUserId()
        {
            var userIdStr = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == "UserId" || x.Type == ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdStr)
                ? Guid.Parse("69BD714F-9576-45BA-B5B7-F00649BE00DE") // hardcode for demo
                : Guid.Parse(userIdStr);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = 0;
            var userId = GetUserId();
            var gioHang = await _gioHangApiClient.GetByUserId(userId);
            if (gioHang?.GioHangChiTiets != null)
            {
                count = gioHang.GioHangChiTiets.Sum(x => x.SoLuong);
            }
            ViewBag.Count = count;
            return View();
        }
    }
} 