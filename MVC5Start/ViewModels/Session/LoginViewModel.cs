using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Session
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            this.ResendVerificationCode = false;
        }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public bool ResendVerificationCode { get; set; }
    }
}