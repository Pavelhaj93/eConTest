using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="isAccepted"></param>
        /// <param name="version"></param>
        /// <param name="state">Check <see cref="OfferModel.State"/></param>
        /// <param name="validTo"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public OfferModel CreateOffer(string guid, bool isAccepted, int version, string state, string validTo, OfferAttributeModel[] attributes)
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, state, validTo);
            var offer = new OfferModel(offerXml, version, offerHeader, isAccepted, false, DateTime.Now.AddDays(1), attributes ?? new OfferAttributeModel[] { });
            return offer;
        }

        public UserCacheDataModel CreateAnonymousUser(OfferModel offer)
        {
            var user = new UserCacheDataModel();
            user.SetAuth(offer.Guid, AUTH_METHODS.NONE);
            return user;
        }

        public UserCacheDataModel CreateTwoSecretsUser(OfferModel offer)
        {
            var user = new UserCacheDataModel();
            user.SetAuth(offer.Guid, AUTH_METHODS.TWO_SECRETS);
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
            user.SetAuth(offer.Guid, AUTH_METHODS.COGNITO);
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
            string cognitoTokensUrl = "http://cognito-tokens.amazon.com";
            string cognitoClientId = "jfm40b2mlsagh822g"; 
            string cognitoCookiePrefix = "CognitoCookie"; 
            string cognitoCookieUser = "LastAuthUser"; 
            string innogyLoginUrl = "http://test.innogy.cz/login"; 
            string innogyLogoutUrl = "http://test.innogy.cz/logout"; 
            string innogyRegistrationUrl = "http://test.innogy.cz/registration";
            string innogyDashboardUrl = "http://test.innogy.cz/dashboard";

            var model = new CognitoSettingsModel(cognitoBaseUrl, cognitoTokensUrl, cognitoClientId, cognitoCookiePrefix, cognitoCookieUser, innogyLoginUrl, innogyLogoutUrl, innogyRegistrationUrl, innogyDashboardUrl);
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

        public Item CreateItem(ID id)
        {
            var itemDefinition = new ItemDefinition(id, "item", ID.NewID, ID.NewID);
            var itemData = new ItemData(itemDefinition, Language.Parse("en"), Sitecore.Data.Version.First, new FieldList());
            var item = new Item(id, itemData, new MemoryDatabase());
            return item;
        }

        public ZCCH_ST_FILE CreateRootFile()
        {
            string content = "<?xml version=\"1.0\"?><asx:abap xmlns:asx=\"http://www.sap.com/abapxml\" version=\"1.0\"><Nabidka><Header/><Body><GUID>0635F899B3111EED8C8B323695A6A453</GUID><ISU_CONTRACT></ISU_CONTRACT><CONTSTART>20221001</CONTSTART><DATE_TO>20220924</DATE_TO><STATUS>Uvolněno</STATUS><PARTNER>9513257699</PARTNER><NAME_LAST>Pechánek</NAME_LAST><NAME_FIRST>Lukáš</NAME_FIRST><NAME_ORG1></NAME_ORG1><BIRTHDT>11.05.1984</BIRTHDT><IC></IC><EMAIL>pechaneklukas@seznam.cz</EMAIL><PHONE>775633873</PHONE><EXT_UI>27ZG500Z0256975I</EXT_UI><VSTELLE>9300338163</VSTELLE><ANLAGE></ANLAGE><BUAG>826000848849</BUAG><PSC_MS>503 11</PSC_MS><PSC_ADDR>503 11</PSC_ADDR><ACCOUNT_NUMBER></ACCOUNT_NUMBER><GUID_GDPR></GUID_GDPR><BUS_PROCESS>02</BUS_PROCESS><BUS_TYPE>L</BUS_TYPE><Attachments>    <Template><SEQNR>001</SEQNR><IDATTACH>EPO</IDATTACH><TEMPLATE>EPO</TEMPLATE><DESCRIPTION>Informace pro zákazníka – spotřebitele</DESCRIPTION><OBLIGATORY></OBLIGATORY><PRINTED>X</PRINTED><SIGN_REQ></SIGN_REQ><TMST_REQ></TMST_REQ><ADDINFO></ADDINFO><TEMPL_ALC_ID>CRM046E</TEMPL_ALC_ID><PRODUCT>G_START12</PRODUCT><GROUP>COMMODITY</GROUP><GROUP_OBLIGATORY>X</GROUP_OBLIGATORY><ITEM_GUID>0635F899B3111EED8C8B323695A6A453</ITEM_GUID><CONSENT_TYPE>S</CONSENT_TYPE></Template></Attachments></Body></Nabidka></asx:abap>";
            
            var rootFile = new ZCCH_ST_FILE();
            rootFile.MIMETYPE = "text/xml";
            rootFile.FILENAME = "BN_0206308752.xml";
            rootFile.FILECONTENT = Encoding.ASCII.GetBytes(content);
            
            return rootFile;
        }
    }
}
