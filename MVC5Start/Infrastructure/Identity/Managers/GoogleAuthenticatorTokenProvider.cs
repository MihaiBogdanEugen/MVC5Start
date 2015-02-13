using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Base32;
using Microsoft.AspNet.Identity;
using MVC5Start.Models.Identity;
using OtpSharp;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public sealed class GoogleAuthenticatorTokenProvider : IUserTokenProvider<User, int>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<User, int> manager, User user)
        {
            return Task.FromResult((string)null);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<User, int> manager, User user)
        {
            long timeStepMatched;
            var totp = new Totp(Base32Encoder.Decode(user.GoogleAuthenticatorSecretKey));
            var isValid = totp.VerifyTotp(token, out timeStepMatched, new VerificationWindow(2, 2));
            return Task.FromResult(isValid);
        }

        public Task NotifyAsync(string token, UserManager<User, int> manager, User user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<User, int> manager, User user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}