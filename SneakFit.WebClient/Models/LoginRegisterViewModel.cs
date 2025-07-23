using SneakFit.ViewModels.System.User;

namespace SneakFit.WebClient.Models
{
    public class LoginRegisterViewModel
    {
        public LoginRequest Login { get; set; } = new LoginRequest();
        public RegisterRequest Register { get; set; } = new RegisterRequest();
        public QuenMatKhauRequest quenMatKhau { get; set; }
    }
}
