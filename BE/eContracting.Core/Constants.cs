﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Sitecore.Data;

namespace eContracting
{
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public const string DatabaseContextConnectionStringName = "eContractingContext";
        public const string TimeStampFormat = "yyyyMMddHHmmss";
        public const string GTMElectricityIdentifier = "8591824";
        public static string FakeOfferGuid => Guid.Empty.ToString("B");
        public static string RootSitePath = "/sitecore/content/eContracting2";
        public const string CHECKED = "X";

        /// <summary>
        /// Used for *_VISIBILITY postfix in text parameters.
        /// </summary>
        public const string HIDDEN = "S";

        public static class OfferAttributes
        {
            public const string COUNTER = "COUNTER";
            public const string MD5 = "CONTENT_MD5_HASH";
            public const string VERSION = "MODELO_OFERTA";
            public const string ACCEPTED_DATE = "ACCEPTED_AT";
            public const string VALID_TO = "VALID_TO";
            public const string ZIDENTITYID = "ZIDENTITYID";
            public const string MCFU_REG_STAT = "MCFU_REG_STAT";
            public const string KEY_GDPR = "KEY_GDPR";
            public const string CAMPAIGN = "CAMPAIGN";

            /// <summary>
            /// Attribute to determinate if show prices (with value X) or not.
            /// </summary>
            public const string NO_PROD_CHNG = "NO_PROD_CHNG";
            /// <summary>
            /// Where the offer comes from.
            /// </summary>
            /// <remarks>
            /// Possible values for this attribute:
            /// <list type="bullet">
            /// <item>1 = from CRM</item>
            /// <item>2 = user created the offer by himself</item>
            /// </list>
            /// </remarks>
            public const string ORDER_ORIGIN = "ORDER_ORIGIN";

            /// <summary>
            /// Different type of offer.
            /// </summary>
            public const string OPPORTUNITY = "OPPTACQ";
        }

        public static class OfferAttributeValues
        {
            public const string VERSION_2 = "01";
            public const string VERSION_3 = "02";
        }

        public static class OfferDefaults
        {
            public const string BUS_PROCESS = "00";
            public const string BUS_PROCESS_TYPE_A = "A";
            public const string BUS_PROCESS_TYPE_B = "B";
            public const string GROUP = "COMMODITY";
        }

        public static class OfferGroups
        {
            public const string COMMODITY = "COMMODITY";
            public const string NONCOMMODITY = "NONCOMMODITY";
            public const string DSL = "DSL";
        }

        public static class FileAttributes
        {
            public const string LABEL = "LINK_LABEL";
            public const string TEMPLATE = "TEMPLATE";
            public const string CHECK_VALUE = "X";
            public const string TYPE = "IDATTACH";
            public const string PRODUCT = "PRODUCT";
            public const string GROUP = "GROUP";
            public const string GROUP_OBLIG = "GROUP_OBLIGATORY";
            public const string OBLIGATORY = "OBLIGATORY";
            public const string PRINTED = "PRINTED";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string SEQUENCE_NUMBER = "SEQNR";
            public const string SIGN_REQ = "SIGN_REQ";
            public const string TMST_REQ = "TMST_REQ";
            public const string ADDINFO = "ADDINFO";
            public const string MAIN_DOC = "MAIN_DOC";
            public const string TEMPL_ALC_ID = "TEMPL_ALC_ID";
            public const string ITEM_GUID = "ITEM_GUID";
            public const string CONSENT_TYPE = "CONSENT_TYPE";
        }

        public static class FileAttributeValues
        {
            public const string TEXT_PARAMETERS = "AD1";
            public const string EXTRA_PARAMETERS_E = "CBE";
            public const string EXTRA_PARAMETERS_P = "CBP";
            public const string SIGN_FILE_IDATTACH = "A10";
            public const string CHECK_VALUE = "X";
            public const string CONSENT_TYPE_P = "P";
            public const string CONSENT_TYPE_S = "S";

            /// <summary>
            /// IDATTACH values for old offers where the value doesn't match with file template.
            /// </summary>
            public static string[] SignFileIdAttachValues = new[] { "PLH", "ELH" };
        }

        public static class FileAttributeDefaults
        {
            public const string GROUP = "OTHER";
            public const int COUNTER = 100;
        }

        public static class OfferTextParameters
        {
            public const string REGISTRATION_LINK = "PERSON_MMB_URL";
        }

        public static class SitecorePaths
        {
            public const string LOGIN_TYPES = "/sitecore/content/eContracting2/Settings/LoginTypes";
            public const string PROCESSES = "/sitecore/content/eContracting2/Settings/Processes";
            public const string PROCESS_TYPES = "/sitecore/content/eContracting2/Settings/ProcessTypes";
            public const string DEFINITIONS = "/sitecore/content/eContracting2/Definitions";
            public const string PRODUCT_INFOS = "/sitecore/content/eContracting2/Settings/Products";
        }

        public static class ErrorCodes
        {
            /// <summary>
            /// User access homepage which technically doesn't exist.
            /// </summary>
            public const string HOMEPAGE = "000";
            /// <summary>
            /// Unknown error.
            /// </summary>
            public const string UNKNOWN = "999";
            /// <summary>
            /// User is blocked for login.
            /// </summary>
            public const string USER_BLOCKED = "UB1";
            /// <summary>
            /// Invalid guid provided (usually empty).
            /// </summary>
            public const string INVALID_GUID = "IG1";
            /// <summary>
            /// Offer not retrieved from SAP.
            /// </summary>
            public const string OFFER_NOT_FOUND = "ONF";
            /// <summary>
            /// Offer state equals to 1 - invalid offer.
            /// </summary>
            public const string OFFER_STATE_1 = "OS1";
            /// <summary>
            /// Missing birtdate in offer (it's required).
            /// </summary>
            public const string MISSING_BIRTDATE = "OMB";

            /// <summary>
            /// The authentication process - unknown exception.
            /// </summary>
            public const string AUTH1_UNKNOWN = "100";

            /// <summary>
            /// Call to CACHE during authentication process.
            /// </summary>
            public const string AUTH1_CACHE = "110";

            /// <summary>
            /// Call to CACHE during authentication process - unknown exception.
            /// </summary>
            public const string AUTH1_CACHE2 = "111";

            public const string AUTH1_MISSING_AUTH_TYPES = "120";

            public const string AUTH1_MISSING_CHOICES = "121";

            /// <summary>
            /// The authentication process - application exception.
            /// </summary>
            public const string AUTH1_APP = "190";

            /// <summary>
            /// The authentication process - invalid operation exception.
            /// </summary>
            public const string AUTH1_INV_OP = "195";

            /// <summary>
            /// The authentication process - unknown exception.
            /// </summary>
            public const string AUTH2_UNKNOWN = "200";

            /// <summary>
            /// Call to CACHE during authentication process.
            /// </summary>
            public const string AUTH2_CACHE = "210";

            /// <summary>
            /// Call to CACHE during authentication process - unknown exception.
            /// </summary>
            public const string AUTH2_CACHE2 = "211";

            /// <summary>
            /// The authentication process - application exception.
            /// </summary>
            public const string AUTH2_APP = "290";

            /// <summary>
            /// The authentication process - invalid operation exception.
            /// </summary>
            public const string AUTH2_INV_OP = "295";

            public const string OFFER_NOT_SIGNED = "350";

            public const string OFFER_EXCEPTION = "390";

            //public static IDictionary<string, string> Descriptions = new Dictionary<string, string>()
            //{
            //    { HOMEPAGE, "Direct access from homepage" },
            //    { UNKNOWN, "Unknown / unexpected error" },
            //    { USER_BLOCKED, "User is blocked" },
            //    { INVALID_GUID, "Requested guid is invalid" },
            //    { OFFER_NOT_FOUND, "Offer was not found" },
            //    { OFFER_STATE_1, "Offer has state 1" },
            //    { MISSING_BIRTDATE, "Missing birthdate value" },
            //    { AUTH1_UNKNOWN, "Unknown error on login page" },
            //    { AUTH1_CACHE, "Issue when connecting to remote endpoint on login page" },
            //    { AUTH1_CACHE2, "Issue with remote endpoint on login page" },
            //    { AUTH1_APP, "Application error on login page" },
            //    { AUTH1_INV_OP, "Invalid operation happend on login page" },
            //    { AUTH2_UNKNOWN, "Unknown / unexpected error while submitting login data" },
            //    { AUTH2_CACHE, "Issue when connecting to remote endpoint while submitting login data" },
            //    { AUTH2_CACHE2, "Issue with remote endpoint while submitting login data" },
            //    { AUTH2_APP, "Application error while submitting login data" },
            //    { AUTH2_INV_OP, "Invalid operation happend while submitting login data" },
            //    { OFFER_NOT_SIGNED, "Offer coudn't be signed" },
            //    { OFFER_EXCEPTION, "Something went wrong on offer page" }
            //};
        }

        public static string GetErrorDescription(string code)
        {
            switch (code)
            {
                case ErrorCodes.HOMEPAGE:
                    return "Invalid initial page";
                case ErrorCodes.USER_BLOCKED:
                    return "User is temporarily blocked due to exceeded";
                case ErrorCodes.INVALID_GUID:
                    return "Invalid unique identifier found";
                case ErrorCodes.OFFER_NOT_FOUND:
                    return "Offer was not found or doesn't exist";
                case ErrorCodes.OFFER_STATE_1:
                    return "Offer has invalid state";
                case ErrorCodes.MISSING_BIRTDATE:
                    return "Missing birth date";
                case ErrorCodes.AUTH1_UNKNOWN:
                    return "Unknown error while getting your data";
                case ErrorCodes.AUTH1_CACHE:
                    return "Cannot get your data or data are invalid";
                case ErrorCodes.AUTH1_CACHE2:
                    return "Cannot verify your data";
                case ErrorCodes.AUTH1_MISSING_AUTH_TYPES:
                    return "Internal error - missing relevant data (auth types)";
                case ErrorCodes.AUTH1_MISSING_CHOICES:
                    return "Internal error - missing relevant data (auth choices)";
                case ErrorCodes.AUTH1_APP:
                    return "Unknown application exception";
                case ErrorCodes.AUTH1_INV_OP:
                    return "Unknown operation exception";
                case ErrorCodes.AUTH2_UNKNOWN:
                    return "Unknown error while authorazing you inputs";
                case ErrorCodes.AUTH2_CACHE:
                    return "Cannot validate your data or data are invalid";
                case ErrorCodes.AUTH2_CACHE2:
                    return "Unknown error while validating your data";
                case ErrorCodes.AUTH2_APP:
                    return "Unknown application exception";
                case ErrorCodes.AUTH2_INV_OP:
                    return "Unknown operation exception";
                default: // case Constants.ErrorCodes.UNKNOWN:
                    return "unknown error";
            }
        }

        public static class ValidationCodes
        {
            public const string UNKNOWN = "v00";
            public const string INVALID_BIRTHDATE = "v01";
            public const string INVALID_VALUE = "v02";
            public const string INVALID_BIRTHDATE_AND_VALUE = "v03";
            public const string INVALID_BIRTHDATE_DEFINITION = "v91";
            public const string INVALID_VALUE_DEFINITION = "v92";
            public const string KEY_MISMATCH = "v95";
            public const string KEY_VALUE_MISMATCH = "v96";
        }

        public static class SessionKeys
        {
            public const string NAME = "ECON-GUID";
        }

        public static class TemplateIds
        {
            public static ID PageHome { get; } = new ID("{652F8E4F-5A5B-484B-9552-7E1A8650644C}");
            public static ID PageSummary { get; } = new ID("{F6909B18-2F78-459B-866B-3CC82F308BA7}");
            public static ID PageLogin { get; } = new ID("{C8C58D58-C5D9-47C2-AEF3-F4DEFCA62A2C}");
            public static ID PageLogout { get; } = new ID("{F6611407-BC55-499F-9E03-1E58AC434335}");
            public static ID PageOffer { get; } = new ID("{456D5421-A2DE-42B4-97E6-A42FC243BF10}");
            public static ID PageOfferAccepted { get; } = new ID("{0F225E4F-AA1E-44CC-91F4-19D4FB5C859C}");
            public static ID PageOfferExpired { get; } = new ID("{220B964C-EA21-4672-92F1-58CCE932BD33}");
            public static ID PageThankYou { get; } = new ID("{4F3B54BA-3DBC-408C-9573-F9F86AC0C3C7}");

            /// <summary>
            /// Determines whether <paramref name="templateId"/> contains data with offer.
            /// </summary>
            /// <param name="templateId">The template identifier.</param>
            public static bool IsOfferPage(ID templateId)
            {
                var ids = new[] { PageLogin, PageOffer, PageOfferAccepted, PageOfferExpired, PageThankYou };
                return ids.Contains(templateId);
            }
        }

        public static class TemplateFields
        {
            public static class Steps
            {
                public static ID Step_Default { get; } = new ID("{AA07168B-5469-4E1F-AD6E-A0D05D359327}");
                public static ID Step_NoLogin { get; } = new ID("{E50A95B7-9FCC-4F81-86AF-8BF2192BE8D4}");
                public static ID Step_DistributorChange { get; } = new ID("{FAF10978-ADE1-4A95-A937-4DFD1AE90E8F}");
                public static ID Step_ProductChange { get; } = new ID("{66AB9C32-C130-4793-A0D8-146CDB36FA0A}");
            }
        }

        public static class CacheKeys
        {
            public const string OFFER_IDENTIFIER = "eContracting.OFFER_IDENTIFIER";
            public const string USER_DATA = "eContracting.USER_DATA";
            public const string OFFER_DATA = "eContracting=OFFER_DATA";
        }

        public static class QueryKeys
        {
            public const string GUID = "guid";
            public const string ERROR_CODE = "code";
            public const string PROCESS = "econ_p";
            public const string PROCESS_TYPE = "econ_pt";
            public const string MATRIX = "econ_m";
            public const string CAMPAIGN = "utm_campaign";
            public const string IDENTITY = "idi";
            public const string DO_NOT_AUTO_LOGIN = "dnat";
            public const string REDIRECT = "redirect";
            public const string GLOBAL_LOGOUT = "global";
            public const string RENEW_SESSION = "renew-session";
            public const string LOOP_PROTECTION = "lp";
        }

        public static class QueryValues
        {
            public const string DO_NOT_AUTO_LOGIN_TRUE = "1";
        }

        public static class JsonDocumentDataModelType
        {
            public const string DOCS_SIGN = "docsSign";
            public const string DOCS_SIGN_E = "docsSign-E";
            public const string DOCS_SIGN_G = "docsSign-G";
            public const string DOCS_SIGN_E_G = "docsSign-E/G";
            public const string DOCS_CHECK_E = "docsCheck-E";
            public const string DOCS_CHECK_G = "docsCheck-G";
            public const string DOCS_CHECK_E_G = "docsCheck-E/G";
        }

        public static class CommodityProductType
        {
            public const string GAS = "G";
            public const string ELECTRICITY = "E";
        }
    }
}
