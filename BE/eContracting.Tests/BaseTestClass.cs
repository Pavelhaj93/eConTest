using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public abstract class BaseTestClass
    {
        public string CreateGuid()
        {
            return Guid.NewGuid().ToString("N").ToUpperInvariant();
        }

        public OfferModel CreateOffer()
        {
            return CreateOffer(this.CreateGuid());
        }

        public OfferModel CreateOffer(Guid guid)
        {
            return CreateOffer(guid.ToString("N").ToUpperInvariant());
        }

        public OfferModel CreateOffer(int version)
        {
            return CreateOffer(this.CreateGuid(), false, version, "3");
        }

        public OfferModel CreateOffer(string guid)
        {
            return CreateOffer(guid, false, 2);
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version)
        {
            return CreateOffer(guid, isAccepted, version, "3");
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version, string state)
        {
            return CreateOffer(guid, isAccepted, version, state, "31.12.2021", null);
        }

        public OfferModel CreateOffer(bool isAccepted, int version)
        {
            return CreateOffer(this.CreateGuid(), isAccepted, version, "3");
        }

        public OfferModel CreateOffer(bool isAccepted, int version, string state)
        {
            return CreateOffer(this.CreateGuid(), isAccepted, version, state, "31.12.2021", null);
        }

        public OfferModel CreateOffer(string guid, bool isAccepted, int version, string state, string validTo, OfferAttributeModel[] attributes)
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, state, validTo);
            var offer = new OfferModel(offerXml, version, offerHeader, isAccepted, false, attributes ?? new OfferAttributeModel[] { });
            return offer;
        }

        public UserCacheDataModel CreateAnonymousUser(OfferModel offer)
        {
            var user = new UserCacheDataModel();
            user.AuthorizedGuids[offer.Guid] = AUTH_METHODS.NONE;
            return user;
        }

        public UserCacheDataModel CreateTwoSecretsUser(OfferModel offer)
        {
            var user = new UserCacheDataModel();
            user.AuthorizedGuids[offer.Guid] = AUTH_METHODS.TWO_SECRETS;
            return user;
        }

        public UserCacheDataModel CreateCognitoUser(OfferModel offer)
        {
            var tokens = new OAuthTokensModel("accessToken", "idToken", "refreshToken", "lukas.dvorak@actumdigital.com");
            var cognitoData = this.CreateCognitoData();
            var cognito = new CognitoUserModel(cognitoData);
            var user = new UserCacheDataModel();
            user.Tokens = tokens;
            user.CognitoUser = cognito;
            user.AuthorizedGuids[offer.Guid] = AUTH_METHODS.COGNITO;
            return user;
        }

        public CognitoUserDataModel CreateCognitoData()
        {
            var model = new CognitoUserDataModel();
            model.Username = "ae974d48-7307-4228-a624-3ce3a1070091";
            model.UserAttributes = new List<CognitoUserDataModel.UserAttribute>()
            {
                new CognitoUserDataModel.UserAttribute() { Name = "sub", Value = "ae974d48-7307-4228-a624-3ce3a1070091" },
                new CognitoUserDataModel.UserAttribute() { Name = "custom:segment", Value = "B2C_Customers" },
                new CognitoUserDataModel.UserAttribute() { Name = "email_verified", Value = "true" },
                new CognitoUserDataModel.UserAttribute() { Name = "preferred_username", Value = "ae974d48-7307-4228-a624-3ce3a1070091" },
                new CognitoUserDataModel.UserAttribute() { Name = "given_name", Value = "Lukáš" },
                new CognitoUserDataModel.UserAttribute() { Name = "family_name", Value = "Dvořák" },
                new CognitoUserDataModel.UserAttribute() { Name = "email", Value = "lukas.dvorak@actumdigital.com" },

            }.ToArray();
            return model;
        }

        public CognitoSettingsModel CreateCognitoSettings()
        {
            string cognitoBaseUrl = "http://cognito.amazon.com";
            string cognitoClientId = "jfm40b2mlsagh822g"; 
            string cognitoCookiePrefix = "CognitoCookie"; 
            string cognitoCookieUser = "LastAuthUser"; 
            string innogyLoginUrl = "http://test.innogy.cz/login"; 
            string innogyLogoutUrl = "http://test.innogy.cz/logout"; 
            string innogyRegistrationUrl = "http://test.innogy.cz/registration";
            string innogyDashboardUrl = "http://test.innogy.cz/dashboard";

            var model = new CognitoSettingsModel(cognitoBaseUrl, cognitoClientId, cognitoCookiePrefix, cognitoCookieUser, innogyLoginUrl, innogyLogoutUrl, innogyRegistrationUrl, innogyDashboardUrl);
            return model;
        }

        public CognitoUserModel CreateCognitoUserModel()
        {
            string sub = "lkjg840292j8f3";
            IEnumerable<string> customSegments = new List<string>() { "B2C_Customers", "B2B_Customers" };
            bool emailVerified = true;
            string preferredUsername = "blazena_z_hostic";
            string givenName = "Blažena";
            string familyName = "Škopková";
            string email = "blazena.skopkova@hostice.cz";
            string username = "lkjg840292j8f3";

            var model = new CognitoUserModel(sub, customSegments, emailVerified, preferredUsername, givenName, familyName, email, username);
            return model;
        }

        public CognitoUserDataModel CreateCognitoUserDataModel()
        {
            var model = new CognitoUserDataModel();
            model.UserAttributes = new List<CognitoUserDataModel.UserAttribute>()
            {
                new CognitoUserDataModel.UserAttribute() { Name = "sub", Value = "ae974d48-7307-4228-a623-3ce3a1070090" },
                new CognitoUserDataModel.UserAttribute() { Name = "custom:segment", Value = "B2C_Customers" },
                new CognitoUserDataModel.UserAttribute() { Name = "email_verified", Value = "true" },
                new CognitoUserDataModel.UserAttribute() { Name = "preferred_username", Value = "ae974d48-7307-4228-a623-3ce3a1070091" },
                new CognitoUserDataModel.UserAttribute() { Name = "given_name", Value = "Lukáš" },
                new CognitoUserDataModel.UserAttribute() { Name = "family_name", Value = "Dvořák" },
                new CognitoUserDataModel.UserAttribute() { Name = "email", Value = "lukas.dvorak@actumdigital.com" }
            }.ToArray();
            model.Username = "ae974d48-7307-4228-a623-3ce3a1070093";
            return model;
        }
    }
}
