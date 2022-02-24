using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Controllers;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2Controller : eContracting2MvcController
    {
        protected readonly IDataSessionCacheService Cache;
        protected readonly IOfferService OfferService;
        protected readonly IUserFileCacheService UserFileCache;
        protected readonly ITextService TextService;
        protected readonly IMvcContext MvcContext;

        [ExcludeFromCodeCoverage]
        public eContracting2Controller() : base(
            ServiceLocator.ServiceProvider.GetRequiredService<ILogger>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IUserService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISessionProvider>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IDataRequestCacheService>())
        {
            this.Cache = ServiceLocator.ServiceProvider.GetRequiredService<IDataSessionCacheService>();
            this.OfferService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.UserFileCache = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
            this.TextService = ServiceLocator.ServiceProvider.GetRequiredService<ITextService>();
            this.MvcContext = ServiceLocator.ServiceProvider.GetRequiredService<IMvcContext>();
        }

        [ExcludeFromCodeCoverage]
        public eContracting2Controller(
            ILogger logger,
            IDataSessionCacheService cache,
            IOfferService offerService,
            ISettingsReaderService settingsReader,
            IUserService userService,
            IContextWrapper contextWrapper,
            IUserFileCacheService userFileCache,
            ITextService textService,
            ISessionProvider sessionProvider,
            IDataRequestCacheService dataRequestCacheService,
            IMvcContext mvcContext) : base(logger, contextWrapper, userService, settingsReader, sessionProvider, dataRequestCacheService)
        {
            this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.OfferService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            this.UserFileCache = userFileCache ?? throw new ArgumentNullException(nameof(userFileCache));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
            this.MvcContext = mvcContext ?? throw new ArgumentNullException(nameof(mvcContext));
        }

        /// <summary>
        /// Rendering view for offer.
        /// </summary>
        [HttpGet]
        public ActionResult Offer()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.GetOfferEditView();
                }

                if (!this.UserService.IsAuthorizedFor(guid))
                {
                    this.Logger.Debug(guid, $"Not authorized");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                if (!this.CanRead(guid))
                {
                    this.Logger.Debug(null, $"Session expired");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }
                
                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    this.Logger.Debug(guid, "Offer not loaded (doesn't exist or invalid user)");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                if (offer.IsAccepted)
                {
                    this.Logger.Debug(guid, $"Offer already accepted");
                    return Redirect(PAGE_LINK_TYPES.AcceptedOffer, guid, null, true);
                }

                if (offer.IsExpired)
                {
                    this.Logger.Debug(guid, $"Offer expired");
                    return Redirect(PAGE_LINK_TYPES.OfferExpired, guid, null, true);
                }

                this.OfferService.SignInOffer(guid, user);
                this.SessionProvider.SetTimeout(this.SettingsService.SessionTimeout);

                try
                {
                    this.UserFileCache.Clear(new DbSearchParameters(null, guid, null));
                }
                catch (Exception ex)
                {
                    this.Logger.Error(guid, "Cannot clear user file cache", ex);
                }

                var datasource = this.MvcContext.GetPageContextItem<IPageNewOfferModel>();
                var viewModel = this.GetOfferViewModel(offer, datasource);
                return this.View("/Areas/eContracting2/Views/Offer.cshtml", viewModel);
            }
            catch (EcontractingApplicationException ex)
            {
                if (ex.Error.Code == "OF-SIO-CSS")
                {
                    this.Logger.Error(guid, $"Offer was not signed in ({Constants.ErrorCodes.OFFER_NOT_SIGNED})", ex);
                    return Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.OFFER_NOT_SIGNED);
                }

                this.Logger.Fatal(guid, ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid, ex.Error.Code);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying offer.", ex);
                return this.Redirect(PAGE_LINK_TYPES.SystemError, guid, Constants.ErrorCodes.OFFER_EXCEPTION);
            }
        }

        // AcceptedOfferController
        public ActionResult AcceptedOffer()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.GetAcceptedOfferEditView();
                }

                if (!this.UserService.IsAuthorizedFor(guid))
                {
                    this.Logger.Debug(guid, $"Not authorized");
                    return this.Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                if (!this.CanRead(guid))
                {
                    this.Logger.Debug(guid, $"User cannot read offer");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    this.Logger.Debug(guid, "Offer not loaded (doesn't exist or invalid user)");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                if (!offer.IsAccepted)
                {
                    return Redirect(PAGE_LINK_TYPES.Offer, guid);
                }

                var datasource = this.MvcContext.GetPageContextItem<IPageAcceptedOfferModel>();
                var definition = this.SettingsService.GetDefinition(offer);
                var settings = this.SettingsService.GetSiteSettings();
                var viewModel = new AcceptedOfferViewModel(settings, guid);
                viewModel.Datasource = datasource;
                viewModel.MainText = Utils.GetReplacedTextTokens(definition.OfferAcceptedMainText.Text, offer.TextParameters);
                viewModel["appUnavailableTitle"] = settings.ApplicationUnavailableTitle;
                viewModel["appUnavailableText"] = settings.ApplicationUnavailableText;
                viewModel.AbMatrixCombinationPixelUrl = definition.OfferAcceptedAbMatrixPixel?.Src;

                if (user.IsCognitoGuid(guid))
                {
                    var cognitoSettings = this.SettingsService.GetCognitoSettings();
                    viewModel.ShowDashboardButton = true;
                    viewModel.ButtonDashboardUrl = Utils.SetQuery(cognitoSettings.InnogyDashboardUrl, Constants.QueryKeys.IDENTITY, offer.GdprIdentity);
                }

                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying accepted offer.", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid);
            }
        }

        // ExpirationController
        public ActionResult Expiration()
        {
            string guid = this.GetGuid();

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.GetExpirationEditModel();
                }

                if (!this.UserService.IsAuthorizedFor(guid))
                {
                    this.Logger.Debug(guid, $"Not authorized");
                    return Redirect(PAGE_LINK_TYPES.SessionExpired, guid);
                }

                if (!this.CanRead(guid))
                {
                    this.Logger.Debug(null, $"Session expired");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                var data = this.RequestCacheService.GetOffer(guid);

                if (data == null)
                {
                    this.Logger.Debug(guid, "Offer not loaded (doesn't exist or invalid user)");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                var definition = this.SettingsService.GetDefinition(data.Process, data.ProcessType);
                var datasource = this.MvcContext.GetPageContextItem<IPageExpirationModel>();

                var viewModel = new PageExpirationViewModel();
                viewModel.Datasource = datasource;
                viewModel.MainText = Utils.GetReplacedTextTokens(definition.OfferExpiredMainText.Text, data.TextParameters);
                viewModel.AbMatrixCombinationPixelUrl = definition.OfferExpiredAbMatrixPixel?.Src;
                return View("/Areas/eContracting2/Views/Expiration.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying expiration page", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid);
            }
        }

        // ThankYouController
        public ActionResult ThankYou()
        {
            var guid = this.GetGuid();

            try
            {
                if (!this.ContextWrapper.IsNormalMode())
                {
                    return this.GetThankYouEditModel();
                }

                if (!this.UserService.IsAuthorizedFor(guid))
                {
                    this.Logger.Debug(guid, $"Not authorized");
                    return Redirect(PAGE_LINK_TYPES.SessionExpired, guid);
                }

                if (!this.CanRead(guid))
                {
                    this.Logger.Debug(null, $"Session expired");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                var user = this.UserService.GetUser();
                var offer = this.OfferService.GetOffer(guid, user);

                if (offer == null)
                {
                    this.Logger.Debug(guid, "Offer not loaded (doesn't exist or invalid user)");
                    return Redirect(PAGE_LINK_TYPES.Login, guid);
                }

                if (!offer.IsAccepted)
                {
                    this.Logger.Info(guid, $"Offer is not accepted");
                    return this.Redirect(PAGE_LINK_TYPES.Offer, guid);
                }

                this.ClearUserData(guid);

                var viewModel = this.GetThankYouViewModel(offer, user);
                return this.View("/Areas/eContracting2/Views/ThankYou.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying thank you page", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid);
            }
        }

        // WelcomeRichTextController
        public ActionResult RichText()
        {
            var dataSource = this.MvcContext.GetPageContextItem<IRichTextModel>();
            return View("/Areas/eContracting2/Views/RichText.cshtml", dataSource);
        }

        // UserBlockedController
        public ActionResult UserBlocked()
        {
            var guid = this.GetGuid();

            try
            {
                var datasource = this.MvcContext.GetPageContextItem<IPageUserBlockedModel>();
                return View("/Areas/eContracting2/Views/UserBlocked.cshtml", datasource);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, "Error when displaying user blocked page", ex);
                return Redirect(PAGE_LINK_TYPES.SystemError, guid);
            }
        }

        // eContractingController
        public ActionResult CookieLaw()
        {
            var model = this.SettingsService.GetSiteSettings().CookieLawSettings;

            if (model != null)
            {
                return View("/Areas/eContracting2/Views/CookieLaw.cshtml", model);
            }

            return new EmptyResult();
        }
        
        public ActionResult Disclaimer()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageDisclaimerModel>();

            if (this.ContextWrapper.IsEditMode())
            {
                return View("/Areas/eContracting2/Views/Edit/Disclaimer.cshtml", datasource);
            }

            return View("/Areas/eContracting2/Views/Disclaimer.cshtml", datasource);
        }

        public ActionResult Error404()
        {
            if (this.ContextWrapper.IsEditMode())
            {
                return this.GetError404EditView();
            }

            var code = this.Request.QueryString["code"];
            var datasource = this.MvcContext.GetPageContextItem<IPageError404Model>();

            if (!this.ContextWrapper.IsEditMode())
            {
                var error = Constants.GetErrorDescription(code);
                datasource.PageTitle = datasource.PageTitle.Replace("{CODE}", error);
                datasource.MainText = datasource.MainText.Replace("{CODE}", error);
            }

            return View("/Areas/eContracting2/Views/Error404.cshtml", datasource);
        }

        public ActionResult Error500()
        {
            if (this.ContextWrapper.IsEditMode())
            {
                return this.GetError500EditView();
            }

            var code = this.Request.QueryString["code"];

            var datasource = this.MvcContext.GetPageContextItem<IPageError500Model>();

            if (!this.ContextWrapper.IsEditMode())
            {
                var error = Constants.GetErrorDescription(code);
                datasource.PageTitle = datasource.PageTitle.Replace("{CODE}", error);
                datasource.MainText = datasource.MainText.Replace("{CODE}", error);
            }

            return View("/Areas/eContracting2/Views/Error500.cshtml", datasource);
        }

        public ActionResult SessionExpired()
        {
            var datasouce = this.MvcContext.GetPageContextItem<IPageSessionExpiredModel>();

            if (this.ContextWrapper.IsEditMode())
            {
                return View("/Areas/eContracting2/Views/Edit/SessionExpired.cshtml", datasouce);
            }

            if (this.IsLogoutParam())
            {
                // on demand (link from header) clear any user login session, eg. ensuring privacy on a publicly shared computer
                this.SessionProvider.Abandon();
            }

            return View("/Areas/eContracting2/Views/SessionExpired.cshtml", datasouce);
        }

        public ActionResult Header()
        {
            var guid = this.GetGuid();
            bool authorized = this.UserService.IsAuthorized();
            var datasource = this.MvcContext.GetDataSourceItem<IPageHeaderModel>();
            var viewModel = new HeaderViewModel();
            viewModel.Datasource = datasource;

            if (authorized)
            {
                viewModel.ShowLogoutButton = true;
                var siteSettings = this.SettingsService.GetSiteSettings();
                string redirectUrl = string.Empty;

                var authType = AUTH_METHODS.TWO_SECRETS;
                var user = this.UserService.GetUser();

                if (user.AuthorizedGuids.ContainsKey(guid))
                {
                    authType = user.AuthorizedGuids[guid];
                }
                else
                {
                    authType = AUTH_METHODS.COGNITO;
                }

                if (authType == AUTH_METHODS.TWO_SECRETS)
                {
                    var url = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.Login, guid);
                    redirectUrl = $"{this.Request.Url.Scheme}://{this.Request.Url.Host}/logout?{Constants.QueryKeys.REDIRECT}=" + HttpUtility.UrlEncode(url);
                    redirectUrl = Utils.SetQuery(redirectUrl, Constants.QueryKeys.GUID, guid);
                    viewModel.LogoutUrlLabel = datasource.LogoutLinkLabel;
                }
                else
                {
                    var logout = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.Logout, guid);
                    logout = Utils.SetQuery(logout, Constants.QueryKeys.GUID, guid);
                    var url = this.SettingsService.GetCognitoSettings().InnogyLogoutUrl;
                    redirectUrl = Utils.SetQuery(url, Constants.QueryKeys.REDIRECT, logout);
                    viewModel.LogoutUrlLabel = datasource.LogoutLinkCognitoLabel;
                }

                // show the logout button whenever the user is logged in, except of the logout action likely used on SessionExpired page to logout just after rendering the header component
                viewModel.LogoutUrl = redirectUrl;
            }

            return this.View("/Areas/eContracting2/Views/Shared/Header.cshtml", viewModel);
        }

        public ActionResult Footer()
        {
            var model = this.MvcContext.GetDataSourceItem<IPageFooterModel>();
            return this.View("/Areas/eContracting2/Views/Shared/Footer.cshtml", model);
        }

        public ActionResult PromoBox()
        {
            var datasource = this.MvcContext.GetDataSourceItem<IPromoBoxModel>();

            if (datasource == null)
            {
                return new EmptyResult();
            }

            return this.View("/Areas/eContracting2/Views/PromoBox.cshtml", datasource);
        }

        public ActionResult AccountButtons()
        {
            var guid = this.GetGuid();
            var offer = this.RequestCacheService.GetOffer(guid);
            var settings = this.SettingsService.GetCognitoSettings();
            var datasource = this.MvcContext.GetDataSourceItem<IAccountButtonsModel>();
            var user = this.UserService.GetUser();
            var viewModel = new ButtonsViewModel();
            viewModel.Datasource = datasource;
            viewModel.ShowRegistrationButtons = !user.HasAuth(AUTH_METHODS.COGNITO);
            viewModel.HasTooltip = !string.IsNullOrEmpty(datasource.ButtonLoginAccountTooltip);

            if (viewModel.ShowRegistrationButtons)
            {
                viewModel.ButtonLoginAccountUrl = Utils.SetQuery(settings.InnogyLoginUrl, Constants.QueryKeys.REDIRECT, settings.InnogyDashboardUrl);
                viewModel.ButtonNewAccountUrl = settings.InnogyRegistrationUrl;
            }
            else
            {
                viewModel.ButtonDashboardUrl = Utils.SetQuery(settings.InnogyDashboardUrl, Constants.QueryKeys.IDENTITY, offer.GdprIdentity);
            }

            return View("/Areas/eContracting2/Views/AccountButtons.cshtml", viewModel);
        }

        protected internal OfferViewModel GetOfferViewModel(OfferModel offer, IPageNewOfferModel datasource)
        {
            var definition = this.SettingsService.GetDefinition(offer);
            this.Logger.Info(offer.Guid, "Matrix used: " + definition.Path);
            var steps = this.SettingsService.GetSteps(datasource.Step);
            var siteSettings = this.SettingsService.GetSiteSettings();
            var viewModel = new OfferViewModel(siteSettings, offer.Guid);
            viewModel.PageTitle = definition.OfferTitle.Text;
            viewModel.MainText = definition.OfferMainText.Text;
            viewModel.Steps = new StepsViewModel(steps);
            viewModel.AllowedContentTypes = siteSettings.AllowedDocumentTypesList();
            viewModel.MaxFileSize = siteSettings.SingleUploadFileSizeLimitKBytes * 1024;
            viewModel.MaxGroupFileSize = siteSettings.GroupResultingFileSizeLimitKBytes * 1024;
            viewModel.MaxAllFilesSize = siteSettings.TotalResultingFilesSizeLimitKBytes * 1024;
            viewModel.ThankYouPage = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.ThankYou, offer.Guid);
            viewModel.SessionExpiredPage = siteSettings.SessionExpired.Url;

            if (offer.HasGDPR)
            {
                var GDPRGuid = Utils.AesEncrypt(offer.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);
                viewModel.GdprGuid = GDPRGuid;
                viewModel.GdprUrl = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
            }

            if (definition.OfferSelectedList != null)
            {
                viewModel.List = new ListViewModel(definition.OfferSelectedList);
                viewModel.List.Label = definition.OfferSelectedListLabel;
            }

            viewModel["appUnavailableTitle"] = siteSettings.ApplicationUnavailableTitle;
            viewModel["appUnavailableText"] = siteSettings.ApplicationUnavailableText;
            viewModel["acceptAll"] = this.TextService.FindByKey("MARK_ALL");
            viewModel["acceptOfferTitle"] = definition.OfferAcceptTitle.Text;
            viewModel["acceptOfferHelptext"] = definition.OfferAcceptText.Text;
            viewModel["submitBtn"] = this.TextService.FindByKey("ACCEPTING");
            viewModel["signatureBtn"] = this.TextService.FindByKey("SIGN");
            viewModel["signatureEditBtn"] = this.TextService.FindByKey("MODIFY_SIGNATURE");
            viewModel["signatureModalTitle"] = datasource.SignModalWindowTitle;
            viewModel["signatureModalText"] = datasource.SignModalWindowText;
            viewModel["signatureNote"] = datasource.SignModalWindowNote;
            viewModel["signatureModalConfirm"] = datasource.SignModalWindowConfirmButtonLabel;
            viewModel["signatureModalClear"] = datasource.SignModalWindowClearButtonLabel;
            viewModel["signatureModalError"] = datasource.SignModalWindowGeneralErrorMessage;
            viewModel["signatureModalThumbnailAlt"] = datasource.SignModalWindowThumbnailText;
            viewModel["signatureModalSign"] = datasource.SignModalWindowSign;
            viewModel["signatureModalClose"] = datasource.SignModalWindowClose;
            viewModel["signaturePadAlt"] = datasource.SignModalWindowPenArea;
            viewModel["selectFile"] = this.TextService.FindByKey("SELECT_DOCUMENT");
            viewModel["selectFileHelpText"] = this.TextService.FindByKey("DRAG_&_DROP") + " " + this.TextService.FindByKey("OR");
            viewModel["removeFile"] = this.TextService.FindByKey("REMOVE_DOCUMENT");
            viewModel["fileSize"] = this.TextService.FindByKey("DOCUMENT_SIZE");
            viewModel["selectedFiles"] = this.TextService.FindByKey("SELECTED_DOCUMENTS");
            viewModel["rejectedFiles"] = this.TextService.FindByKey("WRONG_DOCUMENTS");
            viewModel["uploadFile"] = this.TextService.FindByKey("UPLOAD_DOCUMENT");
            viewModel["captureFile"] = this.TextService.FindByKey("PHOTO_&_UPLOAD");
            viewModel["invalidFileTypeError"] = this.TextService.FindByKey("INVALID_DOCUMENT_FORMAT", new Dictionary<string, string>() { { "fileTypes", siteSettings.AllowedDocumentTypesDescription } });
            viewModel["fileExceedSizeError"] = this.TextService.FindByKey("DOCUMENT_TOO_BIG", new Dictionary<string, string>() { { "maxSize", Utils.GetReadableFileSize(siteSettings.SingleUploadFileSizeLimitKBytes * 1024) } });
            viewModel["fileExceedGroupSizeError"] = this.TextService.FindByKey("DOCUMENTS_TOO_BIG", new Dictionary<string, string>() { { "maxSize", Utils.GetReadableFileSize(siteSettings.GroupResultingFileSizeLimitKBytes * 1024) } });
            viewModel["acceptanceModalTitle"] = datasource.ConfirmModalWindowTitle;
            viewModel["acceptanceModalText"] = datasource.ConfirmModalWindowText;
            viewModel["acceptanceModalAccept"] = datasource.ConfirmModalWindowButtonAcceptLabel;
            viewModel["acceptanceModalCancel"] = datasource.ConfirmModalWindowButtonCancelLabel;
            viewModel["acceptanceModalError"] = datasource.ConfirmModalWindowGeneralErrorMessage;
            
            viewModel.AbMatrixCombinationPixelUrl = definition.OfferVisitedAbMatrixPixel?.Src;

            return viewModel;
        }

        protected internal ActionResult GetOfferEditView()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageNewOfferModel>();
            var steps = this.SettingsService.GetSteps(datasource.Step);

            var data = this.RequestCacheService.GetOffer(Constants.FakeOfferGuid);
            var definition = this.SettingsService.GetDefinition(data.Process, data.ProcessType);

            var siteSettings = this.SettingsService.GetSiteSettings();
            var viewModel = new OfferViewModel(siteSettings, Constants.FakeOfferGuid);
            viewModel.Definition = definition;
            viewModel.PageTitle = definition.OfferTitle?.Text;
            viewModel.MainText = definition.OfferMainText?.Text;
            viewModel.Steps = new StepsViewModel(steps);
            viewModel.AllowedContentTypes = siteSettings.AllowedDocumentTypesList();
            viewModel.MaxFileSize = siteSettings.SingleUploadFileSizeLimitKBytes * 1024;
            viewModel.MaxGroupFileSize = siteSettings.GroupResultingFileSizeLimitKBytes * 1024;
            viewModel.MaxAllFilesSize = siteSettings.TotalResultingFilesSizeLimitKBytes * 1024;
            viewModel.ThankYouPage = siteSettings.ThankYou.Url;
            viewModel.SessionExpiredPage = siteSettings.SessionExpired.Url;

            if (this.ContextWrapper.IsEditMode())
            {
                return this.View("/Areas/eContracting2/Views/Edit/Offer.cshtml", viewModel);
            }

            return this.View("/Areas/eContracting2/Views/Preview/Offer.cshtml", viewModel);
        }

        protected internal ActionResult GetAcceptedOfferEditView()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageAcceptedOfferModel>();
            var definition = this.SettingsService.GetDefinitionDefault();
            var settings = this.SettingsService.GetSiteSettings();
            var viewModel = new AcceptedOfferViewModel(settings, Constants.FakeOfferGuid);
            viewModel.Datasource = datasource;
            viewModel.MainText = definition.OfferAcceptedMainText.Text;
            viewModel["appUnavailableTitle"] = "No available";
            viewModel["appUnavailableText"] = "Not available in Experience editor";
            return View("/Areas/eContracting2/Views/Edit/AcceptedOffer.cshtml", viewModel);
        }

        protected internal ThankYouViewModel GetThankYouViewModel(OfferModel offer, UserCacheDataModel user)
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageThankYouModel>();
            var definition = this.SettingsService.GetDefinition(offer);
            var steps = this.SettingsService.GetSteps(datasource.Step);
            var settings = this.SettingsService.GetCognitoSettings();
            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));

            var mainText = definition.MainTextThankYou.Text;

            if (user.IsCognitoGuid(offer.Guid) && definition.MainTextThankYou2?.Text != null)
            {
                var withoutHtml = Utils.StripHtml(definition.MainTextThankYou2.Text);

                if (!string.IsNullOrEmpty(withoutHtml))
                {
                    mainText = definition.MainTextThankYou2.Text;
                }
            }

            viewModel.MainText = Utils.GetReplacedTextTokens(mainText, offer.TextParameters);
            var googleEvendData = this.GetGoogleEventData(offer, datasource, definition);
            viewModel.ScriptParameters = googleEvendData;

            viewModel.AbMatrixCombinationPixelUrl = definition.ThankYouAbMatrixPixel?.Src;

            return viewModel;
        }

        protected internal ActionResult GetThankYouEditModel()
        {
            var offer = this.RequestCacheService.GetOffer(Constants.FakeOfferGuid);
            var datasource = this.MvcContext.GetPageContextItem<IPageThankYouModel>();
            var definition = this.SettingsService.GetDefinition(offer.Process, offer.ProcessType);
            var steps = this.SettingsService.GetSteps(datasource.Step);
            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));
            viewModel.MainText = definition.MainTextThankYou.Text;

            if (this.ContextWrapper.IsEditMode())
            {
                return this.View("/Areas/eContracting2/Views/Edit/ThankYou.cshtml", viewModel);
            }

            return this.View("/Areas/eContracting2/Views/ThankYou.cshtml", viewModel);
        }

        protected internal ActionResult GetExpirationEditModel()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageExpirationModel>();
            var viewModel = new PageExpirationViewModel();
            viewModel.Datasource = datasource;

            if (this.ContextWrapper.IsPreviewMode())
            {
                return View("/Areas/eContracting2/Views/Expiration.cshtml", viewModel);
            }

            return View("/Areas/eContracting2/Views/Edit/Expiration.cshtml", viewModel);
        }

        protected internal ActionResult GetError404EditView()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageError404Model>();
            return View("/Areas/eContracting2/Views/Edit/Error404.cshtml", datasource);
        }

        protected internal ActionResult GetError500EditView()
        {
            var datasource = this.MvcContext.GetPageContextItem<IPageError500Model>();
            return View("/Areas/eContracting2/Views/Edit/Error500.cshtml", datasource);
        }

        protected internal void ClearUserData(string guid)
        {
            try
            {
                var searchFilesParams = new DbSearchParameters(null, guid, null);
                this.UserFileCache.Clear(searchFilesParams);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, "Cannot clear user file cache", ex);
            }
        }

        protected internal GoogleAnalyticsEvendDataModel GetGoogleEventData(OfferModel offer, IPageThankYouModel datasource, IDefinitionCombinationModel definition)
        {
            return this.GetGoogleEventData(
                offer,
                datasource.CampaignLabel,
                datasource.IndividualLabel,
                datasource.ElectricityLabel,
                datasource.GasLabel,
                datasource.GoogleAnalytics_eCat,
                definition.Process.GoogleAnalytics_eAct,
                definition.ProcessType.GoogleAnalytics_eLab,
                definition);
        }

        protected internal bool IsLogoutParam()
        {
            string logoutParm = this.Request.QueryString["logout"];
            return !string.IsNullOrEmpty(logoutParm) && logoutParm.ToLower() != "false";
        }
    }
}
