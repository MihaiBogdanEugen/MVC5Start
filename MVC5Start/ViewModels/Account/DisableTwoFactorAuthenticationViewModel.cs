using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Account
{
    public class DisableTwoFactorAuthenticationViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}