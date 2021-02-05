﻿using System;
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

        public string ErrorCodeDescription { get; set; }

        public string GoogleTagManagerKey { get; set; }

        /// <summary>
        /// Gets true when <see cref="GoogleTagManagerKey"/> is not empty and Sitecore is not in editing mode.
        /// </summary>
        public bool GoogleTagManagerShow
        {
            get
            {
                return !string.IsNullOrEmpty(this.GoogleTagManagerKey) && !Sitecore.Context.PageMode.IsExperienceEditorEditing;
            }
        }

        public override void Initialize(Rendering rendering)
        {
            try
            {
                var settingsReader = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
                var settings = settingsReader.GetSiteSettings();

                if (rendering.Item.TemplateID == Constants.TemplateIds.PageHome)
                {
                    var url = settings.WrongUrl.Url + "?code=" + Constants.ErrorCodes.HOMEPAGE;
                    HttpContext.Current.Response.Redirect(url, true);
                    return;
                }

                base.Initialize(rendering);

                this.GoogleTagManagerKey = settings.GoogleTagManager;

                //this.ProcessErrorDescription(rendering);
                //this.PageMetaTitle = Sitecore.Context.Item["PageTitle"] + (this.ErrorCodeDescription != null ? " - " + this.ErrorCodeDescription : "");
                this.PageMetaTitle = Sitecore.Context.Item["PageTitle"];

                if (!Sitecore.Context.PageMode.IsNormal)
                {
                    var processCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS];
                    var processTypeCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS_TYPE];
                    var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();

                    if (string.IsNullOrEmpty(processCode))
                    {
                        processCode = settingsReader.GetAllProcesses().First().Code;
                    }

                    if (string.IsNullOrEmpty(processTypeCode))
                    {
                        processTypeCode = settingsReader.GetAllProcessTypes().First().Code;
                    }

                    var data = new OfferCacheDataModel(processCode, processTypeCode);
                    cache.Set(Constants.CacheKeys.OFFER_IDENTIFIER, data);
                }
                else
                {
                    if (rendering.Item.TemplateID == Constants.TemplateIds.PageLogin)
                    {
                        var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];

                        if (!string.IsNullOrEmpty(guid))
                        {
                            var api = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
                            var offer = api.GetOffer(guid);

                            if (offer != null)
                            {
                                var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
                                var data = new OfferCacheDataModel(offer);
                                cache.Set(Constants.CacheKeys.OFFER_IDENTIFIER, data);
                            }
                        }
                    }
                    else
                    {
                        this.DisableCaching();
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = ServiceLocator.ServiceProvider.GetService<ILogger>();
                logger?.Error("Preparing layout model failed", ex);
            }
        }

        protected void DisableCaching()
        {
            try
            {
                HttpContext.Current.Response.Cache.SetNoStore();
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetLastModified(DateTime.Now.AddDays(-1));
            }
            catch (Exception ex)
            {

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
