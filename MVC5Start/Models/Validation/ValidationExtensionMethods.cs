using System;

namespace MVC5Start.Models.Validation
{
    public static class ValidationExtensionMethods
    {
        public static bool IsValid(this string text, int maxLength, bool allowEmpty = false)
        {
            if (maxLength < 0)
                throw new ArgumentNullException("maxLength");

            if (string.IsNullOrEmpty(text))
                return allowEmpty;

            return text.Length <= maxLength;
        }
    }
}