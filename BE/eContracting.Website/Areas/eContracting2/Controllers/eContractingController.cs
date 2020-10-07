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
using eContracting.Services;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public class eContractingController : GlassController
    {
        protected readonly RweClient Client;
        protected readonly ISettingsReaderService SettingsReaderService;
        protected readonly IAuthenticationDataSessionStorage DataSessionStorage;

        public eContractingController()
        {
            this.Client = ServiceLocator.ServiceProvider.GetRequiredService<RweClient>();
            this.SettingsReaderService = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.DataSessionStorage = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationDataSessionStorage>();
        }

        public eContractingController(RweClient client, ISettingsReaderService settingsReaderService, IAuthenticationDataSessionStorage dataSessionStorage)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
            this.DataSessionStorage = dataSessionStorage ?? throw new ArgumentNullException(nameof(client));
        }

        // AcceptedOfferController
        public ActionResult AcceptedOffer()
        {
            string guid = string.Empty;

            try
            {
                if (!this.DataSessionStorage.IsDataActive)
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SessionExpired).Url);
                }
                var datasource = this.GetLayoutItem<EContractingAcceptedOfferTemplate>();
                var data = this.DataSessionStorage.GetUserData();
                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                string mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;

                var generalSettings = this.SettingsReaderService.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.SignFailure;

                return View("/Areas/eContracting2/Views/AcceptedOffer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying accepted offer.", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
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

                if (!this.DataSessionStorage.IsDataActive)
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SessionExpired).Url);
                }

                var data = this.DataSessionStorage.GetUserData();
                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                string mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;

                return View("/Areas/eContracting2/Views/Expiration.cshtml", datasource);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying expiration page", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
            }
        }

        // OfferController
        public ActionResult Offer()
        {
            string guid = string.Empty;

            try
            {
                if (!this.DataSessionStorage.IsDataActive)
                {
                    Log.Debug($"[{guid}] Session expired", this);
                    return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SessionExpired).Url);
                }

                var data = this.DataSessionStorage.GetUserData();
                guid = data.Identifier;

                if (data.IsAccepted)
                {
                    Log.Debug($"[{guid}] Offer already accepted", this);
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.AcceptedOffer).Url;
                    return Redirect(redirectUrl);
                }

                if (data.OfferIsExpired)
                {
                    Log.Debug($"[{guid}] Offer expired", this);
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.OfferExpired).Url;
                    return Redirect(redirectUrl);
                }

                this.Client.SignOffer(guid);

                var offer = this.Client.GenerateXml(guid);
                var parameters = new Dictionary<string, string>(); //TODO: SystemHelpers.GetParameters(this.Client, data.Identifier, data.OfferType, SystemHelpers.GetCodeOfAdditionalInfoDocument(offer), this.SettingsReaderService.GetGeneralSettings());
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText, parameters);
                var datasource = this.GetLayoutItem<EContractingOfferTemplate>();
                string mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                this.ViewData["MainText"] = mainText;
                this.ViewData["VoucherText"] = string.Empty; //TODO: textHelper.GetVoucherText(datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (offer.OfferInternal.HasGDPR)
                {
                    var GDPRGuid = StringUtils.AesEncrypt(offer.OfferInternal.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);

                    ViewData["GDPRGuid"] = GDPRGuid;
                    ViewData["GDPRUrl"] = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }


                var generalSettings = this.SettingsReaderService.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.GetSignInFailure(data.OfferType);

                return View("/Areas/eContracting2/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying offer.", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
            }
        }

        // WelcomeRichTextController
        public ActionResult RichText()
        {
            var dataSource = this.GetLayoutItem<EContractingWelcomeRichTextDatasource>();
            WelcomeRichTextModel viewModel;

            if (Sitecore.Context.PageMode.IsNormal)
            {
                var processingParameters = this.HttpContext.Items["WelcomeData"] as IDictionary<string, string>;

                if (processingParameters == null)
                {
                    return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url);
                }

                var replacedText = SystemHelpers.ReplaceParameters(dataSource.Text, processingParameters);

                viewModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = replacedText };
            }
            else
            {
                viewModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = dataSource.Text };
            }

            return View("/Areas/eContracting2/Views/Content/WelcomeRichText.cshtml", viewModel);
        }

        // ThankYouController
        public ActionResult ThankYou()
        {
            var mainText = string.Empty;
            var guid = string.Empty;
            var datasource = this.GetLayoutItem<EContractingThankYouTemplate>();

            try
            {
                var data = this.DataSessionStorage.GetUserData();

                if (!this.DataSessionStorage.IsDataActive)
                {
                    return this.Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SessionExpired).Url);
                }

                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                mainText = null; //TODO: textHelper.GetMainText(this.Client, datasource, data, this.SettingsReaderService.GetGeneralSettings());

                if (mainText == null)
                {
                    var redirectUrl = this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.WrongUrl).Url;
                    return this.Redirect(redirectUrl);
                }

                this.Session.Remove("UserFiles");
                var scriptParameters = this.GetScriptParameters(data);

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
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
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
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
            }
        }

        // eContractingController
        [Obsolete]
        public ActionResult DocumentPanel(bool isAccepted)
        {
            //// {CE5332E3-21B0-419D-BE64-FAD155123E42}
            try
            {
                using (var sitecoreContext = new SitecoreContext())
                {
                    MW01DataSource model = new MW01DataSource();
                    model.Item = Sitecore.Context.Database.GetItem(ItemPaths.ModalWindowSettings);

                    var authenticationDataItem = this.DataSessionStorage.GetUserData();

                    model.ClientId = authenticationDataItem.Identifier;
                    model.IsAccepted = isAccepted;
                    model.IsRetention = authenticationDataItem.OfferType == Kernel.OfferTypes.Retention;
                    model.IsAcquisition = authenticationDataItem.OfferType == Kernel.OfferTypes.Acquisition;

                    var generalSettings = this.SettingsReaderService.GetGeneralSettings();
                    ViewData["SelectAll_Text"] = Sitecore.Context.Item["SelectAll_Text"];
                    ViewData["IAmInformed"] = generalSettings.IAmInformed;
                    ViewData["IAgree"] = generalSettings.IAgree;
                    ViewData["Accept"] = generalSettings.Accept;

                    Kernel.GlassItems.Settings.GeneralTextsSettings texts = null;

                    if (model.IsRetention)
                    {
                        texts = generalSettings.GetTexts(Kernel.OfferTypes.Retention);
                    }
                    else if (model.IsAcquisition)
                    {
                        texts = generalSettings.GetTexts(Kernel.OfferTypes.Acquisition);
                    }
                    else
                    {
                        texts = generalSettings.GetTexts(Kernel.OfferTypes.Default);
                    }

                    if (texts != null)
                    {
                        ViewData["DocumentToSign"] = texts.DocumentToSign;
                        ViewData["DocumentToSignAccepted"] = texts.DocumentToSignAccepted;
                        ViewData["Step1Heading"] = texts.Step1Heading;
                        ViewData["Step2Heading"] = texts.Step2Heading;
                        ViewData["WhySignIsRequired"] = texts.WhySignIsRequired;
                        ViewData["SignButton"] = texts.SignButton;
                        ViewData["HowToSign"] = texts.HowToSign;
                        ViewData["HowToAccept"] = texts.HowToAccept;
                        ViewData["SignDocument"] = texts.SignDocument;
                        ViewData["SignRequest"] = texts.SignRequest;
                        ViewData["SignConfirm"] = texts.SignConfirm;
                        ViewData["SignDelete"] = texts.SignDelete;
                    }

                    return View("/Areas/eContracting2/Views/DocumentPanel.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying document panel.", ex, this);
                return Redirect(this.SettingsReaderService.GetPageLink(Kernel.Helpers.PageLinkType.SystemError).Url);
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
            var datasouce = this.GetLayoutItem<EContractingSessionExpiredTemplate>();
            return View("/Areas/eContracting2/Views/SessionExpired.cshtml", datasouce);
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
