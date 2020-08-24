using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Content;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class eContracting2Controller : GlassController
    {
        protected readonly IRweClient Client;
        protected readonly IAuthenticationDataSessionStorage DataSessionStorage;

        public eContracting2Controller()
        {
            this.Client = ServiceLocator.ServiceProvider.GetRequiredService<IRweClient>();
            this.DataSessionStorage = ServiceLocator.ServiceProvider.GetRequiredService<IAuthenticationDataSessionStorage>();
        }

        public eContracting2Controller(IRweClient client, IAuthenticationDataSessionStorage dataSessionStorage)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.DataSessionStorage = dataSessionStorage ?? throw new ArgumentNullException(nameof(client));
        }

        // WelcomeController
        [HttpGet]
        public ActionResult Welcome()
        {
            throw new NotImplementedException();
        }

        // WelcomeController
        [HttpPost]
        public ActionResult Welcome(string guid)
        {
            throw new NotImplementedException();
        }

        // eContractingAuthenticationController
        [HttpGet]
        public ActionResult Login()
        {
            throw new NotImplementedException();
        }

        // eContractingAuthenticationController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AuthenticationModel authenticationModel)
        {
            throw new NotImplementedException();
        }

        // AcceptedOfferController
        [HttpGet]
        public ActionResult AcceptedOffer()
        {
            throw new NotImplementedException();
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
                var datasource = this.GetDataSourceItem<EContractingExpirationTemplate>();

                if (!this.DataSessionStorage.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = this.DataSessionStorage.GetUserData();
                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                var mainText = textHelper.GetMainText(datasource, data);

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;

                return View("/Areas/eContracting/Views/Expiration.cshtml", datasource);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying expiration page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
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
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = this.DataSessionStorage.GetUserData();
                guid = data.Identifier;

                if (data.IsAccepted)
                {
                    Log.Debug($"[{guid}] Offer already accepted", this);
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    return Redirect(redirectUrl);
                }

                if (data.OfferIsExpired)
                {
                    Log.Debug($"[{guid}] Offer expired", this);
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                    return Redirect(redirectUrl);
                }

                this.Client.SignOffer(guid);

                var offer = this.Client.GenerateXml(guid);
                var parameters = SystemHelpers.GetParameters(data.Identifier, data.OfferType, SystemHelpers.GetCodeOfAdditionalInfoDocument(offer));
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText, parameters);
                var datasource = this.GetDataSourceItem<EContractingOfferTemplate>();
                var mainText = textHelper.GetMainText(datasource, data);

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                this.ViewData["MainText"] = mainText;
                this.ViewData["VoucherText"] = textHelper.GetVoucherText(datasource, data);

                if (offer.OfferInternal.HasGDPR)
                {
                    var GDPRGuid = StringUtils.AesEncrypt(offer.OfferInternal.GDPRKey, datasource.AesEncryptKey, datasource.AesEncryptVector);

                    ViewData["GDPRGuid"] = GDPRGuid;
                    ViewData["GDPRUrl"] = datasource.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }


                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.GetSignInFailure(data.OfferType);

                return View("/Areas/eContracting/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        // WelcomeRichTextController
        public ActionResult RichText()
        {
            var dataSource = this.GetDataSourceItem<EContractingWelcomeRichTextDatasource>();
            WelcomeRichTextModel viewModel;

            if (Sitecore.Context.PageMode.IsNormal)
            {
                var processingParameters = this.HttpContext.Items["WelcomeData"] as IDictionary<string, string>;

                if (processingParameters == null)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var replacedText = SystemHelpers.ReplaceParameters(dataSource.Text, processingParameters);

                viewModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = replacedText };
            }
            else
            {
                viewModel = new WelcomeRichTextModel() { Datasource = dataSource, ReplacedText = dataSource.Text };
            }

            return View("/Areas/eContracting/Views/Content/WelcomeRichText.cshtml", viewModel);
        }

        // ThankYouController
        public ActionResult ThankYou()
        {
            var mainText = string.Empty;
            var guid = string.Empty;
            var datasource = this.GetDataSourceItem<EContractingThankYouTemplate>();

            try
            {
                var data = this.DataSessionStorage.GetUserData();

                if (!this.DataSessionStorage.IsDataActive)
                {
                    return this.Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                mainText = textHelper.GetMainText(datasource, data);

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
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

            return this.View("/Areas/eContracting/Views/ThankYou.cshtml", datasource);
        }

        // UserBlockedController
        public ActionResult UserBlocked()
        {
            try
            {
                var datasource = this.GetDataSourceItem<EContractingUserBlockedTemplate>();
                return View("/Areas/eContracting/Views/UserBlocked.cshtml", datasource);
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
                        return View("/Areas/eContracting/Views/CookieLaw.cshtml", cookieLawSettings);
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

        // eContractingController
        [Obsolete]
        public ActionResult DocumentPanel(bool isAccepted)
        {
            throw new NotImplementedException();
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
