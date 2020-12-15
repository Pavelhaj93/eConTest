using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Logging;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class LayoutViewModel : RenderingModel
    {
        public string PageMetaTitle { get; protected set; }

        public string ImageLinkUrl { get; set; }

        public string PhoneLinkUrl { get; set; }

        public string PhoneLinkUrlText { get; set; }

        public string CopyrightText { get; set; }

        public string EmailLink { get; set; }

        public string EmailText { get; set; }

        public string PhoneLinkLowerUrl { get; set; }

        public string PhoneLinkLowerUrlText { get; set; }

        public string DisclaimerText { get; set; }

        public string DisclaimerLink { get; set; }

        public string ErrorCodeDescription { get; set; }

        public override void Initialize(Rendering rendering)
        {
            try
            {
                if (rendering.Item.TemplateID == Constants.TemplateIds.PageHome)
                {
                    var settings = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
                    var url = settings.GetSiteSettings().WrongUrl.Url + "?code=" + Constants.ErrorCodes.HOMEPAGE;
                    HttpContext.Current.Response.Redirect(url, true);
                    return;
                }

                base.Initialize(rendering);

                //this.ProcessErrorDescription(rendering);
                //this.PageMetaTitle = Sitecore.Context.Item["PageTitle"] + (this.ErrorCodeDescription != null ? " - " + this.ErrorCodeDescription : "");
                this.PageMetaTitle = Sitecore.Context.Item["PageTitle"];

                if (!Sitecore.Context.PageMode.IsNormal)
                {
                    var processCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS];
                    var processTypeCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS_TYPE];
                    var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
                    var settings = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();

                    if (string.IsNullOrEmpty(processCode))
                    {
                        processCode = settings.GetAllProcesses().First().Code;
                    }

                    if (string.IsNullOrEmpty(processTypeCode))
                    {
                        processTypeCode = settings.GetAllProcessTypes().First().Code;
                    }

                    var data = new OfferIdentifier(processCode, processTypeCode);
                    cache.Set(Constants.CacheKeys.OFFER_IDENTIFIER, data);
                }
                else
                {
                    if (rendering.Item.TemplateID == Constants.TemplateIds.PageLogin)
                    {
                        var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];

                        if (!string.IsNullOrEmpty(guid))
                        {
                            var api = ServiceLocator.ServiceProvider.GetRequiredService<IApiService>();
                            var offer = api.GetOffer(guid);

                            if (offer != null)
                            {
                                var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
                                var data = new OfferIdentifier(offer);
                                cache.Set(Constants.CacheKeys.OFFER_IDENTIFIER, data);
                            }
                        }
                    }
                }

                using (var sitecoreContext = new SitecoreContext())
                {
                    var headerSettings = sitecoreContext.GetItem<HeaderSettings>(ItemPaths.HeaderSettings);
                    var footerSettings = sitecoreContext.GetItem<FooterSettings>(ItemPaths.FooterSettings);

                    if (headerSettings != null)
                    {
                        if (headerSettings.ImageLink != null)
                        {
                            ImageLinkUrl = headerSettings.ImageLink.Url;
                        }

                        PhoneLinkUrl = headerSettings.PhoneNumberLink;
                        PhoneLinkUrlText = headerSettings.PhoneNumber;
                    }

                    if (footerSettings != null)
                    {
                        CopyrightText = footerSettings.CopyrightText;

                        EmailLink = footerSettings.EmailLink;
                        EmailText = footerSettings.EmailText;
                        PhoneLinkLowerUrl = footerSettings.PhoneNumberLinkFooter;
                        PhoneLinkLowerUrlText = footerSettings.PhoneNumberFooter;

                        if (footerSettings.DisclaimerLink != null)
                        {
                            DisclaimerText = footerSettings.DisclaimerLink.Text;
                            DisclaimerLink = footerSettings.DisclaimerLink.Url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = ServiceLocator.ServiceProvider.GetService<ILogger>();
                logger?.Error("Preparing layout model failed", ex);
            }
        }

        //protected void ProcessErrorDescription(Rendering rendering)
        //{
        //    if (HttpContext.Current.Request.QueryString.AllKeys.Contains("code"))
        //    {
        //        var key = HttpContext.Current.Request.QueryString["code"];

        //        if (Constants.ErrorCodes.Descriptions.ContainsKey(key))
        //        {
        //            this.ErrorCodeDescription = Constants.ErrorCodes.Descriptions[key];
        //        }
        //    }
        //}
    }
}
