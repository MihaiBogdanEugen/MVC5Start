using System.Collections.Generic;

namespace MVC5Start.Infrastructure
{
    public static class Constants
    {
        #region Default User Role

        public const string DefaultUserRoleName = "User";
        public const string DefaultUserRoleDescription = "Default user role";

        #endregion Default User Role

        #region Default Administrator Role

        public const string DefaultAdministratorRoleName = "Administrator";
        public const string DefaultAdministratorRoleDescription = "Administrator role, full privileges";

        #endregion Default Administrator Role

        #region Default Administrator User

        public const string DefaultAdministratorFirstName = "Bogdan";
        public const string DefaultAdministratorLastName = "Mihai";
        public const string DefaultAdministratorEmail = "mbe1224@gmail.com";
        public const string DefaultAdministratorPassword = "danila8C@";

        #endregion Default Administrator User
        
        #region User Configuration

        public const bool UserAllowOnlyAlphanumericUserNames = false;
        public const bool UserRequireUniqueEmail = true;
        public const bool UserLockoutEnabled = true;
        public const int UserAccountLockoutMinutes = 5;
        public const int UserMaxFailedAccessAttemptsBeforeLockout = 5;

        #endregion User Configuration

        #region Password Config

        public const int PasswordRequiredLength = 8;
        public const bool PasswordRequireNonLetterOrDigit = true;
        public const bool PasswordRequireDigit = true;
        public const bool PasswordRequireLowercase = true;
        public const bool PasswordRequireUppercase = true;

        #endregion Password Config

        public const string DefaultController = "Home";
        public const string DefaultAction = "Index";

        public const string Anonymous = "Anonymous";

        public const string DefaultConnection = "DefaultConnection";
        public const string DefaultApplicationEmailAddress = "bogdan.mihai@live.com";
        public const string ConfirmationEmailSubject = "Verify your " + ApplicationName + " account";
        public const string PasswordResetEmailSubject = "Reset your " + ApplicationName + " account password";
        public const string ApplicationName = "MyDocShelf";

        public const string DefaultDateFormat = "dd/MM/yyyy";
        public const string DefaultLanguageCode = "en";
        public const string DefaultTimeFormat = "HH:mm";
        public const string DefaultTimeZoneCode = "(UTC) Coordinated Universal Time";

        public static readonly List<string> DateFormats = new List<string>
        {
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "dd.MM.yyyy",
            "MM/dd/yyyy",
            "MM-dd-yyyy",
            "MM.dd.yyyy",
        };

        public static readonly List<string> TimeFormats = new List<string>
        {
            "HH:mm",
            "HH:mm:ss",
        };

        //https://msdn.microsoft.com/en-us/library/ms533052(v=vs.85).aspx
        public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            { "en", "English" },
            { "ro", "Romanian" },
        };
    }
}