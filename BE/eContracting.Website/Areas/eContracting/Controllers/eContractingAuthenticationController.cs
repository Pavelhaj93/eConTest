using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using eContracting.Website.Areas.eContracting.Models;
using Sitecore.Collections;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContractingAuthenticationController : BaseController<EContractingAuthenticationTemplate>
    {
        private const string salt = "228357";
        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult Authentication()
        {
            var guid = string.Empty;
            var errorString = string.Empty;

            try
            {
                guid = Request.QueryString["guid"];
                var msg = Request.QueryString["error"];

                if (!string.IsNullOrEmpty(msg) && msg=="validationError" && !string.IsNullOrEmpty((string)this.Session["ErrorMessage"]))
                {
                    errorString = (string)this.Session["ErrorMessage"];
                    this.Session["ErrorMessage"] = null;    ////After error page refresh user will get general validation error message
                }

                if (string.IsNullOrEmpty(guid))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                }

                var client = new RweClient();
                Offer offer = client.GenerateXml(guid);

                if (offer == null || offer.OfferInternal == null)
                {
                    Log.Warn($"[{guid}] No offer found", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (offer.OfferInternal.State == "1")
                {
                    Log.Warn($"[{guid}] Offer with state [1] will be ignored", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (offer.OfferInternal.Body == null)
                {
                    Log.Warn($"[{guid}] Offer is empty", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                {
                    Log.Warn($"[{guid}] Attribute BIRTHDT is offer is empty", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var authSettings = ConfigHelpers.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(
                    offer,
                    new AuthenticationDataSessionStorage(),
                    this.Context.UserChoiceAuthenticationEnabled,
                    this.Context.UserChoiceAuthenticationEnabledRetention,
                    this.Context.UserChoiceAuthenticationEnabledAcquisition,
                    authSettings);
                AuthenticationDataItem userData = authHelper.GetUserData();

                if (!this.Context.WelcomePageEnabled && (offer.OfferInternal.State == "3"))
                {
                    client.ReadOffer(guid);
                }

                Log.Debug($"[{guid}] Offer type is " + Enum.GetName(typeof(OfferTypes), userData.OfferType), this);

                if (this.Request.QueryString["fromWelcome"] != "1" && !userData.IsAccepted)
                {
                    if (
                        (this.Context.WelcomePageEnabled && userData.OfferType == OfferTypes.Default)
                        || (this.Context.WelcomePageEnabledRetention && userData.OfferType == OfferTypes.Retention)
                        || (this.Context.WelcomePageEnabledAcquisition && userData.OfferType == OfferTypes.Acquisition))
                    {
                        UriBuilder welcomeUri = new UriBuilder(ConfigHelpers.GetPageLink(PageLinkType.Welcome).Url);
                        var query = new SafeDictionary<string>();
                        query["guid"] = guid;

                        /// keep GA information
                        if (this.Request.QueryString.AllKeys.Contains("utm_source"))
                        {
                            query["utm_source"] = this.Request.QueryString.Get("utm_source");
                            query["utm_medium"] = this.Request.QueryString.Get("utm_medium");
                            query["utm_campaign"] = this.Request.QueryString.Get("utm_campaign");
                        }

                        welcomeUri.Query = WebUtil.BuildQueryString(query, false);

                        var welcomeRedirectUrl = welcomeUri.Uri.ToString();

                        Log.Debug($"[{guid}] Redirecting to welcome page", this);
                        return Redirect(welcomeRedirectUrl);
                    }
                    else
                    {
                        if (offer.OfferInternal.State == "3")
                        {
                            client.ReadOffer(guid);
                        }
                    }
                }

                var choices = new List<LoginChoiceViewModel>();
                string hiddenValue = null;

                if (authHelper.IsUserChoice)
                {
                    var available = authHelper.GetAvailableAuthenticationFields();

                    foreach (AuthenticationSettingsItemModel item in available)
                    {
                        var choice = new LoginChoiceViewModel();
                        choice.HelpText = item.HelpText;
                        choice.Key = item.Key;
                        choice.Label = item.Label;
                        choice.Placeholder = item.Placeholder;
                        choice.ValidationRegex = item.ValidationRegex;
                        choices.Add(choice);
                    }
                }
                else
                {
                    var choice = new LoginChoiceViewModel();
                    choice.Key = "additional";
                    choice.Label = userData.ItemFriendlyName;
                    choice.Placeholder = this.TryToGetPlaceholder(authSettings, userData.ItemFriendlyName);
                    choice.ValidationRegex = "^.{1,}$"; // any non empty value
                    choices.Add(choice);

                    var value = userData.ItemValue.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();
                    hiddenValue = string.Format("{0}{1}", value, salt);     ////hash + salt
                }

                var contentText = userData.IsAccepted ? this.Context.AcceptedOfferText : this.Context.NotAcceptedOfferText;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                var maintext = textHelper.GetMainText(userData, contentText);

                if (maintext == null)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var viewModel = GetViewModel(userData, errorString);
                viewModel.HiddenValue = hiddenValue;
                viewModel.Choices = choices;

                ViewData["MainText"] = maintext;

                return View("/Areas/eContracting/Views/Authentication.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Authenticating failed", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Processes user's authentication.
        /// </summary>
        /// <param name="authenticationModel">Authentication model.</param>
        /// <returns>Instance result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authentication(AuthenticationModel authenticationModel)
        {
            var guid = Request.QueryString.Get("guid");
            var reportDateOfBirth = false;
            var reportAdditionalValue = false;

            try
            {
                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    Log.Info($"[{guid}] Temporary blocked", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                }


                var client = new RweClient();
                var offer = client.GenerateXml(Request.QueryString["guid"]);

                var authSettings = ConfigHelpers.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(
                    offer,
                    new AuthenticationDataSessionStorage(),
                    this.Context.UserChoiceAuthenticationEnabled,
                    this.Context.UserChoiceAuthenticationEnabledRetention,
                    this.Context.UserChoiceAuthenticationEnabledAcquisition,
                    authSettings);
                var userData = authHelper.GetUserData();

                var dateOfBirthRealValue = userData.DateOfBirth.Trim().Replace(" ", string.Empty).ToLower();                    ////Value from offer
                var dateOfBirthUserValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();           ////Value from user
                var additionalUserValue = authenticationModel.Additional.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();      ////Value from user hashed

                Log.Debug($"[{guid}] Selected type of authentication: {authenticationModel.SelectedKey}", this);

                var additionalRealValue = authHelper.IsUserChoice 
                    ? authHelper.GetRealAdditionalValue(authenticationModel.SelectedKey) 
                    : authenticationModel.ItemValue.Replace(salt, string.Empty);     ////Value from offer hashed - salt

                var validFormatDateOfBirth = (!string.IsNullOrEmpty(dateOfBirthRealValue)) && (!string.IsNullOrEmpty(dateOfBirthUserValue));
                var validFormatAdditinalValue = (!string.IsNullOrEmpty(additionalUserValue)) && (!string.IsNullOrEmpty(additionalRealValue));

                var validFormat = validFormatDateOfBirth && validFormatAdditinalValue;
                var validDateOfBirth = (dateOfBirthUserValue == dateOfBirthRealValue);
                var validAdditionalValue = (additionalUserValue == additionalRealValue);

                var validData = validDateOfBirth && validAdditionalValue;
                var loginReportService = new MongoOfferLoginReportService();
                var reportTime = DateTime.UtcNow.ToString("d.M.yyyy hh:mm:ss");
                var offerTypeIdentifier = userData.IsIndi ?
                    $"{offer.OfferInternal.CreatedAt}" : 
                    $"{offer.OfferInternal.Body.Campaign}";

                if (!validFormat || !validData)
                {
                    Log.Info($"[{guid}] Log-in failed", this);
                    var dateOfBirthValidationMessage = string.Empty;
                    var specificValidationMessage = string.Empty;
                    var validationMessage = this.Context.ValidationMessage;

                    if (!validFormatDateOfBirth)
                    {
                        Log.Info($"[{guid}] Invalid format of date of birth", this);
                        reportDateOfBirth = true;
                        dateOfBirthValidationMessage = this.Context.DateOfBirthValidationMessage;
                    }

                    if (!validAdditionalValue)
                    {
                        Log.Info($"[{guid}] Invalid format of additional value ({authenticationModel.SelectedKey})", this);
                        reportAdditionalValue = true;
                        specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey);
                    }

                    if (!validDateOfBirth)
                    {
                        Log.Info($"[{guid}] Date of birth doesn't match", this);
                        reportDateOfBirth = true;
                        dateOfBirthValidationMessage = this.Context.DateOfBirthValidationMessage;
                    }

                    if (!validAdditionalValue)
                    {
                        Log.Info($"[{guid}] Additional value ({authenticationModel.SelectedKey}) doesn't match", this);
                        reportAdditionalValue = true;
                        specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey);
                    }

                    this.ReportLogin(loginReportService, reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

                    if (reportDateOfBirth && !reportAdditionalValue)
                    {
                        validationMessage = dateOfBirthValidationMessage;
                    }

                    if (!reportDateOfBirth && reportAdditionalValue)
                    {
                        validationMessage = specificValidationMessage;
                    }
                    
                    if (!string.IsNullOrEmpty(validationMessage))
                    {
                        this.Session["ErrorMessage"] = validationMessage;
                    }

                    var siteSettings = ConfigHelpers.GetSiteSettings();
                    var loginsCheckerClient = new LoginsCheckerClient(siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);
                    loginsCheckerClient.AddFailedAttempt(guid, this.Session.SessionID, Request.Browser.Browser);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";

                    return Redirect(url);
                }

                if (!ModelState.IsValid)
                {
                    Log.Debug($"[{guid}] Invalid log-in data", this);
                    this.ReportLogin(loginReportService, reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier, generalError:true);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";
                    return Redirect(url);
                }

                this.ReportLogin(loginReportService, reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

                var aut = new AuthenticationDataSessionStorage();   ////why??? - good question!
                aut.Login(userData);

                Log.Info($"[{guid}] Successfully log-ged in", this);

                if (userData.IsAccepted)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    Log.Info($"[{guid}] Offer already accepted", this);
                    return Redirect(redirectUrl);
                }

                if (userData.OfferIsExpired)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                    Log.Info($"[{guid}] Offer expired", this);
                    return Redirect(redirectUrl);
                }

                return Redirect(Context.NextPageLink.Url);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}] Authentication process failed", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        private void ReportLogin(MongoOfferLoginReportService service, string reportTime, bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
        {
            if (generalError)
            {
                service.AddOfferLoginAttempt(this.Session.SessionID, reportTime, guid, type, generalError: true);
                return;
            }

            if (wrongAdditionalValue)
            {
                if (additionalValueKey == "identitycardnumber")
                {
                    service.AddOfferLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongIdentityCardNumber: true);
                    return;
                }

                if (additionalValueKey == "permanentresidencepostalcode")
                {
                    service.AddOfferLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongResidencePostalCode: true);
                    return;
                }

                if (additionalValueKey == "postalcode")
                {
                    service.AddOfferLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, wrongPostalCode: true);
                    return;
                }
            }
            else
            {
                service.AddOfferLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth);
            }
        }

        private string GetFieldSpecificValidationMessage(AuthenticationSettingsModel authSettings, string key)
        {
            var settingsItem = authSettings.AuthFields.FirstOrDefault(a => a.Key == key);

            if (settingsItem != null)
            {
                return settingsItem.ValidationMessage;
            }

            return this.Context.ValidationMessage;
        }

        private LoginViewModel GetViewModel(AuthenticationDataItem userData, string validationMessage = null)
        {
            var viewModel = new LoginViewModel();
            viewModel.IsRetention = userData.OfferType == OfferTypes.Retention;
            viewModel.IsAcquisition = userData.OfferType == OfferTypes.Acquisition;
            viewModel.FormAction = this.Request.RawUrl;

            viewModel.Labels = new Dictionary<string, string>();
            viewModel.Labels["requiredFields"] = this.Context.RequiredFields;
            viewModel.Labels["birthDate"] = this.Context.BirthDateLabel;
            viewModel.Labels["birthDatePlaceholder"] = this.Context.BirthDatePlaceholder;
            viewModel.Labels["verificationMethod"] = this.Context.VerificationMethodLabel;
            viewModel.Labels["submitBtn"] = this.Context.ButtonText;
            viewModel.Labels["ariaOpenCalendar"] = this.Context.CalendarOpen;
            viewModel.Labels["ariaNextMonth"] = this.Context.CalendarNextMonth;
            viewModel.Labels["ariaPreviousMonth"] = this.Context.CalendarPreviousMonth;
            viewModel.Labels["ariaChooseDay"] = this.Context.CalendarSelectDay;
            viewModel.Labels["validationError"] = validationMessage;

            return viewModel;
        }

        private string TryToGetPlaceholder(AuthenticationSettingsModel settings, string displayName)
        {
            try
            {
                if (displayName.ToLowerInvariant().Contains("psč"))
                {
                    var match = settings.AuthFields.FirstOrDefault(x => x.Label.ToLowerInvariant().Contains("psč"));

                    if (match != null)
                    {
                        return match.Placeholder;
                    }
                }
                else if (displayName.ToLowerInvariant().Contains("číslo"))
                {
                    var match = settings.AuthFields.FirstOrDefault(x => x.Label.ToLowerInvariant().Contains("číslo"));

                    if (match != null)
                    {
                        return match.Placeholder;
                    }
                }
            }
            catch { }

            return displayName;
        }
    }
}
