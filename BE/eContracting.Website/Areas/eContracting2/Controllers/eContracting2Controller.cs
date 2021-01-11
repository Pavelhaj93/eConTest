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
using Sitecore.Globalization;

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
        }

        [ExcludeFromCodeCoverage]
        public eContracting2Controller(
            ILogger logger,
            IUserDataCacheService cache,
            IOfferService apiService,
            ISettingsReaderService settingsReaderService,
            IAuthenticationService authService,
            IContextWrapper context,
            IUserFileCacheService userFileCache)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthenticationService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.UserFileCache = userFileCache ?? throw new ArgumentNullException(nameof(userFileCache));
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
                if (!Sitecore.Context.PageMode.IsNormal)
                {
                    return this.OfferPreview();
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

                if (offer.OfferIsExpired)
                {
                    this.Logger.Debug(guid, $"Offer expired");
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.OfferExpired);
                    return Redirect(redirectUrl);
                }

                if (!this.ApiService.SignInOffer(guid))
                {
                    this.Logger.Debug(guid, $"Offer was not signed in ({Constants.ErrorCodes.OFFER_NOT_SIGNED})");
                    var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.OFFER_NOT_SIGNED;
                    return Redirect(redirectUrl);
                }

                try
                {
                    this.UserFileCache.Clear(new DbSearchParameters(null, guid, null));
                }
                catch (Exception ex)
                {
                    this.Logger.Error(guid, "Cannot clear user file cache", ex);
                }

                var datasource = this.GetLayoutItem<PageNewOfferModel>();
                var definition = this.SettingsReaderService.GetDefinition(offer);
                var steps = this.SettingsReaderService.GetSteps(datasource.Step);
                var viewModel = new OfferViewModel(definition, steps, this.SettingsReaderService);
                viewModel.GdprGuid = offer.GDPRKey;
                viewModel.GdprUrl = datasource.GDPRUrl;
                this.FillLabels(viewModel, datasource, definition);

                if (offer.HasGDPR)
                {
                    var GDPRGuid = Utils.AesEncrypt(offer.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);
                    viewModel.GdprGuid = GDPRGuid;
                    viewModel.GdprUrl = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }

                return this.View("/Areas/eContracting2/Views/Offer.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Error when displaying offer.", ex);
                var redirectUrl = this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError) + "?code=" + Constants.ErrorCodes.OFFER_EXCEPTION;
                return this.Redirect(redirectUrl);
            }
        }

        public ActionResult OfferPreview()
        {
            var fakeHeader = new OfferHeaderModel("XX", Guid.NewGuid().ToString("N"), "3", "");
            var fateXml = new OfferXmlModel() { Content = new OfferContentXmlModel() };
            var fakeAttr = new OfferAttributeModel[] { };
            var fakeOffer = new OfferModel(fateXml, 1, fakeHeader, true, fakeAttr);
            var datasource = this.GetLayoutItem<PageNewOfferModel>();
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var definition = this.SettingsReaderService.GetDefinition(fakeOffer);
            var viewModel = new OfferViewModel(definition, steps, this.SettingsReaderService);
            return this.View("/Areas/eContracting2/Views/Preview/Offer.cshtml", viewModel);
        }

        // AcceptedOfferController
        public ActionResult AcceptedOffer()
        {
            string guid = string.Empty;

            try
            {
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
                var settings = this.SettingsReaderService.GetSiteSettings();
                var viewModel = new AcceptedOfferViewModel(datasource, settings.ApplicationUnavailableTitle, settings.ApplicationUnavailableText);
                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Error when displaying accepted offer.", ex);
                return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SystemError));
            }
        }

        // ExpirationController
        public ActionResult Expiration()
        {
            string guid = string.Empty;

            try
            {
                var datasource = this.GetLayoutItem<PageExpirationModel>();

                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired));
                }

                var data = this.AuthenticationService.GetCurrentUser();
                guid = data.Guid;

                var viewModel = new PageExpirationViewModel();
                viewModel.Datasource = datasource;
                viewModel.MainText = Utils.GetReplacedTextTokens(datasource.MainText, data.TextParameters);
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
                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return this.Redirect(this.SettingsReaderService.GetPageLink(PAGE_LINK_TYPES.SessionExpired));
                }

                var user = this.AuthenticationService.GetCurrentUser();
                guid = user.Guid;

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
            return View("/Areas/eContracting2/Views/Disclaimer.cshtml", datasource);
        }

        public ActionResult Error404()
        {
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

        public ActionResult SessionExpired()
        {
            var datasouce = this.GetLayoutItem<PageSessionExpiredModel>();
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

        private (string eCat, string eAct, string eLab) GetScriptParameters(OfferModel offer, PageThankYouModel datasource)
        {
            const string electricityIdentifier = "8591824";
            var eCat = string.Empty;
            var eAct = string.Empty;
            var eLab = string.Empty;

            try
            {
                var type = offer.IsCampaign ? datasource.CampaignLabel : datasource.IndividualLabel;
                var code = offer.IsCampaign ? offer.Campaign : offer.CreatedAt;
                var commodity = string.Empty;

                if (string.IsNullOrEmpty(offer.Commodity))
                {
                    commodity = "NotDefined";
                }
                else if (offer.Commodity.StartsWith(electricityIdentifier))
                {
                    commodity = "Electricity";
                }
                else
                {
                    commodity = "Gas";
                }

                eCat = string.Format(datasource.CatText, type);
                eAct = string.Format(datasource.ActText, type, commodity);
                eLab = string.Format(datasource.LabText, eAct, code);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{offer.Guid}] Can not process Google script parameters", ex);
            }

            return (eCat, eAct, eLab);
        }

        protected internal ThankYouViewModel GetThankYouViewModel(OfferModel offer)
        {
            var datasource = this.GetLayoutItem<PageThankYouModel>();
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);
            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));
            viewModel.MainText = Utils.GetReplacedTextTokens(definition.MainTextThankYou.Text, offer.TextParameters);
            var scriptParameters = this.GetScriptParameters(offer, datasource);
            viewModel.ScriptParameters["eCat"] = scriptParameters.eCat;
            viewModel.ScriptParameters["eAct"] = scriptParameters.eAct;
            viewModel.ScriptParameters["eLab"] = scriptParameters.eLab;
            return viewModel;
        }

        protected internal void FillLabels(OfferViewModel viewModel, PageNewOfferModel datasource, DefinitionCombinationModel definition)
        {
            var settings = this.SettingsReaderService.GetSiteSettings();
            viewModel["appUnavailableTitle"] = settings.ApplicationUnavailableTitle;
            viewModel["appUnavailableText"] = settings.ApplicationUnavailableText;
            viewModel["acceptAll"] = Translate.Text("MARK_ALL");
            viewModel["acceptOfferTitle"] = definition.OfferAcceptTitle.Text;
            viewModel["acceptOfferHelptext"] = definition.OfferAcceptText.Text;
            viewModel["submitBtn"] = Translate.Text("ACCEPTING");
            viewModel["signatureBtn"] = Translate.Text("SIGN");
            viewModel["signatureEditBtn"] = Translate.Text("MODIFY_SIGNATURE");
            viewModel["signatureModalTitle"] = datasource.SignModalWindowTitle;
            viewModel["signatureModalText"] = datasource.SignModalWindowText;
            viewModel["signatureModalConfirm"] = datasource.SignModalWindowConfirmButtonLabel;
            viewModel["signatureModalClear"] = datasource.SignModalWindowClearButtonLabel;
            viewModel["signatureModalError"] = datasource.SignModalWindowGeneralErrorMessage;
            viewModel["signatureModalThumbnailAlt"] = datasource.SignModalWindowThumbnailText;
            viewModel["signaturePadAlt"] = datasource.SignModalWindowPenArea;
            viewModel["selectFile"] = Translate.Text("SELECT_DOCUMENT");
            viewModel["selectFileHelpText"] = Translate.Text("DRAG_&_DROP") + " " + Translate.Text("OR");
            viewModel["removeFile"] = Translate.Text("REMOVE_DOCUMENT");
            viewModel["fileSize"] = Translate.Text("DOCUMENT_SIZE");
            viewModel["selectedFiles"] = Translate.Text("SELECTED_DOCUMENTS");
            viewModel["rejectedFiles"] = Translate.Text("WRONG_DOCUMENTS");
            viewModel["uploadFile"] = Translate.Text("UPLOAD_DOCUMENT");
            viewModel["captureFile"] = Translate.Text("PHOTO_&_UPLOAD");
            viewModel["invalidFileTypeError"] = Translate.Text("INVALID_DOCUMENT_FORMAT");
            viewModel["fileExceedSizeError"] = Translate.Text("DOCUMENT_TOO_BIG");
            viewModel["acceptanceModalTitle"] = datasource.ConfirmModalWindowTitle;
            viewModel["acceptanceModalText"] = datasource.ConfirmModalWindowText;
            viewModel["acceptanceModalAccept"] = datasource.ConfirmModalWindowButtonAcceptLabel;
            viewModel["acceptanceModalCancel"] = datasource.ConfirmModalWindowButtonCancelLabel;
            viewModel["acceptanceModalError"] = datasource.ConfirmModalWindowGeneralErrorMessage;
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
    }
}
