using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Account
{
    public class GoogleAuthenticatorViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string SecretKey { get; set; }

        [Required]
        public string BarcodeUrl { get; set; }

        [Required]
        public string DisableTwoFactorAuthPassword { get; set; }
    }
}