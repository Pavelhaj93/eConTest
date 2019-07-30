// <copyright file="SystemHelpers.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    using System;
    using System.Linq;
    using Sitecore.Configuration;
    using eContracting.Kernel.Utils;
    using eContracting.Kernel.Services;
    using System.Collections.Generic;
    using Sitecore.Diagnostics;
    using System.Text;

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


            IEnumerable<XmlText> text = client.GetTextsXml(data.Identifier);
            XmlText letterXml = client.GetLetterXml(text);
            Dictionary<string, string> parameters = client.GetAllAtrributes(letterXml);

            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Retrieved parameters for '" + data.Identifier + "':");
                stringBuilder.AppendLine("-----------------------------------------------------");
                stringBuilder.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(parameters, Newtonsoft.Json.Formatting.Indented));
                stringBuilder.AppendLine("-----------------------------------------------------");
                Log.Info(stringBuilder.ToString(), typeof(SystemHelpers));
            }
            catch (Exception ex)
            {
                Log.Warn("Cannot serialize parameters", ex, typeof(SystemHelpers));
            }

            foreach (var item in parameters)
            {
                //mainRawText = mainRawText.Replace("{" + item.Key + "}", item.Value);
                mainRawText = mainRawText.Replace(string.Format("{{{0}}}", item.Key), item.Value);
            }

            mainRawText = mainRawText.Replace("{DATE_TO}", data.ExpDateFormatted);

            return mainRawText;
        }
    }
}
