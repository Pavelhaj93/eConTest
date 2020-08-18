using System;
using System.Collections.Generic;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;

namespace eContracting.Kernel.Helpers
{
    public class AuthenticationHelper
    {
        private readonly AuthenticationMethod authProcessor;
        private readonly AuthenticationDataSessionStorage authSession;

        public bool IsUserChoice { get; set; }

        public AuthenticationHelper(Offer offer, AuthenticationDataSessionStorage authSessionStorage, bool userChoiceAuthNormal, bool userChoiceAuthRetention, bool userChoiceAuthAcquisition, AuthenticationSettingsModel settings)
        {
            this.authSession = authSessionStorage;

            if (offer == null) throw new NullReferenceException("Offer can not be null");

            bool withSettings = false;

            if (offer.OfferInternal.Body.OfferIsRetention)
            {
                withSettings = userChoiceAuthRetention;
            }
            else if (offer.OfferInternal.Body.OfferIsAquisition)
            {
                withSettings = userChoiceAuthAcquisition;
            }
            else
            {
                withSettings = userChoiceAuthNormal;
            }

            if (withSettings)
            {
                this.authProcessor = new AuthenticationUserChoice(authSessionStorage, offer, settings);
                this.IsUserChoice = true;
            }
            else
            {
                this.authProcessor = new AuthenticationRandomChoice(authSessionStorage, offer);
                this.IsUserChoice = false;
            }
        }

        public AuthenticationDataItem GetUserData()
        {
            return this.authProcessor.GetUserData();
        }

        public void Login(AuthenticationDataItem data)
        {
            this.authSession.Login(data);
        }

        public IEnumerable<AuthenticationSettingsItemModel> GetAvailableAuthenticationFields()
        {
            return this.authProcessor.GetAvailableAuthenticationFields();
        }

        public string GetRealAdditionalValue(string key)
        {
            var tmp = this.authProcessor.GetRealAdditionalValue(key);
            return tmp == null ? tmp : tmp.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();
        }
    }
}
