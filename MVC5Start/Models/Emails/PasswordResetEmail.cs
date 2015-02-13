using MVC5Start.Infrastructure;
using Postal;

namespace MVC5Start.Models.Emails
{
    public class PasswordResetEmail : Email
    {
        public PasswordResetEmail()
        {
            this.Subject = Constants.PasswordResetEmailSubject;
            this.From = Constants.DefaultApplicationEmailAddress;
        }

        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string FullName { get; set; }
        public string CallbackUrl { get; set; }
    }
}