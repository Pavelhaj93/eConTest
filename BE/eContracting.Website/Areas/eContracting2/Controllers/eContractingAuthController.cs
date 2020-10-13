using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using eContracting.Models;
using eContracting.Services;
using eContracting.Services.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Collections;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor.RichTextEditor;
using Sitecore.Web;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContractingAuthController : GlassController
    {
        [Obsolete]
        private const string salt = "228357";
        protected readonly ILogger Logger;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ILoginReportStore LoginReportService;
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContractingAuthController"/> class.
        /// </summary>
        public eContractingAuthController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginReportStore>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eContractingAuthController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="apiService">The API service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="loginReportService">The login report service.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// apiService
        /// or
        /// authService
        /// or
        /// settingsReaderService
        /// or
        /// loginReportService
        /// </exception>
        public eContractingAuthController(
            ILogger logger,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService,
            ILoginReportStore loginReportService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.LoginReportService = loginReportService ?? throw new ArgumentNullException(nameof(loginReportService));
        }

        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult Authentication()
        {
            var guid = string.Empty;
            var errorString = string.Empty;
            var datasource = this.GetLayoutItem<EContractingAuthenticationTemplate>();

            try
            {
                guid = Request.QueryString["guid"];
                var msg = Request.QueryString["error"];

                if (!string.IsNullOrEmpty(msg) && msg == "validationError" && !string.IsNullOrEmpty((string)this.Session["ErrorMessage"]))
                {
                    errorString = (string)this.Session["ErrorMessage"];
                    this.Session["ErrorMessage"] = null;    ////After error page refresh user will get general validation error message
                }

                var offer = this.ApiService.GetOffer(guid, "NABIDKA");
                var canLogin = this.CanLogin(guid, offer);

                if (canLogin != AuthLoginStates.OK)
                {
                    return this.GetFailReturns(canLogin, guid);
                }

                if (offer.State == "3")
                {
                    var res = this.ApiService.ReadOffer(guid);
                }

                var authTypes = this.SettingsReaderService.GetLoginTypes(offer);
                var choices = authTypes.Select(x => this.GetChoiceViewModel(x, offer));
                var viewModel = this.GetViewModel(datasource, choices, errorString);
                return View("/Areas/eContracting2/Views/Login.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Authenticating failed", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Processes user's authentication.
        /// </summary>
        /// <param name="authenticationModel">Authentication model.</param>
        /// <returns>Instance result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authentication(LoginSubmitViewModel authenticationModel)
        {
            var guid = Request.QueryString.Get("guid");
            var reportDateOfBirth = false;
            var reportAdditionalValue = false;
            var datasource = this.GetLayoutItem<EContractingAuthenticationTemplate>();

            try
            {
                if (!this.ModelState.IsValid)
                {
                    Log.Debug($"[{guid}] Invalid log-in data", this);
                    //TODO: this.ReportLogin(reportDateOfBirth, reportAdditionalValue, authenticationModel.Key, guid, offerTypeIdentifier, generalError: true);
                    var url = Request.RawUrl.Contains("&error=validationError") ? this.Request.RawUrl : this.Request.RawUrl + "&error=validationError";
                    return Redirect(url);
                }

                var offer = this.ApiService.GetOffer(guid, "NABIDKA");
                var canLogin = this.CanLogin(guid, offer);

                if (canLogin != AuthLoginStates.OK)
                {
                    return this.GetFailReturns(canLogin, guid);
                }

                var result = this.AuthService.Login(offer, authenticationModel.BirthDate, authenticationModel.Key, authenticationModel.Value);
                
                //var authSettings = ConfigHelpers.GetAuthenticationSettings();
                //var authHelper = new AuthenticationHelper(
                //    offer,
                //    this.DataSessionStorage,
                //    datasource.UserChoiceAuthenticationEnabled,
                //    datasource.UserChoiceAuthenticationEnabledRetention,
                //    datasource.UserChoiceAuthenticationEnabledAcquisition,
                //    authSettings);
                //var userData = authHelper.GetUserData();

                //var dateOfBirthRealValue = userData.DateOfBirth.Trim().Replace(" ", string.Empty).ToLower();                    ////Value from offer
                //var dateOfBirthUserValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();           ////Value from user
                //var additionalUserValue = authenticationModel.Additional.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();      ////Value from user hashed

                //Log.Debug($"[{guid}] Selected type of authentication: {authenticationModel.SelectedKey}", this);

                //var additionalRealValue = authHelper.IsUserChoice 
                //    ? authHelper.GetRealAdditionalValue(authenticationModel.SelectedKey) 
                //    : authenticationModel.ItemValue.Replace(salt, string.Empty);     ////Value from offer hashed - salt

                //var validFormatDateOfBirth = (!string.IsNullOrEmpty(dateOfBirthRealValue)) && (!string.IsNullOrEmpty(dateOfBirthUserValue));
                //var validFormatAdditinalValue = (!string.IsNullOrEmpty(additionalUserValue)) && (!string.IsNullOrEmpty(additionalRealValue));

                //var validFormat = validFormatDateOfBirth && validFormatAdditinalValue;
                //var validDateOfBirth = (dateOfBirthUserValue == dateOfBirthRealValue);
                //var validAdditionalValue = (additionalUserValue == additionalRealValue);

                //var validData = validDateOfBirth && validAdditionalValue;

                //var reportTime = DateTime.UtcNow.ToString("d.M.yyyy hh:mm:ss");
                //var offerTypeIdentifier = userData.IsIndi ?
                //    $"{offer.OfferInternal.CreatedAt}" : 
                //    $"{offer.OfferInternal.Body.Campaign}";

                if (!result.Succeeded)
                {
                    Log.Info($"[{guid}] Log-in failed", this);
                    //var dateOfBirthValidationMessage = string.Empty;
                    //var specificValidationMessage = string.Empty;
                    //var validationMessage = datasource.ValidationMessage;

                    //if (!validFormatDateOfBirth)
                    //{
                    //    Log.Info($"[{guid}] Invalid format of date of birth", this);
                    //    reportDateOfBirth = true;
                    //    dateOfBirthValidationMessage = datasource.BirthDateValidationMessage;
                    //}

                    //if (!validAdditionalValue)
                    //{
                    //    Log.Info($"[{guid}] Invalid format of additional value ({authenticationModel.SelectedKey})", this);
                    //    reportAdditionalValue = true;
                    //    specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey, datasource.ValidationMessage);
                    //}

                    //if (!validDateOfBirth)
                    //{
                    //    Log.Info($"[{guid}] Date of birth doesn't match", this);
                    //    reportDateOfBirth = true;
                    //    dateOfBirthValidationMessage = datasource.BirthDateValidationMessage;
                    //}

                    //if (!validAdditionalValue)
                    //{
                    //    Log.Info($"[{guid}] Additional value ({authenticationModel.SelectedKey}) doesn't match", this);
                    //    reportAdditionalValue = true;
                    //    specificValidationMessage = this.GetFieldSpecificValidationMessage(authSettings, authenticationModel.SelectedKey, datasource.ValidationMessage);
                    //}

                    //this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

                    //if (reportDateOfBirth && !reportAdditionalValue)
                    //{
                    //    validationMessage = dateOfBirthValidationMessage;
                    //}

                    //if (!reportDateOfBirth && reportAdditionalValue)
                    //{
                    //    validationMessage = specificValidationMessage;
                    //}
                    
                    //if (!string.IsNullOrEmpty(validationMessage))
                    //{
                    //    this.Session["ErrorMessage"] = validationMessage;
                    //}

                    this.LoginReportService.AddFailedAttempt(guid, this.Session.SessionID, Request.Browser.Browser);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";

                    return Redirect(url);
                }

                //TODO: this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);

                //this.DataSessionStorage.Login(userData);

                Log.Info($"[{guid}] Successfully log-ged in", this);

                if (offer.IsAccepted)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkTypes.AcceptedOffer);
                    Log.Info($"[{guid}] Offer already accepted", this);
                    return Redirect(redirectUrl);
                }

                if (offer.OfferIsExpired)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkTypes.OfferExpired);
                    Log.Info($"[{guid}] Offer expired", this);
                    return Redirect(redirectUrl);
                }

                return Redirect(datasource.NextPageLink.Url);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}] Authentication process failed", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError));
            }
        }

        protected internal void ReportLogin(bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
        {
            
        }

        protected internal void ReportLogin(string reportTime, bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
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

        protected internal string GetFieldSpecificValidationMessage(AuthenticationSettingsModel authSettings, string key, string defaultMessage)
        {
            var settingsItem = authSettings.AuthFields.FirstOrDefault(a => a.Key == key);

            if (settingsItem != null)
            {
                return settingsItem.ValidationMessage;
            }

            return defaultMessage;
        }

        protected internal LoginViewModel GetViewModel(EContractingAuthenticationTemplate datasource, IEnumerable<LoginChoiceViewModel> choices, string validationMessage = null)
        {
            var viewModel = new LoginViewModel();
            viewModel.FormAction = this.Request.RawUrl;
            viewModel.Choices = choices;
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

        protected internal string TryToGetPlaceholder(AuthenticationSettingsModel settings, string displayName)
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
        [Obsolete]
        protected internal bool CheckWhetherUserIsBlocked(string guid)
        {
            var siteSettings = this.SettingsReaderService.GetSiteSettings();
            return !this.LoginReportService.CanLogin(guid, siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);
        }

        protected AuthLoginStates CanLogin(string guid, OfferModel offer)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return AuthLoginStates.INVALID_GUID;
            }

            if (!this.LoginReportService.CanLogin(guid))
            {
                return AuthLoginStates.USER_BLOCKED;
            }

            if (offer?.Xml == null)
            {
                return AuthLoginStates.NO_OFFER;
            }

            if (offer.State == "1")
            {
                return AuthLoginStates.OFFER_STATE_1;
            }

            if (offer.Xml.Content == null)
            {
                return AuthLoginStates.EMPTY_OFFER;
            }

            if (string.IsNullOrEmpty(offer.Birthday))
            {
                return AuthLoginStates.MISSING_BIRTHDAY;
            }

            return AuthLoginStates.OK;
        }

        protected ActionResult GetFailReturns(AuthLoginStates canLogin, string guid)
        {
            if (canLogin == AuthLoginStates.INVALID_GUID)
            {
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
            }

            if (canLogin == AuthLoginStates.USER_BLOCKED)
            {
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.UserBlocked).Url);
            }

            if (canLogin == AuthLoginStates.NO_OFFER)
            {
                this.Logger.Warn($"[{guid}] No offer found");
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
            }

            if (canLogin == AuthLoginStates.OFFER_STATE_1)
            {
                this.Logger.Warn($"[{guid}] Offer with state [1] will be ignored");
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
            }

            if (canLogin == AuthLoginStates.EMPTY_OFFER)
            {
                Log.Warn($"[{guid}] Offer is empty", this);
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
            }

            if (canLogin == AuthLoginStates.MISSING_BIRTHDAY)
            {
                this.Logger.Warn($"[{guid}] Attribute BIRTHDT is offer is empty");
                return Redirect(ConfigHelpers.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
            }

            return new EmptyResult();
        }
        
        protected LoginChoiceViewModel GetChoiceViewModel(LoginTypeModel model, OfferModel offer)
        {
            var login = new LoginChoiceViewModel(model);
            login.Key = Utils.GetMd5(model.ComputeId + offer.Guid);
            return login;
        }
    }
}
