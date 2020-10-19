using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
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
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Collections;
using Sitecore.DependencyInjection;
using Sitecore.Shell.Applications.ContentEditor.RichTextEditor;
using Sitecore.Web;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContracting2AuthController : GlassController
    {
        [Obsolete]
        private const string salt = "228357";
        protected readonly ILogger Logger;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ILoginReportStore LoginReportService;
        protected readonly ISettingsReaderService SettingsReaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2AuthController"/> class.
        /// </summary>
        public eContracting2AuthController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginReportStore>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2AuthController"/> class.
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
        public eContracting2AuthController(
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
        public ActionResult Login()
        {
            var guid = string.Empty;
            var errorString = string.Empty;
            var datasource = this.GetLayoutItem<LoginComponentModel>();

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

                if (canLogin != LoginStates.OK)
                {
                    //TODO: this.ReportLogin()
                    return this.GetLoginFailReturns(canLogin, guid);
                }

                if (offer.State == "3")
                {
                    var res = this.ApiService.ReadOffer(guid);
                }

                var authTypes = this.SettingsReaderService.GetLoginTypes(offer);
                var choices = authTypes.Select(x => this.GetChoiceViewModel(x, offer));
                var viewModel = this.GetViewModel(datasource, choices, errorString);
                viewModel.Birthdate = offer.Birthday;
                viewModel.BussProcess = offer.Process;
                viewModel.BussProcessType = offer.ProcessType;
                viewModel.Partner = offer.PartnerNumber;
                viewModel.Zip1 = offer.PostNumber;
                viewModel.Zip2 = offer.PostNumberConsumption;
                //TODO: this.ReportLogin()
                return View("/Areas/eContracting2/Views/Login.cshtml", viewModel);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is EndpointNotFoundException)
                {
                    this.Logger.Error($"[{guid}] Connection to CACHE failed ({Constants.ErrorCodes.AUTH1_CACHE})", ex);
                    return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_CACHE);
                }

                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH1_CACHE2})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH1_APP})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_APP);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH1_INV_OP})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH1_UNKNOWN})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_UNKNOWN);
            }
        }

        /// <summary>
        /// Processes user's authentication.
        /// </summary>
        /// <param name="authenticationModel">Authentication model.</param>
        /// <returns>Instance result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginSubmitViewModel authenticationModel)
        {
            var guid = Request.QueryString.Get("guid");
            //var datasource = this.GetLayoutItem<LoginComponentModel>();

            try
            {
                //if (!this.ModelState.IsValid)
                //{
                //    Log.Debug($"[{guid}] Invalid log-in data", this);
                //    //TODO: this.ReportLogin(reportDateOfBirth, reportAdditionalValue, authenticationModel.Key, guid, offerTypeIdentifier, generalError: true);
                //    var url = Request.RawUrl.Contains("&error=validationError") ? this.Request.RawUrl : this.Request.RawUrl + "&error=validationError";
                //    return Redirect(url);
                //}

                var offer = this.ApiService.GetOffer(guid, "NABIDKA");
                var canLogin = this.CanLogin(guid, offer);

                if (canLogin != LoginStates.OK)
                {
                    return this.GetLoginFailReturns(canLogin, guid);
                }

                var result = this.AuthService.GetLoginState(offer, authenticationModel.BirthDate, authenticationModel.Key, authenticationModel.Value);

                if (result != AuthResultState.SUCCEEDED)
                {
                    this.Logger.Info($"[{guid}] Log-in failed");
                    //TODO: this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);
                    //TODO: this.LoginReportService.AddFailedAttempt(guid, this.Session.SessionID, this.Request.Browser.Browser);
                    return this.GetLoginFailReturns(result, guid);
                }

                //TODO: this.DataSessionStorage.Login(userData);
                this.AuthService.Login(new AuthDataModel(guid));
                this.Logger.Info($"[{guid}] Successfully log-ged in");

                if (offer.IsAccepted)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkTypes.AcceptedOffer);
                    this.Logger.Info($"[{guid}] Offer already accepted");
                    return Redirect(redirectUrl);
                }

                if (offer.OfferIsExpired)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PageLinkTypes.OfferExpired);
                    this.Logger.Info($"[{guid}] Offer expired");
                    return this.Redirect(redirectUrl);
                }

                return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.Offer));
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is EndpointNotFoundException)
                {
                    this.Logger.Error($"[{guid}] Connection to CACHE failed ({Constants.ErrorCodes.AUTH2_CACHE})", ex);
                    return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_CACHE);
                }

                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH2_CACHE2})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH2_APP})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_APP);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH2_INV_OP})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Authenticating failed ({Constants.ErrorCodes.AUTH2_UNKNOWN})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_UNKNOWN);
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

        protected internal LoginViewModel GetViewModel(LoginComponentModel datasource, IEnumerable<LoginChoiceViewModel> choices, string validationMessage = null)
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

        protected LoginStates CanLogin(string guid, OfferModel offer)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return LoginStates.INVALID_GUID;
            }

            if (!this.LoginReportService.CanLogin(guid))
            {
                return LoginStates.USER_BLOCKED;
            }

            if (offer?.Xml == null)
            {
                return LoginStates.NO_OFFER;
            }

            if (offer.State == "1")
            {
                return LoginStates.OFFER_STATE_1;
            }

            if (offer.Xml.Content == null)
            {
                return LoginStates.EMPTY_OFFER;
            }

            if (string.IsNullOrEmpty(offer.Birthday))
            {
                return LoginStates.MISSING_BIRTHDAY;
            }

            return LoginStates.OK;
        }

        protected ActionResult GetLoginFailReturns(LoginStates canLogin, string guid)
        {
            if (canLogin == LoginStates.INVALID_GUID)
            {
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.WrongUrl));
            }

            if (canLogin == LoginStates.USER_BLOCKED)
            {
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.UserBlocked));
            }

            if (canLogin == LoginStates.NO_OFFER)
            {
                this.Logger.Warn($"[{guid}] No offer found");
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.WrongUrl));
            }

            if (canLogin == LoginStates.OFFER_STATE_1)
            {
                this.Logger.Warn($"[{guid}] Offer with state [1] will be ignored");
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.WrongUrl));
            }

            if (canLogin == LoginStates.EMPTY_OFFER)
            {
                this.Logger.Warn($"[{guid}] Offer is empty");
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.WrongUrl));
            }

            if (canLogin == LoginStates.MISSING_BIRTHDAY)
            {
                this.Logger.Warn($"[{guid}] Attribute BIRTHDT is offer is empty");
                return Redirect(this.SettingsReaderService.GetPageLink(PageLinkTypes.WrongUrl));
            }

            return new EmptyResult();
        }

        protected ActionResult GetLoginFailReturns(AuthResultState state, string guid)
        {
            if (state == AuthResultState.INVALID_BIRTHDATE)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_BIRTHDATE);
                return Redirect(url);
            }

            if (state == AuthResultState.INVALID_PARTNER)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_PARTNER);
                return Redirect(url);
            }

            if (state == AuthResultState.INVALID_ZIP1)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_ZIP1);
                return Redirect(url);
            }

            if (state == AuthResultState.INVALID_ZIP2)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_ZIP2);
                return Redirect(url);
            }

            if (state == AuthResultState.KEY_MISMATCH)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_MISMATCH);
                return Redirect(url);
            }

            if (state == AuthResultState.KEY_VALUE_MISMATCH)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_VALUE_MISMATCH);
                return Redirect(url);
            }

            var url1 = Utils.SetQuery(this.Request.Url, "error", "v00");
            return Redirect(url1);
        }
        
        protected LoginChoiceViewModel GetChoiceViewModel(LoginTypeModel model, OfferModel offer)
        {
            string key = this.AuthService.GetUniqueKey(model, offer);
            var login = new LoginChoiceViewModel(model, key);
            return login;
        }
    }
}
