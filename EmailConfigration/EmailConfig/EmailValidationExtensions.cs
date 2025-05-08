using System;
using System.Text.RegularExpressions;

namespace EmailConfigration.EmailConfig
{
    public static class EmailValidationExtensions
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Validate if a string is a valid email address
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if the email is valid</returns>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// Normalize an email address by trimming and converting to lowercase
        /// </summary>
        /// <param name="email">Email to normalize</param>
        /// <returns>Normalized email</returns>
        public static string NormalizeEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return email;

            return email.Trim().ToLowerInvariant();
        }
    }
} 