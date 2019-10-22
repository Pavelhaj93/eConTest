using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.GlassItems.Settings;
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
                if (item.Key == "identificationnumber")
                {
                    if (!string.IsNullOrEmpty(offer.OfferInternal.Body.OrganizationNumber))
                    {
                        validItems.Add(item.Key, item.Value);
                    }
                }

                if (item.Key == "birthdate")
                {
                    if (!string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                    {
                        validItems.Add(item.Key, item.Value);
                    }
                }

                if (item.Key == "eanoreic")
                {
                    if (!string.IsNullOrEmpty(offer.OfferInternal.Body.EanOrAndEic))
                    {
                        validItems.Add(item.Key, item.Value);
                    }
                }
            }
            return validItems;
        }

        public override AuthenticationDataItem GetUserData()
        {
            return base.sessionStorage.GetUserData(this.offer, false);
        }
    }
}
