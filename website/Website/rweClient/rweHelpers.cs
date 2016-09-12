using System;
using System.Configuration;

namespace rweClient
{
    public static class rweHelpers
    {
        public static string ReadConfig(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? String.Empty;
        }
    }
}
