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
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContracting2Controller : GlassController
    {
        protected readonly ILogger Logger;
        protected readonly ICache Cache;
        protected readonly IApiService ApiService;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IAuthenticationService AuthenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="eContracting2Controller"/> class.
        /// </summary>
        public eContracting2Controller()
        {
            this.Logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger>();
            this.Cache = ServiceLocator.ServiceProvider.GetRequiredService<ICache>();
            this.ApiService = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.AuthenticationService = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationService>();
        }

        public eContracting2Controller(
            ILogger logger,
            ICache cache,
            IApiService apiService,
            ISettingsReaderService settingsReaderService,
            IAuthenticationService authService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthenticationService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
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

                var datasource = this.GetLayoutItem<OfferPageModel>();
                var definition = this.SettingsReaderService.GetDefinition(offer);

                var viewModel = new OfferViewModel();
                viewModel.Datasource = datasource;
                viewModel.MainText = definition.OfferMainText.Text;

                var parameters = new Dictionary<string, string>(); //TODO: SystemHelpers.GetParameters(this.Client, data.Identifier, data.OfferType, SystemHelpers.GetCodeOfAdditionalInfoDocument(offer), this.SettingsReaderService.GetGeneralSettings());
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText, parameters);
                string mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                //if (mainText == null)
                //{
                //    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                //    return Redirect(redirectUrl);
                //}

                //this.ViewData["MainText"] = mainText;
                //this.ViewData["VoucherText"] = string.Empty; //TODO: textHelper.GetVoucherText(datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (offer.HasGDPR)
                {
                    var GDPRGuid = Utils.AesEncrypt(offer.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);
                    viewModel.GdprGuid = GDPRGuid;
                    viewModel.GdprUrl = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }

                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = string.Empty; //TODO: generalSettings.GetSignInFailure(data.OfferType);

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
            var datasource = this.GetLayoutItem<OfferPageModel>();
            var data = this.Cache.GetFromRequest<OfferIdentifier>(Constants.CacheKeys.OFFER_IDENTIFIER);

            if (data != null)
            {
                var definition = this.SettingsReaderService.GetDefinition(data.Process, data.ProcessType);
                var viewModel = new OfferViewModel();
                viewModel.Datasource = datasource;
                viewModel.Definition = definition;
                return this.View("/Areas/eContracting2/Views/Preview/Offer.cshtml", viewModel);
            }

            return new EmptyResult(); //TODO: We should return something more describable
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
                
                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml");
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
            var mainText = string.Empty;
            var guid = string.Empty;
            var datasource = this.GetLayoutItem<EContractingThankYouTemplate>();

            try
            {
                if (!this.AuthenticationService.IsLoggedIn())
                {
                    return this.Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                guid = this.AuthenticationService.GetCurrentUser().Guid;
                var offer = this.ApiService.GetOffer(guid);
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return this.Redirect(redirectUrl);
                }

                this.Session.Remove("UserFiles");
                var scriptParameters = this.GetScriptParameters(offer);

                if (scriptParameters == null || scriptParameters.Length != 3 || scriptParameters.Any(a => string.IsNullOrEmpty(a)))
                {
                    throw new Exception("Can not get script parameters.");
                }

                this.ViewData["eCat"] = scriptParameters[0];
                this.ViewData["eAct"] = scriptParameters[1];
                this.ViewData["eLab"] = scriptParameters[2];
            }
            catch (Exception ex)
            {
                mainText = datasource.ServiceUnavailableText;
                Sitecore.Diagnostics.Log.Error($"[{guid}] Error when displaying thank you page", ex, this);
            }

            this.ViewData["MainText"] = mainText;

            return this.View("/Areas/eContracting2/Views/ThankYou.cshtml", datasource);
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
            var datasource = this.GetLayoutItem<EContracting404Template>();
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
    }
}
