namespace MVC5Start.ViewModels.Account
{
    public class ManageViewModel
    {
        public string Message { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Roles { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool IsBrowserRemembered { get; set; }
    }
}