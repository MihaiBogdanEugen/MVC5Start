using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Session
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string SelectedProvider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
}