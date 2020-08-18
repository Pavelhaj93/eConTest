using System;
using System.Linq;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Thank you page.
    /// </summary>
    public class ThankYouController : BaseController<EContractingThankYouTemplate>
    {
        /// <summary>
        /// Thank you page.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult ThankYou()
        {
            var mainText = string.Empty;
            var guid = string.Empty;

            try
            {
                var ads = new AuthenticationDataSessionStorage();
                var data = ads.GetUserData();

                if (!ads.IsDataActive)
                {
                    return this.Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                mainText = textHelper.GetMainText(this.Context, data);

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
                mainText = this.Context.ServiceUnavailableText;
                Sitecore.Diagnostics.Log.Error($"[{guid}] Error when displaying thank you page", ex, this);
            }

            this.ViewData["MainText"] = mainText;

            return this.View("/Areas/eContracting/Views/ThankYou.cshtml", this.Context);
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
