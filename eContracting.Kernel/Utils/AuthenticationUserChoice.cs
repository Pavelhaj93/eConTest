using System;
using System.Collections.Generic;
using System.Linq;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;

namespace eContracting.Kernel.Utils
{
    public class AuthenticationUserChoice : AuthenticationMethod
    {
        private readonly Offer offer;
        private readonly AuthenticationSettingsModel settings;
        public AuthenticationUserChoice(AuthenticationDataSessionStorage authSessionStorage, Offer offer, AuthenticationSettingsModel settings) : base(authSessionStorage)
        {
            this.offer = offer;
            this.settings = settings;
        }

        public override Dictionary<string,string> GetAvailableAuthenticationFields()
        {
            var res = new Dictionary<string, string>();

            if (this.settings == null || this.settings.authFields == null || !this.settings.authFields.Any())
            {
                throw new InvalidOperationException("Settings can not be null");
            }

            return this.ValidateItemsAgainstOffer(this.offer, settings.authFields);
        }

        private Dictionary<string, string> ValidateItemsAgainstOffer(Offer offer, Dictionary<string, string> items)
        {
            var validItems = new Dictionary<string, string>();

            foreach (var item in items)
            {
                var tmp = this.GetRealAdditionalValue(item.Key);
                if (tmp == null) continue;
                validItems.Add(item.Key, item.Value);
            }

            return validItems;
        }

        public override AuthenticationDataItem GetUserData()
        {
            return base.sessionStorage.GetUserData(this.offer, false);
        }

        public override string GetRealAdditionalValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (key == "postalcode")
            {
                return this.offer.OfferInternal.Body.PscMistaSpotreby;
            }

            if (key == "permanentresidencepostalcode")
            {
                return this.offer.OfferInternal.Body.PscTrvaleBydliste;
            }

            if (key == "identitycardnumber")
            {
                return this.offer.OfferInternal.Body.PARTNER;
            }

            return null;
        }
    }
}
