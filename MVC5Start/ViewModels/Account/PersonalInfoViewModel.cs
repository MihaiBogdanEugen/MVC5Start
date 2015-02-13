namespace MVC5Start.ViewModels.Account
{
    public class PersonalInfoViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Roles { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}