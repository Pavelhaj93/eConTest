using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Content;
using eContracting.Kernel.GlassItems.Content.Modal_window;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using eContracting.Models;
using eContracting.Services;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Globalization;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2Controller : GlassController
    {
        protected readonly ILogger Logger;
        protected readonly IUserDataCacheService Cache;
        protected readonly IApiService ApiService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IAuthenticationService AuthenticationService;
        protected readonly IContextWrapper Context;
        protected readonly IUserFileCacheService UserFileCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2Controller"/> class.
        /// </summary>
        public eContracting2Controller()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.AuthenticationService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
            this.Context = ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>();
            this.UserFileCache = ServiceLocator.ServiceProvider.GetRequiredService<IUserFileCacheService>();
        }

        public eContracting2Controller(
            ILogger logger,
            IUserDataCacheService cache,
            IApiService apiService,
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

                var datasource = this.GetLayoutItem<OfferPageModel>();
                var definition = this.SettingsReaderService.GetDefinition(offer);

                var viewModel = new OfferViewModel(definition, this.SettingsReaderService);
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
            var datasource = this.GetLayoutItem<OfferPageModel>();
            var definition = this.SettingsReaderService.GetDefinition(fakeOffer);
            var viewModel = new OfferViewModel(definition, this.SettingsReaderService);
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

                var datasource = this.GetLayoutItem<AcceptedOfferPageModel>();
                var settings = this.SettingsReaderService.GetSiteSettings();
                var viewModel = new AcceptedOfferViewModel(datasource, settings.ApplicationUnavailableTitle, settings.ApplicationUnavailableText);
                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{guid}] Error when displaying accepted offer.", ex);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        // eContractingController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept()
        {
            return new EmptyResult();
        }

        // ExpirationController
        public ActionResult Expiration()
        {
            string guid = string.Empty;

            try
            {
                var datasource = this.GetLayoutItem<EContractingExpirationTemplate>();

                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = this.AuthenticationService.GetCurrentUser();
                guid = data.Guid;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                string mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;

                return View("/Areas/eContracting2/Views/Expiration.cshtml", datasource);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying expiration page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
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
                    return this.Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
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
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        // UserBlockedController
        public ActionResult UserBlocked()
        {
            try
            {
                var datasource = this.GetLayoutItem<EContractingUserBlockedTemplate>();
                return View("/Areas/eContracting2/Views/UserBlocked.cshtml", datasource);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying user blocked page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        // eContractingController
        public ActionResult CookieLaw()
        {
            try
            {
                using (var sitecoreContext = new SitecoreContext())
                {
                    var cookieLawSettings = sitecoreContext.GetItem<CookieLawSettings>(ItemPaths.CookieLawSettings);
                    if (cookieLawSettings != null)
                    {
                        return View("/Areas/eContracting2/Views/CookieLaw.cshtml", cookieLawSettings);
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying cookie law", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
        
        public ActionResult Disclaimer()
        {
            var datasource = this.GetLayoutItem<EContractingDisclaimerTemplate>();
            return View("/Areas/eContracting2/Views/Disclaimer.cshtml", datasource);
        }

        public ActionResult Error404()
        {
            var code = this.Request.QueryString["code"];

            var datasource = this.GetLayoutItem<Error404PageModel>();

            if (!this.Context.IsEditMode())
            {
                var error = Constants.GetErrorDescription(code);
                datasource.PageTitle = datasource.PageTitle.Replace("{CODE}", error);
                datasource.Text = datasource.Text.Replace("{CODE}", error);
            }

            return View("/Areas/eContracting2/Views/Error404.cshtml", datasource);
        }

        public ActionResult SessionExpired()
        {
            //this.Server
            var viewModel = new SessionExpiredViewModel();

            var datasouce = this.GetLayoutItem<EContractingSessionExpiredTemplate>();
            return View("/Areas/eContracting2/Views/SessionExpired.cshtml", datasouce);
        }

        private string[] GetScriptParameters(OfferModel offer)
        {
            var eCat = string.Empty;
            var eAct = string.Empty;
            var eLab = string.Empty;

            try
            {
                var settings = this.SitecoreContext.GetItem<ThankYouPageSettings>(ItemPaths.ThankYouPageSettings);
                var commodity = CommodityHelper.CommodityTypeByExtUi(offer.Commodity) == CommodityTypes.Electricity ? settings.ElectricityLabel : settings.GasLabel;
                var type = offer.IsCampaign ? settings.CampaignLabel : settings.IndividualLabel;
                var code = offer.IsCampaign ? offer.Campaign : offer.CreatedAt;
                eCat = string.Format(settings.CatText, type);
                eAct = string.Format(settings.ActText, type, commodity);
                eLab = string.Format(settings.LabText, eAct, code);
            }
            catch (Exception ex)
            {
                this.Logger.Error($"[{offer.Guid}] Can not process Google script parameters", ex);
            }

            return new string[] { eCat, eAct, eLab };
        }

        private string[] GetScriptParameters(AuthenticationDataItem data)
        {
            var eCat = string.Empty;
            var eAct = string.Empty;
            var eLab = string.Empty;

            try
            {
                var settings = this.SitecoreContext.GetItem<ThankYouPageSettings>(ItemPaths.ThankYouPageSettings);
                var commodity = CommodityHelper.CommodityTypeByExtUi(data.Commodity) == CommodityTypes.Electricity ? settings.ElectricityLabel : settings.GasLabel;
                var type = data.IsIndi ? settings.IndividualLabel : settings.CampaignLabel;
                var code = data.IsIndi ? data.CreatedAt : data.Campaign;
                eCat = string.Format(settings.CatText, type);
                eAct = string.Format(settings.ActText, type, commodity);
                eLab = string.Format(settings.LabText, eAct, code);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"[{data.Identifier}] Can not process Google script parameters", ex, this);
            }

            return new string[] { eCat, eAct, eLab };
        }

        protected internal ThankYouViewModel GetThankYouViewModel(OfferModel offer)
        {
            var datasource = this.GetLayoutItem<ThankYouPageModel>();
            var definition = this.SettingsReaderService.GetDefinition(offer);
            var steps = this.SettingsReaderService.GetSteps(datasource.Step);

            var viewModel = new ThankYouViewModel(datasource, new StepsViewModel(steps));
            viewModel.MainText = Utils.GetReplacedTextTokens(definition.MainTextThankYou.Text, offer.TextParameters);
            var scriptParameters = this.GetScriptParameters(offer);

            if (scriptParameters == null || scriptParameters.Length != 3 || scriptParameters.Any(a => string.IsNullOrEmpty(a)))
            {
                throw new Exception("Can not get script parameters.");
            }

            viewModel.ScriptParameters["eCat"] = scriptParameters[0];
            viewModel.ScriptParameters["eAct"] = scriptParameters[1];
            viewModel.ScriptParameters["eLab"] = scriptParameters[2];

            return viewModel;
        }

        protected internal void FillLabels(OfferViewModel viewModel, OfferPageModel datasource, DefinitionCombinationModel definition)
        {
            var settings = this.SettingsReaderService.GetSiteSettings();
            viewModel["appUnavailableTitle"] = settings.ApplicationUnavailableTitle;
            viewModel["appUnavailableText"] = settings.ApplicationUnavailableText;
            viewModel["acceptAll"] = Translate.Text("Označit vše");
            viewModel["acceptOfferTitle"] = definition.OfferAcceptTitle.Text;
            viewModel["acceptOfferHelptext"] = definition.OfferAcceptText.Text;
            viewModel["submitBtn"] = "Akceptuji"; //TODO:
            viewModel["signatureBtn"] = "Podepsat"; //TODO:
            viewModel["signatureEditBtn"] = "signatureEditBtn"; //TODO:
            viewModel["signatureModalTitle"] = "signatureModalTitle"; //TODO:
            viewModel["signatureModalText"] = "Podepište se prosím níže:"; //TODO:
            viewModel["signatureModalConfirm"] = "Potvrdit"; //TODO:
            viewModel["signatureModalClear"] = "Smazat"; //TODO:
            viewModel["signatureModalError"] = "Dokument se nepodařilo podepsat, zkuste to prosím znovu"; //TODO:
            viewModel["selectFile"] = "Vyberte dokument"; //TODO:
            viewModel["selectFileHelpText"] = "Sem přetáhněte jeden i více souborů nebo"; //TODO:
            viewModel["removeFile"] = "Odebrat dokument"; //TODO:
            viewModel["selectedFiles"] = "Vybrané dokumenty"; //TODO:
            viewModel["rejectedFiles"] = "Nevyhovující dokumenty"; //TODO:
            viewModel["uploadFile"] = "Nahrát dokument"; //TODO:
            viewModel["captureFile"] = "Vyfotit a nahrát"; //TODO:
            viewModel["invalidFileTypeError"] = "Dokument má špatný formát"; //TODO:
            viewModel["fileExceedSizeError"] = "Dokument je příliš velký"; //TODO:
            viewModel["acceptanceModalTitle"] = datasource.ConfirmModalWindowTitle;
            viewModel["acceptanceModalText"] = datasource.ConfirmModalWindowText;
            viewModel["acceptanceModalAccept"] = datasource.ConfirmModalWindowButtonAcceptLabel;
            viewModel["acceptanceModalCancel"] = datasource.ConfirmModalWindowButtonCancelLabel;
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
