using MVC5Start.Infrastructure;
using Postal;

namespace MVC5Start.Models.Emails
{
    public class IdentityEmail : Email
    {
        public IdentityEmail()
        {
            this.From = Constants.DefaultApplicationEmailAddress;
        }

        public string Destination { get; set; }
        public string FullName { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}