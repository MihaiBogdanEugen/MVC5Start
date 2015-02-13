using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Account
{
    public class ResendConfirmationEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}