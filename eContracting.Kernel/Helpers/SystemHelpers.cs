using System;
using System.Linq;
using Sitecore.Configuration;

namespace eContracting.Kernel.Helpers
{
    public static class SystemHelpers
    {
        public static string ReadConfig(string key)
        {
            return Settings.GetSetting(key) ?? string.Empty;
        }

        public static Boolean IsAccountNumberValid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var account = input.TrimStart('0');

            return account.All(x => Char.IsDigit(x) || (x == '-' || x == '\\'));
        }
    }
}
