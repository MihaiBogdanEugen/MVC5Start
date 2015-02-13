using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Dapper;
using System.Linq;
using Hangfire;
using Microsoft.AspNet.Identity;
using MVC5Start.Infrastructure.Hangfire;
using MVC5Start.Models.Emails;
using MVC5Start.ViewModels.Queries;
using Postal;

namespace MVC5Start.Infrastructure.Services
{
    public class AppEmailService : IIdentityMessageService
    {
        #region Fields

        private readonly DbConnectionInfo _dbConnectionInfo;
        private readonly EmailService _emailService;

        #endregion Fields

        #region Constructors

        public AppEmailService(DbConnectionInfo dbConnectionInfo)
        {
            this._dbConnectionInfo = dbConnectionInfo;

            var environmentPath = HostingEnvironment.MapPath(@"~/Views/Emails");
            if (string.IsNullOrEmpty(environmentPath))
                this._emailService = new EmailService();
            else
            {
                var viewsPath = Path.GetFullPath(environmentPath);
                var engines = new ViewEngineCollection {new FileSystemRazorViewEngine(viewsPath)};
                this._emailService = new EmailService(engines);
            }            
        }

        #endregion Constructors

        [AutomaticRetry(Attempts = 20), LogHangfireFailure]
        public async Task SendAsync(IdentityMessage message)
        {
            UserEmailInfo data = null;
            using(var connection = new SqlConnection(this._dbConnectionInfo.ConnectionString))
                data = connection.Query<UserEmailInfo>(Sql.Users.GetEmailInfoByEmail, new { Email = message.Destination }).FirstOrDefault();
            
            if (data == null)
                throw new ArgumentNullException("message");

            var email = new IdentityEmail
            {
                Destination = message.Destination,
                FullName = data.FirstName + " " + data.LastName,
                Subject = message.Subject,
                Body = message.Body
            };

            await this._emailService.SendAsync(email);
        }

        [AutomaticRetry(Attempts = 20), LogHangfireFailure]
        public void SendConfirmationEmail(int userId, string callBackUrl)
        {
            if (userId < 1)
                throw new ArgumentOutOfRangeException("userId");

            if(string.IsNullOrEmpty(callBackUrl))
                throw new ArgumentNullException("callBackUrl");

            UserEmailInfo data = null;
            using(var connection = new SqlConnection(this._dbConnectionInfo.ConnectionString))
                data = connection.Query<UserEmailInfo>(Sql.Users.GetEmailInfoById, new { Id = userId }).FirstOrDefault();

            if (data == null)
                throw new ArgumentNullException("userId");

            var email = new RegisterEmail
            {
                To = data.Email,
                FullName = data.FirstName + " " + data.LastName,
                CallbackUrl = callBackUrl,
            };

            this._emailService.Send(email);
        }

        [AutomaticRetry(Attempts = 20), LogHangfireFailure]
        public void SendPasswordResetEmail(int userId, string callBackUrl)
        {
            if (userId < 1)
                throw new ArgumentOutOfRangeException("userId");

            if(string.IsNullOrEmpty(callBackUrl))
                throw new ArgumentNullException("callBackUrl");

            UserEmailInfo data = null;
            using(var connection = new SqlConnection(this._dbConnectionInfo.ConnectionString))
                data = connection.Query<UserEmailInfo>(Sql.Users.GetEmailInfoById, new { Id = userId }).FirstOrDefault();

            if (data == null)
                throw new ArgumentNullException("userId");

            var email = new PasswordResetEmail
            {
                To = data.Email,
                FullName = data.FirstName + " " + data.LastName,
                CallbackUrl = callBackUrl,
            };

            this._emailService.Send(email);
        }
    }
}