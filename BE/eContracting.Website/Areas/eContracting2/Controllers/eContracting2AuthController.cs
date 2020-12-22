using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

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
        protected readonly IContextWrapper ContextWrapper;
        protected readonly ISessionProvider SessionProvider;
        protected readonly IOfferService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly IUserDataCacheService UserDataCache;
        protected readonly ILoginReportStore LoginReportService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IEventLogger EventLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2AuthController"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public eContracting2AuthController()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.ContextWrapper = ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>();
            this.SessionProvider = ServiceLocator.ServiceProvider.GetRequiredService<ISessionProvider>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.AuthService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.UserDataCache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginReportStore>();
            this.EventLogger = ServiceLocator.ServiceProvider.GetRequiredService<IEventLogger>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2AuthController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contextWrapper">The context wrapper.</param>
        /// <param name="apiService">The API service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <param name="loginReportService">The login report service.</param>
        /// <param name="sitecoreContext">The sitecore context.</param>
        /// <param name="renderingContext">The rendering context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// contextWrapper
        /// or
        /// apiService
        /// or
        /// authService
        /// or
        /// settingsReaderService
        /// or
        /// loginReportService
        /// </exception>
        internal eContracting2AuthController(
            ILogger logger,
            IContextWrapper contextWrapper,
            IOfferService apiService,
            ISessionProvider sessionProvider,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService,
            ILoginReportStore loginReportService,
            ISitecoreContext sitecoreContext,
            IRenderingContext renderingContext,
            IUserDataCacheService userDataCache,
            IEventLogger evetLogger) : base(sitecoreContext, renderingContext)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ContextWrapper = contextWrapper ?? throw new ArgumentNullException(nameof(contextWrapper));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.SessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.UserDataCache = userDataCache ?? throw new ArgumentNullException(nameof(userDataCache));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.LoginReportService = loginReportService ?? throw new ArgumentNullException(nameof(loginReportService));
            this.EventLogger = evetLogger ?? throw new ArgumentNullException(nameof(evetLogger));
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

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.LoginEdit();
                }

                guid = Request.QueryString["guid"];

                if (string.IsNullOrEmpty(guid))
                {
                    return this.GetLoginFailReturns(LoginStates.INVALID_GUID, guid);
                }

                //var msg = Request.QueryString["error"];

                //if (!string.IsNullOrEmpty(msg) && msg == "validationError" && !string.IsNullOrEmpty((string)this.Session["ErrorMessage"]))
                //{
                //    errorString = (string)this.Session["ErrorMessage"];
                //    this.Session["ErrorMessage"] = null;    ////After error page refresh user will get general validation error message
                //}

                var offer = this.ApiService.GetOffer(guid);
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

                if (!authTypes.Any())
                {
                    this.Logger.Fatal(guid, $"No authentication types found ({Constants.ErrorCodes.AUTH1_MISSING_AUTH_TYPES})");
                    return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_MISSING_AUTH_TYPES);
                }

                var datasource = this.GetLayoutItem<LoginPageModel>();
                var choices = authTypes.Select(x => this.GetChoiceViewModel(x, offer)).ToArray();
                var steps = this.SettingsReaderService.GetSteps(datasource.Step);
                var viewModel = this.GetViewModel(datasource, choices, steps, errorString);
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
                    this.Logger.Fatal(guid, $"Connection to CACHE failed ({Constants.ErrorCodes.AUTH1_CACHE})", ex);
                    return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_CACHE);
                }

                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_CACHE2})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_APP})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_APP);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_INV_OP})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_UNKNOWN})", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH1_UNKNOWN);
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

                var offer = this.ApiService.GetOffer(guid);
                var canLogin = this.CanLogin(guid, offer);

                if (canLogin != LoginStates.OK)
                {
                    return this.GetLoginFailReturns(canLogin, guid);
                }

                var result = this.AuthService.GetLoginState(offer, authenticationModel.BirthDate, authenticationModel.Key, authenticationModel.Value);

                if (result != AUTH_RESULT_STATES.SUCCEEDED)
                {
                    this.Logger.Info(guid, $"Log-in failed");
                    //TODO: this.ReportLogin(reportTime, reportDateOfBirth, reportAdditionalValue, authenticationModel.SelectedKey, guid, offerTypeIdentifier);
                    //TODO: this.LoginReportService.AddFailedAttempt(guid, this.SessionProvider.GetId(), this.Request.Browser.Browser);
                    return this.GetLoginFailReturns(result, guid);
                }

                //TODO: this.DataSessionStorage.Login(userData);
                this.AuthService.Login(new AuthDataModel(offer));
                this.Logger.Info(guid, $"Successfully log-ged in");

                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.LOGIN);

                if (offer.IsAccepted)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.AcceptedOffer);
                    this.Logger.Info(guid, $"Offer already accepted");
                    return Redirect(redirectUrl);
                }

                if (offer.OfferIsExpired)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.OfferExpired);
                    this.Logger.Info(guid, $"Offer expired");
                    return this.Redirect(redirectUrl);
                }

                return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.Offer));
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is EndpointNotFoundException)
                {
                    this.Logger.Error(guid, $"Connection to CACHE failed ({Constants.ErrorCodes.AUTH2_CACHE})", ex);
                    return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_CACHE);
                }

                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_CACHE2})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_APP})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_APP);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_INV_OP})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_UNKNOWN})", ex);
                return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.AUTH2_UNKNOWN);
            }
        }

        private ActionResult LoginEdit()
        {
            var fakeHeader = new OfferHeaderModel("XX", Guid.NewGuid().ToString("N"), "3", "");
            var fateXml = new OfferXmlModel() { Content = new OfferContentXmlModel() };
            var fakeAttr = new OfferAttributeModel[] { };
            var fakeOffer = new OfferModel(fateXml, 1, fakeHeader, true, fakeAttr);
            var datasource = this.GetLayoutItem<LoginPageModel>();
            var choices = this.SettingsReaderService.GetAllLoginTypes().Select(x => this.GetChoiceViewModel(x, fakeOffer)).ToArray();
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var editModel = this.GetViewModel(datasource, choices, steps);
            editModel.Birthdate = DateTime.Now.ToString("dd.MM.yyyy");
            editModel.BussProcess = "XX";
            editModel.BussProcessType = "YY";
            editModel.PageTitle = datasource.PageTitle;
            editModel.Partner = "1234567890";
            editModel.Zip1 = "190 000";
            editModel.Zip2 = "190 000";

            if (this.ContextWrapper.IsEditMode())
            {
                return View("/Areas/eContracting2/Views/Edit/Login.cshtml", editModel);
            }

            return View("/Areas/eContracting2/Views/Preview/Login.cshtml", editModel);
        }


        /// <summary>
        /// Rendering for '/sitecore/layout/Renderings/eContracting2/Rich Text for login page'.
        /// </summary>
        public ActionResult RichText()
        {
            var dataSource = this.GetDataSourceItem<RichTextModel>();
            var data = this.UserDataCache.Get<OfferIdentifier>(Constants.CacheKeys.OFFER_IDENTIFIER);

            if (dataSource == null)
            {
                var definition = this.SettingsReaderService.GetDefinition(data.Process, data.ProcessType);
                dataSource = data.IsAccepted ? definition.MainTextLoginAccepted : definition.MainTextLogin;
            }

            if (this.ContextWrapper.IsNormalMode())
            {
                dataSource.Text = Utils.GetReplacedTextTokens(dataSource.Text, data.TextParameters);
            }

            return View("/Areas/eContracting2/Views/LoginRichText.cshtml", dataSource);
        }

        protected internal void ReportLogin(bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
        {
            
        }

        protected internal void ReportLogin(string reportTime, bool wrongDateOfBirth, bool wrongAdditionalValue, string additionalValueKey, string guid, string type, bool generalError = false)
        {
            if (generalError)
            {
                this.LoginReportService.AddLoginAttempt(this.SessionProvider.GetId(), reportTime, guid, type, generalError: true);
                return;
            }

            if (wrongAdditionalValue)
            {
                if (additionalValueKey == "identitycardnumber")
                {
                    this.LoginReportService.AddLoginAttempt(this.SessionProvider.GetId(), reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongIdentityCardNumber: true);
                    return;
                }

                if (additionalValueKey == "permanentresidencepostalcode")
                {
                    this.LoginReportService.AddLoginAttempt(this.SessionProvider.GetId(), reportTime, guid, type, birthdayDate: wrongDateOfBirth, WrongResidencePostalCode: true);
                    return;
                }

                if (additionalValueKey == "postalcode")
                {
                    this.LoginReportService.AddLoginAttempt(this.SessionProvider.GetId(), reportTime, guid, type, birthdayDate: wrongDateOfBirth, wrongPostalCode: true);
                    return;
                }
            }
            else
            {
                this.LoginReportService.AddLoginAttempt(this.SessionProvider.GetId(), reportTime, guid, type, birthdayDate: wrongDateOfBirth);
            }
        }

        protected internal LoginViewModel GetViewModel(LoginPageModel datasource, LoginChoiceViewModel[] choices, ProcessStepModel[] steps, string validationMessage = null)
        {
            var viewModel = new LoginViewModel(datasource, new StepsViewModel(steps), choices);
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

        protected internal LoginStates CanLogin(string guid, OfferModel offer)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return LoginStates.INVALID_GUID;
            }

            if (!this.LoginReportService.CanLogin(guid))
            {
                return LoginStates.USER_BLOCKED;
            }

            if (offer == null)
            {
                return LoginStates.OFFER_NOT_FOUND;
            }

            if (offer.State == "1")
            {
                return LoginStates.OFFER_STATE_1;
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
                var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.INVALID_GUID;
                return Redirect(url);
            }

            if (canLogin == LoginStates.OFFER_NOT_FOUND)
            {
                var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.OFFER_NOT_FOUND;
                return Redirect(url);
            }

            if (canLogin == LoginStates.USER_BLOCKED)
            {
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.UserBlocked));
            }

            if (canLogin == LoginStates.OFFER_STATE_1)
            {
                this.Logger.Warn(guid, $"Offer with state [1] will be ignored");
                var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.OFFER_STATE_1;
                return Redirect(url);
            }

            if (canLogin == LoginStates.MISSING_BIRTHDAY)
            {
                this.Logger.Warn(guid, $"Attribute BIRTHDT is offer is empty");
                var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.MISSING_BIRTDATE;
                return Redirect(url);
            }

            var url1 = Utils.SetQuery(this.Request.Url, "error", Constants.ErrorCodes.UNKNOWN);
            return Redirect(url1);
        }

        protected internal ActionResult GetLoginFailReturns(AUTH_RESULT_STATES state, string guid)
        {
            if (state == AUTH_RESULT_STATES.INVALID_BIRTHDATE)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_BIRTHDATE);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_PARTNER || state == AUTH_RESULT_STATES.INVALID_PARTNER_FORMAT)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_PARTNER);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_VALUE)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_VALUE);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_VALUE_FORMAT)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_VALUE_FORMAT);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_VALUE_DEFINITION);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_ZIP1 || state == AUTH_RESULT_STATES.INVALID_ZIP1_FORMAT)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_ZIP1);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.INVALID_ZIP2 || state == AUTH_RESULT_STATES.INVALID_ZIP2_FORMAT)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_ZIP2);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.KEY_MISMATCH)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_MISMATCH);
                return Redirect(url);
            }

            if (state == AUTH_RESULT_STATES.KEY_VALUE_MISMATCH)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_VALUE_MISMATCH);
                return Redirect(url);
            }

            var url1 = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.UNKNOWN);
            return Redirect(url1);
        }
        
        protected internal LoginChoiceViewModel GetChoiceViewModel(LoginTypeModel model, OfferModel offer)
        {
            string key = Utils.GetUniqueKey(model, offer);
            var login = new LoginChoiceViewModel(model, key);
            return login;
        }
    }
}
