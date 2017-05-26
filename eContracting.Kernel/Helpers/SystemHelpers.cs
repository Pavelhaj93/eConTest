// <copyright file="AcceptOfferJob.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    using System;
    using System.Linq;
    using Sitecore.Configuration;
    using eContracting.Kernel.Utils;
    using eContracting.Kernel.Services;

    public static class SystemHelpers
    {
        /// <summary>
        /// Reads the config value.
        /// </summary>
        /// <param name="key">Key to look for.</param>
        /// <returns>Returns value if key exists, otherwise returns null.</returns>
        public static string ReadConfig(string key)
        {
            return Settings.GetSetting(key) ?? string.Empty;
        }

        /// <summary>
        /// Validates the account number.
        /// </summary>
        /// <param name="input">Account number.</param>
        /// <returns>Returns true if account is valid, otherwise retruns false.</returns>
        public static Boolean IsAccountNumberValid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var account = input.TrimStart('0');

            return account.All(x => Char.IsDigit(x) || (x == '-' || x == '\\'));
        }

        /// <summary>
        /// Generates the main text.
        /// </summary>
        /// <param name="data">Authentication data.</param>
        /// <param name="mainRawText">Raw text to modify.</param>
        /// <returns>Returns modified text.</returns>
        public static string GenerateMainText(AuthenticationDataItem data,string mainRawText)
        {
            RweClient client = new RweClient();

            if (string.IsNullOrEmpty(data.DateOfBirth))
            {
                return null;
            }


            var text = client.GetTextsXml(data.Identifier);
            var letterXml = client.GetLetterXml(text);
            var parameters = client.GetAllAtrributes(letterXml);
            foreach (var item in parameters)
            {
                mainRawText = mainRawText.Replace(string.Format("{{{0}}}", item.Key), item.Value);
            }
            mainRawText = mainRawText.Replace("{DATE_TO}", data.ExpDateFormatted);

            return mainRawText;
        }
    }
}
