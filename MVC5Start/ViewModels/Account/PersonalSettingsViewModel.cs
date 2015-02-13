using System.ComponentModel.DataAnnotations;

namespace MVC5Start.ViewModels.Account
{
    public class PersonalSettingsViewModel
    {
        [Display(Name = "Date Format"), Required, StringLength(20)]
        public string DateFormat { get; set; }

        [Display(Name = "Language"), Required, StringLength(10)]
        public string LanguageCode { get; set; }

        [Display(Name = "Time Format"), Required, StringLength(20)]
        public string TimeFormat { get; set; }

        [Display(Name = "Time Zone"), Required, StringLength(100)]
        public string TimeZoneCode { get; set; }
    }
}