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
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Collections;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContractingAuthenticationController : GlassController
    {
        private const string salt = "228357";
        protected readonly IRweClient Client;
        protected readonly ILoginReportStore LoginReportService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IAuthenticationDataSessionStorage DataSessionStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContractingAuthenticationController"/> class.
        /// </summary>
        public eContractingAuthenticationController()
        {
            this.Client = ServiceLocator.ServiceProvider.GetRequiredService<IRweClient>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginReportStore>();
            this.DataSessionStorage = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationDataSessionStorage>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eContractingAuthenticationController"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="loginReportService">The login report service.</param>
        /// <param name="dataSessionStorage">The data session storage.</param>
        /// <exception cref="ArgumentNullException">
        /// client
        /// or
        /// settingsReaderService
        /// or
        /// loginReportService
        /// or
        /// client
        /// </exception>
        public eContractingAuthenticationController(IRweClient client, ISettingsReaderService settingsReaderService, ILoginReportStore loginReportService, IAuthenticationDataSessionStorage dataSessionStorage)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.LoginReportService = loginReportService ?? throw new ArgumentNullException(nameof(loginReportService));
            this.DataSessionStorage = dataSessionStorage ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult Authentication()
        {
            var guid = string.Empty;
            var errorString = string.Empty;
            var datasource = this.GetLayoutItem<EContractingAuthenticationTemplate>();

            try
            {
                guid = this.Request.QueryString["guid"];
                var msg = this.Request.QueryString["error"];

                if (!string.IsNullOrEmpty(msg) && msg=="validationError" && !string.IsNullOrEmpty((string)this.Session["ErrorMessage"]))
                {
                    errorString = (string)this.Session["ErrorMessage"];
                    this.Session["ErrorMessage"] = null;    ////After error page refresh user will get general validation error message
                }

                if (string.IsNullOrEmpty(guid))
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.UserBlocked).Url);
                }

                Offer offer = this.Client.GenerateXml(guid);

                if (offer == null || offer.OfferInternal == null)
                {
                    Log.Warn($"[{guid}] No offer found", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (offer.OfferInternal.State == "1")
                {
                    Log.Warn($"[{guid}] Offer with state [1] will be ignored", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (offer.OfferInternal.Body == null)
                {
                    Log.Warn($"[{guid}] Offer is empty", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                {
                    Log.Warn($"[{guid}] Attribute BIRTHDT is offer is empty", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var authSettings = this.SettingsReaderService.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(
                    offer,
                    this.DataSessionStorage,
                    datasource.UserChoiceAuthenticationEnabled,
                    datasource.UserChoiceAuthenticationEnabledRetention,
                    datasource.UserChoiceAuthenticationEnabledAcquisition,
                    authSettings);
                AuthenticationDataItem userData = authHelper.GetUserData();

                if (!datasource.WelcomePageEnabled && (offer.OfferInternal.State == "3"))
                {
                    this.Client.ReadOffer(guid);
                }

                Log.Debug($"[{guid}] Offer type is " + Enum.GetName(typeof(OfferTypes), userData.OfferType), this);

                if (this.Request.QueryString["fromWelcome"] != "1" && !userData.IsAccepted)
                {
                    if (
                        (datasource.WelcomePageEnabled && userData.OfferType == OfferTypes.Default)
                        || (datasource.WelcomePageEnabledRetention && userData.OfferType == OfferTypes.Retention)
                        || (datasource.WelcomePageEnabledAcquisition && userData.OfferType == OfferTypes.Acquisition))
                    {
                        UriBuilder welcomeUri = new UriBuilder(this.SettingsReaderService.GetPageLink(PageLinkType.Welcome).Url);
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
                            this.Client.ReadOffer(guid);
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

                var contentText = userData.IsAccepted ? datasource.AcceptedOfferText : datasource.NotAcceptedOfferText;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                var maintext = textHelper.GetMainText(this.Client, userData, contentText, this.SettingsReaderService.GetGeneralSettings());

                if (maintext == null)
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var viewModel = GetViewModel(datasource, userData, errorString);
                viewModel.HiddenValue = hiddenValue;
                viewModel.Choices = choices;

                ViewData["MainText"] = maintext;

                return View("/Areas/eContracting/Views/Authentication.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Authenticating failed", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.SystemError).Url);
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
            var datasource = this.GetLayoutItem<EContractingAuthenticationTemplate>();

            try
            {
                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    Log.Info($"[{guid}] Temporary blocked", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.UserBlocked).Url);
                }

                var offer = this.Client.GenerateXml(Request.QueryString["guid"]);

                var authSettings = this.SettingsReaderService.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(
                    offer,
                    this.DataSessionStorage,
                    datasource.UserChoiceAuthenticationEnabled,
                    datasource.UserChoiceAuthenticationEnabledRetention,
                    datasource.UserChoiceAuthenticationEnabledAcquisition,
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

                var reportTime = DateTime.UtcNow.ToString("d.M.yyyy hh:mm:ss");
                var offerTypeIdentifier = userData.IsIndi ?
                    $"{offer.OfferInternal.CreatedAt}" : 
                    $"{offer.OfferInternal.Body.Campaign}";

                if (!validFormat || !validData)
                {
                    Log.Info($"[{guid}] Log-in failed", this);
                    var dateOfBirthValidationMessage = string.Empty;
                    var specificValidationMessage = string.Empty;
                    var validationMessage = datasource.ValidationMessage;

                    if (!validFormatDateOfBirth)
                    {
                        Log.Info($"[{guid}] Invalid format of date of birth", this);
                        reportDateOfBirth = true;
                        dateOfBirthValidationMessage = datasource.BirthDateValidationMessage;
                    }

                    if (!validAdditionalValue)
                    {
                        Log.Info($"[{guid}] Invalid format of additional value ({authenticationModel.SelectedKey})", this);
                        reportAdditionalValue = true;
                        specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey, datasource.ValidationMessage);
                    }

                    if (!validDateOfBirth)
                    {
                        Log.Info($"[{guid}] Date of birth doesn't match", this);
                        reportDateOfBirth = true;
                        dateOfBirthValidationMessage = datasource.BirthDateValidationMessage;
                    }

                    if (!validAdditionalValue)
                    {
                        Log.Info($"[{guid}] Additional value ({authenticationModel.SelectedKey}) doesn't match", this);
                        reportAdditionalValue = true;
                        specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey, datasource.ValidationMessage);
                    }

                    this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

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

                    this.LoginReportService.AddFailedAttempt(guid, this.Session.SessionID, Request.Browser.Browser);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";

                    return Redirect(url);
                }

                if (!ModelState.IsValid)
                {
                    Log.Debug($"[{guid}] Invalid log-in data", this);
                    this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier, generalError:true);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";
                    return Redirect(url);
                }

                this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

                this.DataSessionStorage.Login(userData);

                Log.Info($"[{guid}] Successfully log-ged in", this);

                if (userData.IsAccepted)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    Log.Info($"[{guid}] Offer already accepted", this);
                    return Redirect(redirectUrl);
                }

                if (userData.OfferIsExpired)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkType.OfferExpired).Url;
                    Log.Info($"[{guid}] Offer expired", this);
                    return Redirect(redirectUrl);
                }

                return Redirect(datasource.NextPageLink.Url);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}] Authentication process failed", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult Welcome()
        {
            var guid = string.Empty;
            var datasource = this.GetLayoutItem<EContractingWelcomeTemplate>();

            try
            {
                if (Sitecore.Context.PageMode.IsNormal)
                {
                    guid = Request.QueryString["guid"];

                    if (string.IsNullOrEmpty(guid))
                    {
                        return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                    }

                    if (this.CheckWhetherUserIsBlocked(guid))
                    {
                        return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.UserBlocked).Url);
                    }

                    var offer = this.Client.GenerateXml(guid);

                    if ((offer == null) || (offer.OfferInternal.Body == null) || string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                    {
                        return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.WrongUrl).Url);
                    }

                    if (offer.OfferInternal.State == "3")
                    {
                        this.Client.ReadOffer(guid);
                    }

                    var authenticationData = this.DataSessionStorage.GetUserData(offer, true);

                    datasource.OfferType = authenticationData.OfferType;
                    datasource.IsIndividual = authenticationData.IsIndi;

                    var processingParameters = SystemHelpers.GetParameters(this.Client, guid, authenticationData.OfferType, string.Empty, this.SettingsReaderService.GetGeneralSettings());
                    this.HttpContext.Items["WelcomeData"] = processingParameters;

                    var dataModel = new WelcomeModel() { Guid = guid };

                    dataModel.ButtonText = datasource.ButtonText;

                    return View("/Areas/eContracting/Views/Welcome.cshtml", datasource);
                }
                else
                {
                    //TODO: Remove after check
                    var dataModel = new WelcomeModel() { Guid = string.Empty };
                    dataModel.ButtonText = datasource.ButtonText;

                    return View("/Areas/eContracting/Views/Welcome.cshtml", datasource);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying welcome user page", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [HttpPost]
        public ActionResult WelcomeSubmit(string guid)
        {
            return Redirect(this.SettingsReaderService.GetPageLink(PageLinkType.Login).Url + "?fromWelcome=1&guid=" + guid);
        }

        private void ReportLogin(string reportTime, bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
        {
            if (generalError)
            {
                this.LoginReportService.AddLoginAttempt(this.Session.SessionID, reportTime, guid, type, generalError: true);
                return;
            }

            if (wrongAdditionalValue)
            {
                if (additionalValueKey == "identitycardnumber")
                {
                    this.LoginReportService.AddLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongIdentityCardNumber: true);
                    return;
                }

                if (additionalValueKey == "permanentresidencepostalcode")
                {
                    this.LoginReportService.AddLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongResidencePostalCode: true);
                    return;
                }

                if (additionalValueKey == "postalcode")
                {
                    this.LoginReportService.AddLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth, wrongPostalCode: true);
                    return;
                }
            }
            else
            {
                this.LoginReportService.AddLoginAttempt(this.Session.SessionID, reportTime, guid, type, birthdayDate: wrongDateOfBirth);
            }
        }

        private string GetFieldSpecificValidationMessage(AuthenticationSettingsModel authSettings, string key, string defaultMessage)
        {
            var settingsItem = authSettings.AuthFields.FirstOrDefault(a => a.Key == key);

            if (settingsItem != null)
            {
                return settingsItem.ValidationMessage;
            }

            return defaultMessage;
        }

        private LoginViewModel GetViewModel(EContractingAuthenticationTemplate datasource, AuthenticationDataItem userData, string validationMessage = null)
        {
            var viewModel = new LoginViewModel();
            viewModel.IsRetention = userData.OfferType == OfferTypes.Retention;
            viewModel.IsAcquisition = userData.OfferType == OfferTypes.Acquisition;
            viewModel.FormAction = this.Request.RawUrl;

            viewModel.Labels = new Dictionary<string, string>();
            viewModel.Labels["requiredFields"] = datasource.RequiredFields;
            viewModel.Labels["birthDate"] = datasource.BirthDateLabel;
            viewModel.Labels["birthDatePlaceholder"] = datasource.BirthDatePlaceholder;
            viewModel.Labels["verificationMethod"] = datasource.VerificationMethodLabel;
            viewModel.Labels["submitBtn"] = datasource.ButtonText;
            viewModel.Labels["ariaOpenCalendar"] = datasource.CalendarOpen;
            viewModel.Labels["ariaNextMonth"] = datasource.CalendarNextMonth;
            viewModel.Labels["ariaPreviousMonth"] = datasource.CalendarPreviousMonth;
            viewModel.Labels["ariaChooseDay"] = datasource.CalendarSelectDay;
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

        /// <summary>
        /// Checks whether user is not blocked.
        /// </summary>
        /// <param name="guid">GUID of offer we are checking.</param>
        /// <returns>A value indicating whether user is blocked or not.</returns>
        protected bool CheckWhetherUserIsBlocked(string guid)
        {
            var siteSettings = this.SettingsReaderService.GetSiteSettings();
            return !this.LoginReportService.CanLogin(guid, siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);
        }
    }
}
