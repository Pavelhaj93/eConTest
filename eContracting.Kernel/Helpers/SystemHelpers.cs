﻿using System;
using System.Linq;
using Sitecore.Configuration;
using eContracting.Kernel.Utils;
using eContracting.Kernel.Services;

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

        public static string GenerateMainText(string mainRawText)
        {
            var auth = new AuthenticationDataSessionStorage();
            RweClient client = new RweClient();
            var data = auth.GetData();
            var date = client.GenerateXml(data.Identifier);

            var text = client.GetTextsXml(data.Identifier);
            var letterXml = client.GetLetterXml(text);
            var parameters = client.GetAllAtrributes(letterXml);
            foreach (var item in parameters)
            {
                mainRawText = mainRawText.Replace(string.Format("{{{0}}}", item.Key), item.Value);
            }
            if (date.OfferInternal.AcceptedAt != null)
                mainRawText = mainRawText.Replace("{DATE}", date.OfferInternal.AcceptedAt);

            return mainRawText;
        }
    }
}
