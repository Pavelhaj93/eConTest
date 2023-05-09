using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.DependencyInjection;
using Sitecore.Layouts;
using Sitecore.Mvc.Controllers;
using Sitecore.SecurityModel;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContracting2AuthController : eContracting2MvcController
    {
        [Obsolete]
        private const string salt = "228357";

        protected readonly IDataSessionCacheService SessionCacheService;
        protected readonly ILoginFailedAttemptBlockerStore LoginReportService;
        protected readonly IEventLogger EventLogger;
        protected readonly ITextService TextService;
        private const string SESSION_ERROR_KEY = "INVALID_LOGIN";
        private const string loginMatrixCombinationPlaceholderPrefix = "/eContracting2Main/eContracting2-login";

        [ExcludeFromCodeCoverage]
        public eContracting2AuthController() : base(
            ServiceLocator.ServiceProvider.GetRequiredService<ILogger>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IUserService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISessionProvider>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IDataRequestCacheService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IMvcContext>())
        {
            this.LoginReportService = ServiceLocator.ServiceProvider.GetRequiredService<ILoginFailedAttemptBlockerStore>();
            this.EventLogger = ServiceLocator.ServiceProvider.GetRequiredService<IEventLogger>();
            this.TextService = ServiceLocator.ServiceProvider.GetRequiredService<ITextService>();
        }

        [ExcludeFromCodeCoverage]
        public eContracting2AuthController(
            ILogger logger,
            IContextWrapper contextWrapper,
            IOfferService offerService,
            ISessionProvider sessionProvider,
            IUserService userService,
            ISettingsReaderService settingsReader,
            ILoginFailedAttemptBlockerStore loginReportService,
            IMvcContext mvcContext,
            IEventLogger evetLogger,
            ITextService textService,
            IDataRequestCacheService requestCacheService) : base(logger, contextWrapper, userService, settingsReader, sessionProvider, requestCacheService, offerService, mvcContext)
        {
            this.LoginReportService = loginReportService ?? throw new ArgumentNullException(nameof(loginReportService));
            this.EventLogger = evetLogger ?? throw new ArgumentNullException(nameof(evetLogger));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
        }

        /// <summary>
        /// Authentication GET action. For authentication Cognito user, see <see cref="LayoutViewModel.InitializeUser(string, Sitecore.Mvc.Presentation.Rendering, bool)"/>.
        /// </summary>
        /// <seealso cref="LayoutViewModel"/>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            var guid = this.GetGuid();
            var errorString = string.Empty;

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.LoginEdit();
                }

                if (string.IsNullOrEmpty(guid))
                {
                    return this.GetLoginFailReturns(LOGIN_STATES.INVALID_GUID, guid);
                }

                // if is re-new session requested
                var renewSessionResult = this.RenewSession();

                if (renewSessionResult != null)
                {
                    return renewSessionResult;
                }

                if (!string.IsNullOrEmpty(this.SessionProvider.GetValue<string>(SESSION_ERROR_KEY)))
                {
                    errorString = this.SessionProvider.GetValue<string>(SESSION_ERROR_KEY);
                    this.SessionProvider.Remove(SESSION_ERROR_KEY);    ////After error page refresh user will get general validation error message
                }

                // gets offer as anonymous in case logged user cannot read it under his state
                var offer = this.OfferService.GetOffer(guid);

                if (offer == null)
                {
                    this.Logger.Info(guid, "Offer doesn't exist (invalid guid)");
                    return this.GetLoginFailReturns(LOGIN_STATES.OFFER_NOT_FOUND, guid);
                }

                // Data were set in LayoutViewModel.Initialize()
                // User could be logged in as Cognito
                var user = this.UserService.GetUser();
                // Only Cognito user may not be able to read it
                var canReadOffer = this.OfferService.CanReadOffer(guid, user, OFFER_TYPES.QUOTPRX);
                // User must be authorized for current guid
                var isAuthorizedForOffer = this.UserService.IsAuthorized(user, guid);
                // Do not allow auto login if Cognito logged-in
                var stopAutoLogin = this.Request.QueryString[Constants.QueryKeys.DO_NOT_AUTO_LOGIN] == Constants.QueryValues.DO_NOT_AUTO_LOGIN_TRUE;

                if (isAuthorizedForOffer && canReadOffer && !stopAutoLogin)
                {
                    var campaignCode = this.GetCampaignCode(offer);
                    this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.LOGIN);
                    this.ClearFailedAttempts(guid, user, AUTH_RESULT_STATES.SUCCEEDED, null, campaignCode);

                    if (this.ContextWrapper.GetQueryValue("s") == "o" || offer.Version < 3)
                    {
                        return this.RedirectWithNewSession(PAGE_LINK_TYPES.Offer, guid, true);
                    }

                    return this.RedirectWithNewSession(PAGE_LINK_TYPES.Summary, guid, true);
                }

                // When Cognito cannot read the offer (hide innosvět login) and keep him on login page
                var showInnogyAccount = canReadOffer && offer.HasMcfu;
                var showInnogyAccountHideInfo = canReadOffer ? false : user.IsCognito;

                if (user.RemoveAuth(guid))
                {
                    this.Logger.Debug(guid, "Removing current guid as orphan from AuthorizedGuids");
                }

                // continue on login page

                var datasource = this.MvcContext.GetPageContextItem<IPageLoginModel>();
                var canLogin = this.IsAbleToLogin(guid, offer, datasource);

                if (canLogin != LOGIN_STATES.OK)
                {
                    return this.GetLoginFailReturns(canLogin, guid);
                }

                if (offer.State == "3")
                {
                    this.OfferService.ReadOffer(guid, user);
                }

                var authTypes = this.SettingsService.GetLoginTypes(offer);

                if (!authTypes.Any())
                {
                    this.Logger.Fatal(guid, $"No authentication types found ({Constants.ErrorCodes.AUTH1_MISSING_AUTH_TYPES})");
                    return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_MISSING_AUTH_TYPES);
                }

                var definition = this.SettingsService.GetDefinition(offer);
                var choices = authTypes.Select(x => this.GetChoiceViewModel(x, offer)).ToArray();
                var steps = this.GetSteps(user, offer, datasource, definition);
                var viewModel = this.GetViewModel(definition, datasource, choices, steps, errorString);
                viewModel.OfferAccepted = offer.IsAccepted;
                viewModel.Birthdate = offer.Birthday;
                viewModel.BussProcess = offer.Process;
                viewModel.BussProcessType = offer.ProcessType;
                viewModel.Partner = offer.PartnerNumber;
                viewModel.Zip1 = offer.PostNumber;
                viewModel.Zip2 = offer.PostNumberConsumption;
                viewModel.HideInnogyAccount = !showInnogyAccount;
                viewModel.ShowInnogyAccountHideInfo = showInnogyAccountHideInfo;

                if (!viewModel.HideInnogyAccount)
                {
                    viewModel.ViewEventData = this.GetViewData(offer, datasource, definition);
                    viewModel.ClickEventData = this.GetClickData(offer, datasource, definition);
                }

                // check if there already is an component in place for this matrix combination (possibly generated whenever an editor opens the page in edit mode)
                // if it is not there, do not place a "Placeholders" element with matrix combination and let the view render the default component
                // checking ContextItem which is null when running tests
                if (this.MvcContext.ContextItem != null)
                {
                    var renderings = this.MvcContext.ContextItem.Visualization.GetRenderings(Sitecore.Context.Device, true);
                    var layoutField = new LayoutField(this.MvcContext.ContextItem.Fields[FieldIDs.LayoutField]);
                    LayoutDefinition layoutDef = LayoutDefinition.Parse(layoutField.Value);
                    DeviceDefinition deviceDef = layoutDef.GetDevice(Sitecore.Context.Device.ID.ToString());
                    string combinationIdentifier = "_" + offer.Process + "_" + offer.ProcessType;

                    if (renderings.Any(rend => rend.Placeholder.Equals(loginMatrixCombinationPlaceholderPrefix + combinationIdentifier)))
                    {
                        viewModel.Placeholders.Add(combinationIdentifier);
                    }
                }

                return View("/Areas/eContracting2/Views/Login.cshtml", viewModel);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is EndpointNotFoundException)
                {
                    this.Logger.Fatal(guid, $"Connection to CACHE failed ({Constants.ErrorCodes.AUTH1_CACHE})", ex);
                    return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_CACHE);
                }

                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_CACHE2})", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_APP})", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_APP);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_INV_OP})", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH1_UNKNOWN})", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH1_UNKNOWN);
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
            var guid = this.GetGuid();
            //var datasource = this.GetLayoutItem<LoginComponentModel>();

            try
            {
                //if (!this.ModelState.IsValid)
                //{
                //    Log.Debug($"[{guid}] Invalid log-in offer", this);
                //    //TODO: this.ReportLogin(reportDateOfBirth, reportAdditionalValue, authenticationModel.Key, guid, offerTypeIdentifier, generalError: true);
                //    var url = Request.RawUrl.Contains("&error=validationError") ? this.Request.RawUrl : this.Request.RawUrl + "&error=validationError";
                //    return Redirect(url);
                //}

                if (string.IsNullOrEmpty(guid))
                {
                    this.ReportLogin(LOGIN_STATES.INVALID_GUID, guid, Request.QueryString["utm_campaign"]);
                    return this.GetLoginFailReturns(LOGIN_STATES.INVALID_GUID, guid);
                }

                this.UserService.Logout(guid);
                var user = this.UserService.GetUser();
                
                var datasource = this.MvcContext.GetPageContextItem<IPageLoginModel>();
                var offer = this.OfferService.GetOffer(guid);
                var campaignCode = this.GetCampaignCode(offer);

                var canLogin = this.IsAbleToLogin(guid, offer, datasource);
                
                if (canLogin != LOGIN_STATES.OK)
                {                    
                    this.ReportLogin(canLogin, guid, campaignCode);
                    return this.GetLoginFailReturns(canLogin, guid);
                }

                var loginType = this.GetLoginType(offer, authenticationModel.Key);
                var result = this.GetLoginState(offer, loginType, authenticationModel.BirthDate, authenticationModel.Key, authenticationModel.Value);

                if (result != AUTH_RESULT_STATES.SUCCEEDED)
                {
                    this.Logger.Info(guid, $"Log-in failed");
                    this.ReportLogin(result, loginType, guid, campaignCode);
                    return this.GetLoginFailReturns(result, loginType, guid);
                }

                user.SetAuth(guid, AUTH_METHODS.TWO_SECRETS);
                this.UserService.Authenticate(guid, user); // overwrites current store user
                this.Logger.Debug(guid, $"Logged user [POST]: {JsonConvert.SerializeObject(user, Formatting.Indented)}");
                this.EventLogger.Add(this.SessionProvider.GetId(), guid, EVENT_NAMES.LOGIN);
                this.ClearFailedAttempts(guid, user, result, loginType, campaignCode);

                if (offer.Version < 3)
                {
                    return this.RedirectWithNewSession(PAGE_LINK_TYPES.Offer, guid);
                }

                return this.RedirectWithNewSession(PAGE_LINK_TYPES.Summary, guid);

            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is EndpointNotFoundException)
                {
                    this.Logger.Error(guid, $"Connection to CACHE failed ({Constants.ErrorCodes.AUTH2_CACHE})", ex);
                    return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH2_CACHE);
                }

                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_CACHE2})", ex);
                return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH2_CACHE2);
            }
            catch (ApplicationException ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_APP})", ex);
                return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH2_APP);
            }
            catch (EcontractingMissingDatasourceException ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_INV_OP})", ex);
                return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH2_INV_OP);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Authenticating failed ({Constants.ErrorCodes.AUTH2_UNKNOWN})", ex);
                return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.AUTH2_UNKNOWN);
            }
        }

        public ActionResult RenewSession()
        {
            var guid = this.GetGuid();
            var state = this.ContextWrapper.GetQueryValue(Constants.QueryKeys.RENEW_SESSION);

            if (state == null || state.Length == 0)
            {
                return null;
            }

            if (this.Request.Url.Query.Contains(Constants.QueryKeys.LOOP_PROTECTION))
            {
                this.Logger.Debug(guid, $"Renew session skipped - Loop protection activated.");
            }

            try
            {
                string COOKIE_NAME = "econtracting-check";
                string AES_KEY = "2DCE45D005B34E6D928F593982891B8C";
                string AES_VECTOR = "ABB7762F3ACF1C31";

                if (state == "0")
                {
                    var user = this.UserService.GetUser();
                    this.Logger.Debug(guid, $"Renew session - user: {JsonConvert.SerializeObject(user, Formatting.Indented)}");
                    this.SessionProvider.RefreshSession();
                    if (user.IsCognito)
                    {
                        user.Tokens = null;
                        user.CognitoUser = null;
                        this.Logger.Debug(guid, $"Renew session - Cognito offer were deleted to handle cookie 4096 char length restrictions.");
                    }                    
                    var encryptedUser = Utils.AesEncrypt(user, AES_KEY, AES_VECTOR);
                    this.ContextWrapper.SetCookie(new HttpCookie(COOKIE_NAME, encryptedUser) { Expires = DateTime.Now.AddMinutes(1) });
                    var redirectUrl = Utils.SetQuery(this.Request.Url, Constants.QueryKeys.RENEW_SESSION, "1");
                    this.Logger.Debug(guid, $"Renew session - {redirectUrl}");
                    return this.View("/Areas/eContracting2/Views/LoginRedirect.cshtml", new LoginRedirectViewModel(redirectUrl, 100));                    
                    //return this.Redirect(redirectUrl.ToString());
                }

                if (state == "1")
                {
                    var cookie = this.Request.Cookies[COOKIE_NAME];
                    var decryptedUser = Utils.AesDecrypt<UserCacheDataModel>(cookie.Value, AES_KEY, AES_VECTOR);
                    if (decryptedUser.IsCognitoGuid(guid))
                    {
                        if (!this.UserService.TryAuthenticateUser(guid, decryptedUser))
                        {
                            var redirectLogin = this.Redirect(PAGE_LINK_TYPES.Login, guid, null, true);                            
                            var redirectLoginUrl = Utils.SetQuery(redirectLogin.Url, Constants.QueryKeys.LOOP_PROTECTION, "1");
                            this.Logger.Debug(guid, $"Renew session - loop protection - {redirectLoginUrl}");
                            return this.Redirect(redirectLoginUrl);
                        }
                    }
                    this.Logger.Debug(guid, $"Renew session - decryptedUser: {JsonConvert.SerializeObject(decryptedUser, Formatting.Indented)}");
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    this.UserService.Authenticate(guid, decryptedUser);
                    var redirectUrl = this.Request.QueryString[Constants.QueryKeys.REDIRECT];
                    redirectUrl = Utils.SetQuery(redirectUrl, Constants.QueryKeys.GUID, this.Request.QueryString[Constants.QueryKeys.GUID]);
                    this.Logger.Debug(guid, $"Renew session: {redirectUrl}");
                    return this.View("/Areas/eContracting2/Views/LoginRedirect.cshtml", new LoginRedirectViewModel(redirectUrl, 100));
                    //return this.Redirect(redirectUrl);
                }

                return null;
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(guid, "Problem with renewing session", ex);
                var redirectUrl = this.Request.QueryString[Constants.QueryKeys.REDIRECT];
                if (!redirectUrl.Contains(Constants.QueryKeys.LOOP_PROTECTION))
                {
                    redirectUrl = Utils.SetQuery(redirectUrl, Constants.QueryKeys.LOOP_PROTECTION, "1");
                    this.Logger.Debug(guid, $"Renew session - loop protection - {redirectUrl}");
                }                

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    return this.Redirect(redirectUrl);
                }

                var url = Utils.RemoveQuery(this.Request.Url, Constants.QueryKeys.RENEW_SESSION);
                return this.Redirect(url.ToString());
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var guid = this.GetGuid();

            try
            {
                this.UserService.Logout(null);

                this.SessionProvider.RefreshSession();

                var redirectUrl = this.Request.QueryString.Get(Constants.QueryKeys.REDIRECT);

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = HttpUtility.UrlDecode(redirectUrl);
                }

                if (Uri.IsWellFormedUriString(redirectUrl, UriKind.RelativeOrAbsolute))
                {
                    return this.Redirect(redirectUrl);
                }
                else if (string.IsNullOrEmpty(guid))
                {
                    return this.Redirect(PAGE_LINK_TYPES.SessionExpired, null);
                }
                else
                {
                    return this.Redirect(PAGE_LINK_TYPES.Login, guid);
                }
            }
            catch (EcontractingMissingDatasourceException ex)
            {
                var redirectUrl = "/login";
                this.Logger.Error(guid, ex);
                return this.Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, ex);
                return this.Redirect(PAGE_LINK_TYPES.SessionExpired, guid);
            }
        }

        private ActionResult LoginEdit()
        {
            var data = this.OfferService.GetOffer(Constants.FakeOfferGuid);
            var definition = this.SettingsService.GetDefinition(data.Process, data.ProcessType);
            var user = new UserCacheDataModel();
            var fakeHeader = new OfferHeaderModel("XX", Guid.NewGuid().ToString("N"), "3", "");
            var fakeXml = new OfferXmlModel();
            fakeXml.Content = new OfferContentXmlModel();
            fakeXml.Content.Body = new OfferBodyXmlModel();
            fakeXml.Content.Body.BusProcess = definition.Process.Code;
            fakeXml.Content.Body.BusProcessType = definition.ProcessType.Code;
            var fakeAttr = new OfferAttributeModel[] { };
            var fakeOffer = new OffersModel(fakeHeader.CCHKEY, new[] { new OfferModel(fakeXml, 1, fakeHeader, true, false, DateTime.Now.AddDays(-1), fakeAttr) });
            var datasource = this.MvcContext.GetPageContextItem<IPageLoginModel>();
            var loginTypes = this.SettingsService.GetLoginTypes(fakeOffer);
            var choices = loginTypes.Select(x => this.GetChoiceViewModel(x, fakeOffer)).ToArray();
            var steps = this.GetSteps(user, fakeOffer, datasource, definition);
            var editModel = this.GetViewModel(definition, datasource, choices, steps);
            editModel.Birthdate = DateTime.Now.ToString("dd.MM.yyyy");
            editModel.PageTitle = datasource.PageTitle;
            editModel.Partner = "1234567890";
            editModel.Zip1 = "191 000";
            editModel.Zip2 = "192 000";
            editModel.BussProcess = definition.Process.Code;
            editModel.BussProcessType = definition.ProcessType.Code;

            var processes = this.SettingsService.GetAllProcesses();
            var processTypes = this.SettingsService.GetAllProcessTypes();

            editModel.Placeholders.Add(loginMatrixCombinationPlaceholderPrefix);

            if (this.ContextWrapper.IsEditMode())
            {
                try
                {
                    if (datasource.AutoGenerateTestableCombinationPlaceholders)
                    {
                        var renderings = this.MvcContext.ContextItem.Visualization.GetRenderings(Sitecore.Context.Device, true);
                        var layoutField = new LayoutField(this.MvcContext.ContextItem.Fields[FieldIDs.LayoutField]);
                        LayoutDefinition layoutDef = LayoutDefinition.Parse(layoutField.Value);
                        DeviceDefinition deviceDef = layoutDef.GetDevice(Sitecore.Context.Device.ID.ToString());


                        foreach (var process in processes)
                        {
                            foreach (var type in processTypes)
                            {
                                string combinationIdentifier = "_" + process.Code + "_" + type.Code;
                                
                                var combinationRendering = renderings.FirstOrDefault(rend => rend.Placeholder.Equals(loginMatrixCombinationPlaceholderPrefix + combinationIdentifier));
                                if (combinationRendering == null)
                                {
                                    var contextItem = this.MvcContext.ContextItem;

                                    RenderingDefinition newRenderingDefinition = new RenderingDefinition();
                                    newRenderingDefinition.ItemID = "{994F14EA-C859-4055-BB0E-523CE476057A}"; // TODO: sc config?  	Rich Text for login page
                                    newRenderingDefinition.Placeholder = loginMatrixCombinationPlaceholderPrefix + combinationIdentifier;

                                    deviceDef.AddRendering(newRenderingDefinition);
                                }
                            }
                        }
                        using (new SecurityDisabler())
                        {
                            this.MvcContext.ContextItem.Editing.BeginEdit();
                            layoutField.Value = layoutDef.ToXml();
                            this.MvcContext.ContextItem.Editing.EndEdit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Error autogenerating Login page placeholders for A/B testing for missing matrix combinations. ", ex, this);                    
                }

                return View("/Areas/eContracting2/Views/Edit/Login.cshtml", editModel);
            }

            return View("/Areas/eContracting2/Views/Preview/Login.cshtml", editModel);
        }

        /// <summary>
        /// Rendering for '/sitecore/layout/Renderings/eContracting2/Rich Text for login page'.
        /// </summary>
        public ActionResult RichText()
        {
            var guid = this.GetGuid();
            var dataSource = this.MvcContext.GetDataSourceItem<IRichTextModel>();
            var offer = this.OfferService.GetOffer(guid);

            if (dataSource == null || offer.IsAccepted)
            {
                var definition = this.SettingsService.GetDefinition(offer.Process, offer.ProcessType);

                if (!offer.IsAccepted)
                {
                    dataSource = definition.MainTextLogin;
                }
                else
                {
                    if (dataSource != null)
                    {
                        // in case that this matrix combination has an active AB test (indended for non-accepted offer login text),
                        // variants of the component with specified datasource are rendered even when the offer is accepted, these are different variants of non-accepted text (MainTextLogin)
                        // but we dont want to AB test the MainTextLoginAccepted. 
                        // recognize if the placeholder name is something like /eContracting2Main/eContracting2-login_00_B (ends with 00_B) and if the offer is accepted, ignore the explicitely set datasource then
                        string placeholderName = Sitecore.Mvc.Presentation.RenderingContext.CurrentOrNull?.Rendering?.Placeholder ?? string.Empty;
                        if (placeholderName.EndsWith($"{offer.Process}_{offer.ProcessType}"))
                            dataSource = definition.MainTextLoginAccepted;
                    }
                    else
                    {
                        dataSource = definition.MainTextLoginAccepted;
                    }                     
                }                
            }

            if (this.ContextWrapper.IsNormalMode())
            {
                dataSource.Text = Utils.GetReplacedTextTokens(dataSource.Text, offer.TextParameters);
            }

            return View("/Areas/eContracting2/Views/LoginRichText.cshtml", dataSource);
        }

        /// <summary>
        /// Reports invalid login prerequisites.
        /// </summary>
        /// <param name="loginState">State of the login.</param>
        /// <param name="guid">The unique identifier.</param>
        protected internal void ReportLogin(LOGIN_STATES loginState, string guid, string campaignCode)
        {            
            var model = new LoginFailureModel(guid, this.SessionProvider.GetId());
            model.BrowserAgent = this.ContextWrapper.GetBrowserAgent();
            model.LoginState = loginState;
            model.CampaignCode = campaignCode;

            this.LoginReportService.Add(model);
        }

        protected internal void ReportLogin(AUTH_RESULT_STATES authResultState, ILoginTypeModel loginType, string guid, string campaignCode)
        {
            if (authResultState == AUTH_RESULT_STATES.INVALID_BIRTHDATE)
            {
                var model = new LoginFailureModel(guid, this.SessionProvider.GetId());
                model.BrowserAgent = this.ContextWrapper.GetBrowserAgent();
                model.LoginType = loginType;
                model.IsBirthdateValid = false;
                model.IsValueValid = true;
                model.CampaignCode = campaignCode;

                this.LoginReportService.Add(model);
            }
            else if (authResultState == AUTH_RESULT_STATES.INVALID_VALUE)
            {
                var model = new LoginFailureModel(guid, this.SessionProvider.GetId());
                model.BrowserAgent = this.ContextWrapper.GetBrowserAgent();
                model.LoginType = loginType;
                model.IsBirthdateValid = true;
                model.IsValueValid = false;
                model.CampaignCode = campaignCode;

                this.LoginReportService.Add(model);
            }
            else if (authResultState == AUTH_RESULT_STATES.INVALID_BIRTHDATE_AND_VALUE)
            {
                var model = new LoginFailureModel(guid, this.SessionProvider.GetId());
                model.BrowserAgent = this.ContextWrapper.GetBrowserAgent();
                model.LoginType = loginType;
                model.IsBirthdateValid = false;
                model.IsValueValid = false;
                model.CampaignCode = campaignCode;

                this.LoginReportService.Add(model);
            }
            else if (authResultState == AUTH_RESULT_STATES.SUCCEEDED)
            {
                var model = new LoginFailureModel(guid, this.SessionProvider.GetId());
                model.BrowserAgent = this.ContextWrapper.GetBrowserAgent();
                model.LoginType = loginType;
                model.IsBirthdateValid = true;
                model.IsValueValid = true;
                model.CampaignCode = campaignCode;

                this.LoginReportService.Add(model);
            }
        }

        protected internal LoginViewModel GetViewModel(IDefinitionCombinationModel definition, IPageLoginModel datasource, LoginChoiceViewModel[] choices, IStepsModel steps, string validationMessage = null)
        {
            var viewModel = new LoginViewModel(definition, datasource, new StepsViewModel(steps), choices);
            viewModel.FormAction = this.Request.RawUrl;
            viewModel.Labels = new Dictionary<string, object>();
            viewModel.Labels["requiredFields"] = datasource.RequiredFields;
            viewModel.Labels["birthDate"] = datasource.BirthDateLabel;
            viewModel.Labels["birthDateHelpText"] = datasource.BirthDateHelpMessage;
            viewModel.Labels["birthDatePlaceholder"] = datasource.BirthDatePlaceholder;
            viewModel.Labels["verificationMethod"] = datasource.VerificationMethodLabel;
            viewModel.Labels["submitBtn"] = datasource.ButtonText;
            viewModel.Labels["ariaOpenCalendar"] = datasource.CalendarOpen;
            viewModel.Labels["ariaNextMonth"] = datasource.CalendarNextMonth;
            viewModel.Labels["ariaPreviousMonth"] = datasource.CalendarPreviousMonth;
            viewModel.Labels["ariaChooseDay"] = datasource.CalendarSelectDay;
            viewModel.Labels["validationError"] = validationMessage;

            if (datasource.InfoLoginBox != null)
            {
                var buttonUrl = datasource.InfoLoginBox.ButtonUrl?.Url;

                if (string.IsNullOrEmpty(buttonUrl) || !Uri.IsWellFormedUriString(buttonUrl, UriKind.Absolute))
                {
                    buttonUrl = this.SettingsService.GetCognitoSettings().InnogyLoginUrl;
                }

                if (Uri.IsWellFormedUriString(buttonUrl, UriKind.RelativeOrAbsolute))
                {
                    viewModel.Labels["innogyAccountHeading"] = datasource.InfoLoginBox.Title;
                    viewModel.Labels["innogyAccountBenefits"] = datasource.InfoLoginBox.Items.Select(x => x.Text).ToArray();
                    viewModel.Labels["innogyAccountBtn"] = datasource.InfoLoginBox.ButtonLabel;

                    var clientId = this.SettingsService.GetCognitoSettings().CognitoClientId;
                    var redirectUrl = this.Request.Url.ToString();
                    var uriBuilder = new UriBuilder(buttonUrl);
                    buttonUrl = Utils.SetQuery(buttonUrl, Constants.QueryKeys.REDIRECT, redirectUrl);
                    buttonUrl = Utils.SetQuery(buttonUrl, "client_id", clientId);
                    viewModel.ExtraInfoBoxButtonUrl = buttonUrl;
                }
                else
                {
                    this.Logger.Error(null, "URL for external login not well formatted");
                }
            }

            return viewModel;
        }

        protected internal LOGIN_STATES IsAbleToLogin(string guid, OffersModel offer, IPageLoginModel datasource)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return LOGIN_STATES.INVALID_GUID;
            }

            if (!this.LoginReportService.IsAllowed(guid, datasource.MaxFailedAttempts, datasource.GetDelayAfterFailedAttemptsTimeSpan()))
            {
                return LOGIN_STATES.USER_BLOCKED;
            }

            if (offer == null)
            {
                return LOGIN_STATES.OFFER_NOT_FOUND;
            }

            if (offer.State == "1")
            {
                return LOGIN_STATES.OFFER_STATE_1;
            }

            if (string.IsNullOrEmpty(offer.Birthday))
            {
                return LOGIN_STATES.MISSING_BIRTHDAY;
            }

            return LOGIN_STATES.OK;
        }

        protected internal ActionResult GetLoginFailReturns(LOGIN_STATES state, string guid)
        {
            if (state == LOGIN_STATES.INVALID_GUID)
            {
                return Redirect(PAGE_LINK_TYPES.WrongUrl, guid, Constants.ErrorCodes.INVALID_GUID);
            }

            if (state == LOGIN_STATES.OFFER_NOT_FOUND)
            {
                var url = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.OFFER_NOT_FOUND;
                return Redirect(PAGE_LINK_TYPES.WrongUrl, guid, Constants.ErrorCodes.OFFER_NOT_FOUND);
            }

            if (state == LOGIN_STATES.USER_BLOCKED)
            {
                return Redirect(PAGE_LINK_TYPES.UserBlocked, guid);
            }

            if (state == LOGIN_STATES.OFFER_STATE_1)
            {
                this.Logger.Warn(guid, $"Offer with renewSessionParameter [1] will be ignored");
                return Redirect(PAGE_LINK_TYPES.WrongUrl, guid, Constants.ErrorCodes.OFFER_STATE_1);
            }

            if (state == LOGIN_STATES.MISSING_BIRTHDAY)
            {
                this.Logger.Warn(guid, $"Attribute BIRTHDT is offer is empty");
                return Redirect(PAGE_LINK_TYPES.WrongUrl, guid, Constants.ErrorCodes.MISSING_BIRTDATE);
            }

            var url1 = Utils.SetQuery(this.Request.Url, "error", Constants.ErrorCodes.UNKNOWN);
            return Redirect(url1);
        }

        protected internal ActionResult GetLoginFailReturns(AUTH_RESULT_STATES state, ILoginTypeModel loginType, string guid)
        {
            var msg = loginType.ValidationMessage;
            ActionResult result = null;

            if (state == AUTH_RESULT_STATES.INVALID_BIRTHDATE)
            {
                var datasource = this.MvcContext.GetPageContextItem<IPageLoginModel>();
                msg = datasource.BirthDateValidationMessage;
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_BIRTHDATE);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.INVALID_VALUE)
            {
                msg = loginType.ValidationMessage;
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_VALUE);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.INVALID_BIRTHDATE_AND_VALUE)
            {
                var datasource = this.MvcContext.GetPageContextItem<IPageLoginModel>();
                msg = datasource.ValidationMessage;
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_BIRTHDATE_AND_VALUE);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.INVALID_BIRTHDATE_DEFINITION)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_BIRTHDATE_DEFINITION);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION)
            {
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.INVALID_VALUE_DEFINITION);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.KEY_MISMATCH)
            {
                msg = this.TextService.ErrorCode("LOGIN-KEY_MISMATCH");
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_MISMATCH);
                result = Redirect(url);
            }
            else if (state == AUTH_RESULT_STATES.KEY_VALUE_MISMATCH)
            {
                msg = this.TextService.ErrorCode("LOGIN-KEY_VALUE_MISMATCH");
                var url = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.KEY_VALUE_MISMATCH);
                result = Redirect(url);
            }
            else
            {
                var url1 = Utils.SetQuery(this.Request.Url, "error", Constants.ValidationCodes.UNKNOWN);
                result = Redirect(url1);
            }

            this.SessionProvider.Set(SESSION_ERROR_KEY, msg);
            return result;
        }

        protected internal LoginChoiceViewModel GetChoiceViewModel(ILoginTypeModel model, OffersModel offer)
        {
            string key = Utils.GetUniqueKey(model, offer);
            var login = new LoginChoiceViewModel(model, key);
            return login;
        }

        /// <summary>
        /// Find <see cref="ILoginTypeModel"/> by <paramref name="offer"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="key">The key.</param>
        /// <returns>Login type or null.</returns>
        protected internal ILoginTypeModel GetLoginType(OffersModel offer, string key)
        {
            var loginTypes = this.SettingsService.GetAllLoginTypes();

            foreach (var loginType in loginTypes)
            {
                if (key == Utils.GetUniqueKey(loginType, offer))
                {
                    return loginType;
                }
            }

            return null;
        }

        /// <summary>
        /// Logins the specified offer.
        /// </summary>
        /// <seealso cref="ILoginTypeModel"/>
        /// <param name="offer">The offer.</param>
        /// <param name="loginType">The login type.</param>
        /// <param name="birthDay">The birth day.</param>
        /// <param name="key">The key of login type.</param>
        /// <param name="value">The value by login type.</param>
        protected internal AUTH_RESULT_STATES GetLoginState(OffersModel offer, ILoginTypeModel loginType, string birthDay, string key, string value)
        {
            #region Perform settings check

            if (loginType == null)
            {
                return AUTH_RESULT_STATES.MISSING_LOGIN_TYPE;
            }

            if (string.IsNullOrEmpty(key))
            {
                return AUTH_RESULT_STATES.KEY_MISMATCH;
            }

            // We can do it this way but it strongly depends on editor what he defines as 'loginType.Name'.
            var originalValue = offer.FirstOrDefault()?.GetValue(loginType.Key)?.Trim().Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(originalValue))
            {
                return AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION;
            }

            var originalBirthdate = offer.Birthday.Trim().Replace(" ", string.Empty).ToLower();

            if (string.IsNullOrEmpty(originalBirthdate))
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE_DEFINITION;
            }

            #endregion

            birthDay = birthDay?.Trim().Replace(" ", string.Empty).ToLower();
            value = value?.Trim().Replace(" ", string.Empty); //.Replace(" ", string.Empty).ToLower();

            var birthdateValid = this.IsBirthDateValid(originalBirthdate, birthDay);
            var valueValid = this.IsValueValid(loginType, originalValue, value);

            if (!birthdateValid && !valueValid)
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE_AND_VALUE;
            }

            if (!birthdateValid)
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE;
            }

            if (!valueValid)
            {
                return AUTH_RESULT_STATES.INVALID_VALUE;
            }

            return AUTH_RESULT_STATES.SUCCEEDED;
        }

        protected internal void ClearFailedAttempts(string guid, UserCacheDataModel user, AUTH_RESULT_STATES result, ILoginTypeModel loginType, string campaignCode)
        {
            //odblokuj neuspesne pokusy (uspesne prihlaseni restartuje limit)
            try
            {
                this.LoginReportService.Clear(guid);
                this.ReportLogin(result, loginType, guid, campaignCode);
            }
            catch (Exception clearex)
            {
                this.Logger.Error(guid, $"Error clearing login attempts", clearex);
            }
        }

        protected internal bool IsRegexValid(ILoginTypeModel loginType, string value)
        {
            if (string.IsNullOrEmpty(loginType.ValidationRegex))
            {
                return true;
            }

            try
            {
                return Regex.IsMatch(value, loginType.ValidationRegex);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected internal bool IsBirthDateValid(string originalValue, string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return false;
            }

            if (originalValue != inputValue)
            {
                return false;
            }

            return true;
        }

        protected internal bool IsValueValid(ILoginTypeModel loginType, string originalValue, string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return false;
            }

            if (!this.IsRegexValid(loginType, inputValue))
            {
                return false;
            }

            if (originalValue != inputValue)
            {
                return false;
            }

            return true;
        }

        [Obsolete]
        protected internal bool CanReadOfferWithoutLogin(UserCacheDataModel user, OfferModel offer)
        {
            if (offer == null)
            {
                return false;
            }

            if (!this.UserService.IsAuthorized(user, offer.Guid))
            {
                return false;
            }

            if (!this.OfferService.CanReadOffer(offer.Guid, user, OFFER_TYPES.QUOTPRX))
            {
                return false;
            }

            if (!user.IsCognito)
            {
                return false;
            }

            if (this.Request.QueryString[Constants.QueryKeys.DO_NOT_AUTO_LOGIN] == Constants.QueryValues.DO_NOT_AUTO_LOGIN_TRUE)
            {
                return false;
            }

            return true;
        }

        protected internal bool CanReadOfferWithoutLogin(UserCacheDataModel user, OffersModel offer)
        {
            if (offer == null)
            {
                return false;
            }

            if (!this.UserService.IsAuthorized(user, offer.Guid))
            {
                return false;
            }

            if (!this.OfferService.CanReadOffer(offer.Guid, user, OFFER_TYPES.QUOTPRX))
            {
                return false;
            }

            if (!user.IsCognito)
            {
                return false;
            }

            if (this.Request.QueryString[Constants.QueryKeys.DO_NOT_AUTO_LOGIN] == Constants.QueryValues.DO_NOT_AUTO_LOGIN_TRUE)
            {
                return false;
            }

            return true;
        }

        protected internal string GetCampaignCode(OffersModel offer)
        {
            string campaignCode = string.Empty;
            var firstOffer = offer.FirstOrDefault();

            if (firstOffer != null)
            {
                campaignCode = firstOffer.IsCampaign ? firstOffer.Campaign : firstOffer.CreatedAt;
            }
            else
            {
                campaignCode = this.Request.QueryString[Constants.QueryKeys.CAMPAIGN];
            }

            return campaignCode;
        }

        protected internal GoogleAnalyticsEvendDataModel GetViewData(OffersModel offer, IPageLoginModel datasource, IDefinitionCombinationModel definition)
        {
            return this.GetGoogleEventData(
                offer,
                datasource.CampaignLabel,
                datasource.IndividualLabel,
                datasource.ElectricityLabel,
                datasource.GasLabel,
                datasource.LoginView_eCat,
                datasource.LoginView_eAct,
                datasource.LoginView_eLab,
                definition
            );
        }

        protected internal GoogleAnalyticsEvendDataModel GetClickData(OffersModel offer, IPageLoginModel datasource, IDefinitionCombinationModel definition)
        {
            return this.GetGoogleEventData(
                offer,
                datasource.CampaignLabel,
                datasource.IndividualLabel,
                datasource.ElectricityLabel,
                datasource.GasLabel,
                datasource.LoginClick_eCat,
                datasource.LoginClick_eAct,
                datasource.LoginClick_eLab,
                definition
            );
        }
    }
}
