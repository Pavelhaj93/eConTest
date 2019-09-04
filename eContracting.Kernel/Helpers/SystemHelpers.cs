// <copyright file="SystemHelpers.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using eContracting.Kernel.Services;
    using eContracting.Kernel.Utils;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;

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
        /// Gets a collection of parameters which can be used for string replacmement.
        /// </summary>
        /// <returns>Collection of parameters.</returns>
        public static IDictionary<string, string> GetParameters(string guid)
        {
            var client = new RweClient();
            var text = client.GetTextsXml(guid);
            var parameters = client.GetAllAttributes(text);

            return parameters;
        }

        /// <summary>
        /// Replaces offer parameters in text.
        /// </summary>
        /// <param name="textToBeProcessed">Text to be processed.</param>
        /// <param name="parameters">Parameters to be used as values.</param>
        /// <returns>Replaced text.</returns>
        public static string ReplaceParameters(string textToBeProcessed, IDictionary<string, string> parameters)
        {
            foreach (var item in parameters)
            {
                textToBeProcessed = textToBeProcessed.Replace(string.Format("{{{0}}}", item.Key), item.Value);
            }

            return textToBeProcessed;
        }

        /// <summary>
        /// Generates the main text.
        /// </summary>
        /// <param name="data">Authentication data.</param>
        /// <param name="mainRawText">Raw text to modify.</param>
        /// <returns>Returns modified text.</returns>
        public static string GenerateMainText(AuthenticationDataItem data, string mainRawText)
        {
            if (string.IsNullOrEmpty(data.DateOfBirth))
            {
                return null;
            }

            var parameters = GetParameters(data.Identifier);

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

            mainRawText = ReplaceParameters(mainRawText, parameters);

            // some specific
            mainRawText = mainRawText.Replace("{DATE_TO}", data.ExpDateFormatted);

            return mainRawText;
        }
    }
}
