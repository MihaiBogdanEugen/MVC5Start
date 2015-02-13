using MVC5Start.Infrastructure;
using Postal;

namespace MVC5Start.Models.Emails
{
    public class RegisterEmail : Email
    {
        public RegisterEmail()
        {
            this.Subject = Constants.ConfirmationEmailSubject;
            this.From = Constants.DefaultApplicationEmailAddress;
        }

        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string FullName { get; set; }
        public string CallbackUrl { get; set; }
    }
}