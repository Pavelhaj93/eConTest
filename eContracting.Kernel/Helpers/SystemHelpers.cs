// <copyright file="SystemHelpers.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using eContracting.Kernel.GlassItems.Settings;
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
        public static IDictionary<string, string> GetParameters(string guid, string additionalInfoDocument)
        {
            var client = new RweClient();
            var text = client.GetTextsXml(guid);
            var parameters = client.GetAllAttributes(text, additionalInfoDocument);

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
        /// <param name="additionalInfoDocument">Code name of the document with additional info.</param>
        /// <returns>Returns modified text.</returns>
        public static string GenerateMainText(AuthenticationDataItem data, string mainRawText, string additionalInfoDocument)
        {
            if (string.IsNullOrEmpty(data.DateOfBirth))
            {
                return null;
            }

            var parameters = GetParameters(data.Identifier, additionalInfoDocument);

            return GenerateMainText(data, parameters, mainRawText);
        }

        /// <summary>
        /// Generates the main text.
        /// </summary>
        /// <param name="data">Authentication data.</param>
        /// <param name="mainRawText">Raw text to modify.</param>
        /// <returns>Returns modified text.</returns>
        public static string GenerateMainText(AuthenticationDataItem data, IDictionary<string, string> parameters, string mainRawText)
        {
            if (string.IsNullOrEmpty(data.DateOfBirth))
            {
                return null;
            }

            try
            {
                var stringBuilder = new StringBuilder();
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

            var defaultParams = GetDefaultParameters(ConfigHelpers.GetGeneralSettings());
            parameters = CheckParameters(parameters, defaultParams);

            mainRawText = ReplaceParameters(mainRawText, parameters);

            // some specific
            mainRawText = mainRawText.Replace("{DATE_TO}", data.ExpDateFormatted);

            return mainRawText;
        }

        private static IDictionary<string, string> GetDefaultParameters(GeneralSettings generalSettings)
        {
            var res = new Dictionary<string, string>();

            if (generalSettings == null)
            {
                Log.Warn("General settings can not be null", typeof(SystemHelpers));
                return res;
            }

            if (!string.IsNullOrEmpty(generalSettings.DefaultSalutation))
            {
                res.Add("CUSTTITLELET", generalSettings.DefaultSalutation);
            }

            return res;
        }

        private static IDictionary<string, string> CheckParameters(IDictionary<string,string> parameters, IDictionary<string,string> defaultParameters)
        {
            if (defaultParameters == null || !defaultParameters.Any())
            {
                return parameters;
            }

            foreach (var defaultParam in defaultParameters)
            {
                if (parameters.ContainsKey(defaultParam.Key))
                {
                    var paramValue = parameters[defaultParam.Key];

                    if (string.IsNullOrEmpty(paramValue))
                    {
                        parameters[defaultParam.Key] = defaultParam.Value;
                    }
                }
                else
                {
                    parameters.Add(defaultParam);
                }
            }

            return parameters;
        }

        /// <summary>
        /// Get the code of attachment related to additional info.
        /// </summary>
        /// <param name="offer">Instance of offer.</param>
        /// <returns>Code of additional info document, or empty string.</returns>
        public static string GetCodeOfAdditionalInfoDocument(Offer offer)
        {
            if (offer == null)
            {
                return string.Empty;
            }

            if (offer.OfferInternal.Body.Attachments == null)
            {
                return string.Empty;
            }

            foreach (var attachment in offer.OfferInternal.Body.Attachments)
            {
                if (attachment.AddInfo == null)
                {
                    continue;
                }

                if (attachment.AddInfo.ToLower() == "x")
                {
                    return attachment.IdAttach;
                }
            }

            return string.Empty;
        }
    }
}
