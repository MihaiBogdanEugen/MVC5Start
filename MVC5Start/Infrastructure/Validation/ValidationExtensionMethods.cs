using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MVC5Start.Infrastructure.Validation
{
    public static class ValidationExtensionMethods
    {
        private static readonly Regex EmailRegex = new Regex("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public static bool IsValid(this string text, int maxLength, bool allowEmpty = false)
        {
            if (maxLength < 0)
                throw new ArgumentNullException("maxLength");

            if (string.IsNullOrEmpty(text))
                return allowEmpty;

            return text.Length <= maxLength;
        }

        public static bool IsValidEmail(this string text, int maxLength, bool allowEmpty = false)
        {
            if (maxLength < 0)
                throw new ArgumentNullException("maxLength");

            if (string.IsNullOrEmpty(text))
                return allowEmpty;

            if (text.Length > maxLength)
                return false;

            return EmailRegex.Match(text).Length > 0;
        }

        public static bool IsValidPassword(this string text)
        {
            return text.IsValidPassword(
                requiredLength: Constants.PasswordRequiredLength,
                requireNonLetterOrDigit: Constants.PasswordRequireNonLetterOrDigit,
                requireDigit: Constants.PasswordRequireDigit,
                requireLowercase: Constants.PasswordRequireLowercase,
                requireUppercase: Constants.PasswordRequireUppercase);
        }

        public static bool IsValidPassword(this string text, int requiredLength, 
            bool requireNonLetterOrDigit, bool requireDigit, bool requireLowercase, bool requireUppercase)
        {
            if (requiredLength < 1)
                throw new ArgumentNullException("requiredLength");

            if (string.IsNullOrEmpty(text))
                return false;

            if (text.Length < requiredLength)
                return false;

            if (requireDigit && text.Any(Char.IsDigit) == false)
                return false;

            if (requireUppercase && text.Any(Char.IsUpper) == false)
                return false;

            if (requireLowercase && text.Any(Char.IsLower) == false)
                return false;

            if (requireNonLetterOrDigit && text.All(Char.IsLetterOrDigit))
                return false;

            return true;
        }
    }
}