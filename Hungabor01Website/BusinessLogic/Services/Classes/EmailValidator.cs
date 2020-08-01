using BusinessLogic.Services.Interfaces;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BusinessLogic.Services.Classes
{
    public class EmailValidator : IEmailValidator
    {
        private const string EmailRegexPattern =
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$",DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                return Regex.IsMatch(email, EmailRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            var idn = new IdnMapping();
            var domainName = idn.GetAscii(match.Groups[2].Value);
            return match.Groups[1].Value + domainName;
        }
    }
}
