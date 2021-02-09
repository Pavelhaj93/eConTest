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

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2Controller : GlassController
    {
        protected readonly ILogger Logger;
        protected readonly IUserDataCacheService Cache;
        protected readonly IOfferService ApiService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IAuthenticationService AuthenticationService;
        protected readonly IContextWrapper Context;
        protected readonly IUserFileCacheService UserFileCache;
        protected readonly ITextService TextService;

        [ExcludeFromCodeCoverage]
        public eContracting2Controller()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.AuthenticationService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>();
            this.UserFileCache = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
            this.TextService = ServiceLocator.ServiceProvider.GetRequiredService<ITextService>();
        }

        [ExcludeFromCodeCoverage]
        public eContracting2Controller(
            ILogger logger,
            IUserDataCacheService cache,
            IOfferService apiService,
            ISettingsReaderService settingsReaderService,
            IAuthenticationService authService,
            IContextWrapper context,
            IUserFileCacheService userFileCache,
            ITextService textService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthenticationService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.UserFileCache = userFileCache ?? throw new ArgumentNullException(nameof(userFileCache));
            this.TextService = textService ?? throw new ArgumentNullException(nameof(textService));
        }

        /// <summary>
        /// Rendering view for offer.
        /// </summary>
        [HttpGet]
        public ActionResult Offer()
        {
            string guid = string.Empty;

            try
            {
                if (!this.Context.IsNormalMode())
                {
                    return this.GetOfferEditView();
                }

                if (!this.AuthenticationService.IsLoggedIn())
                {
                    this.Logger.Debug(guid, $"Session expired");
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired);
                    return Redirect(redirectUrl);
                }

                var user = this.AuthenticationService.GetCurrentUser();
                guid = user.Guid;

                var offer = this.ApiService.GetOffer(guid); //TODO: Or NABIDKA_PDF ?

                if (offer.IsAccepted)
                {
                    this.Logger.Debug(guid, $"Offer already accepted");
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.AcceptedOffer);
                    return Redirect(redirectUrl);
                }

                if (offer.IsExpired)
                {
                    this.Logger.Debug(guid, $"Offer expired");
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.OfferExpired);
                    return Redirect(redirectUrl);
                }

                this.ApiService.SignInOffer(guid);

                try
                {
                    this.UserFileCache.Clear(new DbSearchParameters(null, guid, null));
                }
                catch (Exception ex)
                {
                    this.Logger.Error(guid, "Cannot clear user file cache", ex);
                }

                var datasource = this.GetLayoutItem<PageNewOfferModel>();
                var viewModel = this.GetOfferViewModel(offer, datasource);
                return this.View("/Areas/eContracting2/Views/Offer.cshtml", viewModel);
            }
            catch (EcontractingApplicationException ex)
            {
                if (ex.Error.Code == "OF-SIO-CSS")
                {
                    this.Logger.Error(guid, $"Offer was not signed in ({Constants.ErrorCodes.OFFER_NOT_SIGNED})", ex);
                    return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.OFFER_NOT_SIGNED);
                }

                this.Logger.Fatal(guid, ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + ex.Error.Code);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying offer.", ex);
                var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.OFFER_EXCEPTION;
                return this.Redirect(redirectUrl);
            }
        }

        // AcceptedOfferController
        public ActionResult AcceptedOffer()
        {
            string guid = string.Empty;

            try
            {
                if (!this.Context.IsNormalMode())
                {
                    return this.GetAcceptedOfferEditView();
                }
                
                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired));
                }

                var user = this.AuthenticationService.GetCurrentUser();
                guid = user.Guid;
                var offer = this.ApiService.GetOffer(guid);

                if (offer == null)
                {
                    var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.WrongUrl) + "?code=" + Constants.ErrorCodes.OFFER_NOT_FOUND;
                    return Redirect(url);
                }

                if (!offer.IsAccepted)
                {
                    var url = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.Offer);
                    return Redirect(url);
                }

                var datasource = this.GetLayoutItem<PageAcceptedOfferModel>();
                var definition = this.SettingsReaderService.GetDefinition(offer);
                var settings = this.SettingsReaderService.GetSiteSettings();
                var viewModel = new AcceptedOfferViewModel(settings);
                viewModel.Datasource = datasource;
                viewModel.MainText = Utils.GetReplacedTextTokens(definition.OfferAcceptedMainText.Text, offer.TextParameters);
                viewModel["appUnavailableTitle"] = settings.ApplicationUnavailableTitle;
                viewModel["appUnavailableText"] = settings.ApplicationUnavailableText;
                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying accepted offer.", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError));
            }
        }

        // ExpirationController
        public ActionResult Expiration()
        {
            string guid = string.Empty;

            try
            {
                if (!this.Context.IsNormalMode())
                {
                    return this.GetExpirationEditModel();
                }

                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired));
                }

                var data = this.AuthenticationService.GetCurrentUser();
                guid = data.Guid;
                var offer = this.ApiService.GetOffer(guid);

                var definition = this.SettingsReaderService.GetDefinition(offer);
                var datasource = this.GetLayoutItem<PageExpirationModel>();

                var viewModel = new PageExpirationViewModel();
                viewModel.Datasource = datasource;
                viewModel.MainText = Utils.GetReplacedTextTokens(definition.OfferExpiredMainText.Text, offer.TextParameters);
                return View("/Areas/eContracting2/Views/Expiration.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying expiration page", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError));
            }
        }

        // WelcomeRichTextController
        public ActionResult RichText()
        {
            var dataSource = this.GetLayoutItem<RichTextModel>();
            return View("/Areas/eContracting2/Views/RichText.cshtml", dataSource);
        }

        // ThankYouController
        public ActionResult ThankYou()
        {
            var guid = string.Empty;

            try
            {
                if (!this.Context.IsNormalMode())
                {
                    return this.GetThankYouEditModel();
                }

                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired));
                }

                var user = this.AuthenticationService.GetCurrentUser();
                guid = user.Guid;
                var offer = this.ApiService.GetOffer(guid);

                if (!offer.IsAccepted)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.Offer);
                    this.Logger.Info(guid, $"Offer is not accepted");
                    return this.Redirect(redirectUrl);
                }

                this.ClearUserData(guid);

                var viewModel = this.GetThankYouViewModel(offer);
                return this.View("/Areas/eContracting2/Views/ThankYou.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"Error when displaying thank you page", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError));
            }
        }

        // UserBlockedController
        public ActionResult UserBlocked()
        {
            var guid = string.Empty;

            try
            {
                var datasource = this.GetLayoutItem<PageUserBlockedModel>();
                return View("/Areas/eContracting2/Views/UserBlocked.cshtml", datasource);
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, "Error when displaying user blocked page", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError));
            }
        }

        // eContractingController
        public ActionResult CookieLaw()
        {
            var model = this.SettingsReaderService.GetSiteSettings().CookieLawSettings;

            if (model != null)
            {
                return View("/Areas/eContracting2/Views/CookieLaw.cshtml", model);
            }

            return new EmptyResult();
        }
        
        public ActionResult Disclaimer()
        {
            var datasource = this.GetLayoutItem<PageDisclaimerModel>();

            if (this.Context.IsEditMode())
            {
                return View("/Areas/eContracting2/Views/Edit/Disclaimer.cshtml", datasource);
            }

            return View("/Areas/eContracting2/Views/Disclaimer.cshtml", datasource);
        }

        public ActionResult Error404()
        {
            if (this.Context.IsEditMode())
            {
                return this.GetError404EditView();
            }

            var code = this.Request.QueryString["code"];
            var datasource = this.GetLayoutItem<PageError404Model>();

            if (!this.Context.IsEditMode())
            {
                var error = Constants.GetErrorDescription(code);
                datasource.PageTitle = datasource.PageTitle.Replace("{CODE}", error);
                datasource.MainText = datasource.MainText.Replace("{CODE}", error);
            }

            return View("/Areas/eContracting2/Views/Error404.cshtml", datasource);
        }

        public ActionResult Error500()
        {
            if (this.Context.IsEditMode())
            {
                return this.GetError500EditView();
            }

            var code = this.Request.QueryString["code"];

            var datasource = this.GetLayoutItem<PageError500Model>();

            if (!this.Context.IsEditMode())
            {
                var error = Constants.GetErrorDescription(code);
                datasource.PageTitle = datasource.PageTitle.Replace("{CODE}", error);
                datasource.MainText = datasource.MainText.Replace("{CODE}", error);
            }

            return View("/Areas/eContracting2/Views/Error500.cshtml", datasource);
        }

        public ActionResult SessionExpired()
        {
            var datasouce = this.GetLayoutItem<PageSessionExpiredModel>();

            if (this.Context.IsEditMode())
            {
                return View("/Areas/eContracting2/Views/Edit/SessionExpired.cshtml", datasouce);
            }

            return View("/Areas/eContracting2/Views/SessionExpired.cshtml", datasouce);
        }

        public ActionResult Header()
        {
            var model = this.GetDataSourceItem<PageHeaderModel>();
            return this.View("/Areas/eContracting2/Views/Shared/Header.cshtml", model);
        }

        public ActionResult Footer()
        {
            var model = this.GetDataSourceItem<PageFooterModel>();
            return this.View("/Areas/eContracting2/Views/Shared/Footer.cshtml", model);
        }

        public ActionResult PromoBox()
        {
            var datasource = this.GetDataSourceItem<PromoBoxModel>();

            if (datasource == null)
            {
                return new EmptyResult();
            }

            return this.View("/Areas/eContracting2/Views/PromoBox.cshtml", datasource);
        }

        protected internal OfferViewModel GetOfferViewModel(OfferModel offer, PageNewOfferModel datasource)
        {
            var definition = this.SettingsReaderService.GetDefinition(offer);
            this.Logger.Info(offer.Guid, "Matrix used: " + definition.Path);
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var siteSettings = this.SettingsReaderService.GetSiteSettings();
            var viewModel = new OfferViewModel(siteSettings);
            viewModel.PageTitle = definition.OfferTitle.Text;
            viewModel.MainText = definition.OfferMainText.Text;
            viewModel.Steps = new StepsViewModel(steps);
            viewModel.AllowedContentTypes = siteSettings.AllowedDocumentTypesList;
            viewModel.MaxFileSize = siteSettings.SingleUploadFileSizeLimitKBytes * 1024;
            viewModel.MaxGroupFileSize = siteSettings.GroupResultingFileSizeLimitKBytes * 1024;
            viewModel.MaxAllFilesSize = siteSettings.TotalResultingFilesSizeLimitKBytes * 1024;
            viewModel.ThankYouPage = siteSettings.ThankYou.Url;
            viewModel.SessionExpiredPage = siteSettings.SessionExpired.Url;

            if (offer.HasGDPR)
            {
                var GDPRGuid = Utils.AesEncrypt(offer.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);
                viewModel.GdprGuid = GDPRGuid;
                viewModel.GdprUrl = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
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

            return viewModel;
        }

        protected internal ActionResult GetOfferEditView()
        {
            var datasource = this.GetLayoutItem<PageNewOfferModel>();
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);

            var data = this.Cache.Get<OfferCacheDataModel>(Constants.CacheKeys.OFFER_IDENTIFIER);
            var definition = this.SettingsReaderService.GetDefinition(data.Process, data.ProcessType);

            var siteSettings = this.SettingsReaderService.GetSiteSettings();
            var viewModel = new OfferViewModel(siteSettings);
            viewModel.Definition = definition;
            viewModel.PageTitle = definition.OfferTitle.Text;
            viewModel.MainText = definition.OfferMainText.Text;
            viewModel.Steps = new StepsViewModel(steps);
            viewModel.AllowedContentTypes = siteSettings.AllowedDocumentTypesList;
            viewModel.MaxFileSize = siteSettings.SingleUploadFileSizeLimitKBytes * 1024;
            viewModel.MaxGroupFileSize = siteSettings.GroupResultingFileSizeLimitKBytes * 1024;
            viewModel.MaxAllFilesSize = siteSettings.TotalResultingFilesSizeLimitKBytes * 1024;
            viewModel.ThankYouPage = siteSettings.ThankYou.Url;
            viewModel.SessionExpiredPage = siteSettings.SessionExpired.Url;

            if (this.Context.IsEditMode())
            {
                return this.View("/Areas/eContracting2/Views/Edit/Offer.cshtml", viewModel);
            }

            return this.View("/Areas/eContracting2/Views/Preview/Offer.cshtml", viewModel);
        }

        protected internal ActionResult GetAcceptedOfferEditView()
        {
            var datasource = this.GetLayoutItem<PageAcceptedOfferModel>();
            var definition = this.SettingsReaderService.GetDefinitionDefault();
            var settings = this.SettingsReaderService.GetSiteSettings();
            var viewModel = new AcceptedOfferViewModel(settings);
            viewModel.Datasource = datasource;
            viewModel.MainText = definition.OfferAcceptedMainText.Text;
            viewModel["appUnavailableTitle"] = "No available";
            viewModel["appUnavailableText"] = "Not available in Experience editor";
            return View("/Areas/eContracting2/Views/Edit/AcceptedOffer.cshtml", viewModel);
        }

        protected internal ThankYouViewModel GetThankYouViewModel(OfferModel offer)
        {
            var datasource = this.GetLayoutItem<PageThankYouModel>();
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));
            viewModel.MainText = Utils.GetReplacedTextTokens(definition.MainTextThankYou.Text, offer.TextParameters);
            var scriptParameters = this.GetScriptParameters(offer, datasource, definition);
            viewModel.ScriptParameters["eCat"] = scriptParameters.eCat;
            viewModel.ScriptParameters["eAct"] = scriptParameters.eAct;
            viewModel.ScriptParameters["eLab"] = scriptParameters.eLab;
            return viewModel;
        }

        protected internal ActionResult GetThankYouEditModel()
        {
            var datasource = this.GetLayoutItem<PageThankYouModel>();
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));

            if (this.Context.IsEditMode())
            {
                return this.View("/Areas/eContracting2/Views/Edit/ThankYou.cshtml", viewModel);
            }

            return this.View("/Areas/eContracting2/Views/ThankYou.cshtml", viewModel);
        }

        protected internal ActionResult GetExpirationEditModel()
        {
            var datasource = this.GetLayoutItem<PageExpirationModel>();
            var viewModel = new PageExpirationViewModel();
            viewModel.Datasource = datasource;

            if (this.Context.IsPreviewMode())
            {
                return View("/Areas/eContracting2/Views/Expiration.cshtml", viewModel);
            }

            return View("/Areas/eContracting2/Views/Edit/Expiration.cshtml", viewModel);
        }

        protected internal ActionResult GetError404EditView()
        {
            var datasource = this.GetLayoutItem<PageError404Model>();
            return View("/Areas/eContracting2/Views/Edit/Error404.cshtml", datasource);
        }

        protected internal ActionResult GetError500EditView()
        {
            var datasource = this.GetLayoutItem<PageError500Model>();
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

        protected internal (string eCat, string eAct, string eLab) GetScriptParameters(OfferModel offer, PageThankYouModel datasource, DefinitionCombinationModel definition)
        {
            var eCat = string.Empty;
            var eAct = string.Empty;
            var eLab = string.Empty;

            try
            {
                var type = offer.IsCampaign ? datasource.CampaignLabel : datasource.IndividualLabel;
                var code = offer.IsCampaign ? offer.Campaign : offer.CreatedAt;
                var commodity = string.Empty;

                if (offer.Commodity.StartsWith(Constants.GTMElectricityIdentifier))
                {
                    commodity = datasource.ElectricityLabel;
                }
                else
                {
                    commodity = datasource.GasLabel;
                }

                var tokens = new Dictionary<string, string>();
                tokens.Add("TYPE", type);
                tokens.Add("CAMPAIGN", code);
                tokens.Add("COMMODITY", commodity);

                eCat = Utils.GetReplacedTextTokens(datasource.GoogleAnalytics_eCat, tokens);
                eAct = Utils.GetReplacedTextTokens(definition.Process.GoogleAnalytics_eAct, tokens);
                eLab = Utils.GetReplacedTextTokens(definition.ProcessType.GoogleAnalytics_eLab, tokens);
            }
            catch (Exception ex)
            {
                this.Logger.Error(offer.Guid, $"Cannot create data for Google Tag Manager (eCat, eAct, eLab)", ex);
            }

            return (eCat, eAct, eLab);
        }
    }
}
