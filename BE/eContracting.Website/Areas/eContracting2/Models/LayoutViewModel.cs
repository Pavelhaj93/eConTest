using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Web;
using Castle.Core.Logging;
using eContracting.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
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

        public string CookieBotId { get; protected set; }

        protected readonly ISettingsReaderService SettingsReader;
        protected readonly IOfferService OfferService;
        protected readonly IUserService UserService;
        protected readonly IDataRequestCacheService RequestCacheService;
        protected readonly IContextWrapper ContextWrapper;
        protected readonly ILogger Logger;

        public LayoutViewModel()
        {
            this.SettingsReader = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            this.UserService = ServiceLocator.ServiceProvider.GetRequiredService<IUserService>();
            this.OfferService = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            this.RequestCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IDataRequestCacheService>();
            this.ContextWrapper = ServiceLocator.ServiceProvider.GetRequiredService<IContextWrapper>();
            this.Logger = ServiceLocator.ServiceProvider.GetService<ILogger>();
        }

        public LayoutViewModel(
            ISettingsReaderService settingsReader,
            IOfferService offerService,
            IUserService userService,
            IContextWrapper contextWrapper,
            ILogger logger)
        {
            this.SettingsReader = settingsReader;
            this.OfferService = offerService;
            this.UserService = userService;
            this.ContextWrapper = contextWrapper;
            this.Logger = logger;
        }

        public override void Initialize(Rendering rendering)
        {
            try
            {
                var guid = HttpContext.Current.Request.QueryString?[Constants.QueryKeys.GUID];
                this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] Initializing data for '{HttpContext.Current.Request.RawUrl}' [{HttpContext.Current.Request.HttpMethod}]");

                var settings = this.SettingsReader.GetSiteSettings();

                if (rendering.Item.TemplateID == Constants.TemplateIds.PageHome)
                {
                    this.Logger.Warn(guid, $"[{nameof(LayoutViewModel)}] User visited homepage");
                    var url = settings.WrongUrl.Url + "?code=" + Constants.ErrorCodes.HOMEPAGE;
                    HttpContext.Current.Response.Redirect(url, true);
                    return;
                }

                base.Initialize(rendering);

                if (rendering.Item.TemplateID == Constants.TemplateIds.PageLogout)
                {
                    this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] It's logout page. Nothing to process.");
                    return;
                }

                this.GoogleTagManagerKey = settings.GoogleTagManager;
                this.CookieBotId = settings.CookieBotId;

                //this.ProcessErrorDescription(rendering);
                //this.PageMetaTitle = Sitecore.Context.Item["PageTitle"] + (this.ErrorCodeDescription != null ? " - " + this.ErrorCodeDescription : "");
                this.PageMetaTitle = Sitecore.Context.Item["PageTitle"];

                if (!Sitecore.Context.PageMode.IsNormal)
                {
                    var processCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS];
                    var processTypeCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS_TYPE];

                    if (string.IsNullOrEmpty(processCode) || string.IsNullOrEmpty(processTypeCode))
                    {
                        var defaultDefinition = this.SettingsReader.GetDefinitionDefault();
                        processCode = defaultDefinition.Process.Code;
                        processTypeCode = defaultDefinition.ProcessType.Code;
                    }

                    var data = new OfferCacheDataModel(processCode, processTypeCode);
                    this.RequestCacheService.SaveOffer(Constants.FakeOfferGuid, data);
                }
                else if (HttpContext.Current.Request.HttpMethod == HttpMethod.Get.Method)
                {
                    var isLoginPage = rendering.Item.TemplateID == Constants.TemplateIds.PageLogin;

                    // first try to initiliaze user
                    this.InitializeUser(guid, rendering, isLoginPage);
                    // then user can be used to get offer
                    this.InitializeOffer(guid, rendering);

                    if (!isLoginPage)
                    {
                        this.DisableCaching();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.Error($"[{nameof(LayoutViewModel)}] Preparing layout model failed", ex);
            }
        }

        protected void InitializeOffer(string guid, Rendering rendering)
        {
            if (string.IsNullOrEmpty(guid))
            {
                this.Logger.Warn(guid, $"[{nameof(LayoutViewModel)}] Cannot initialize guid, it's empty but shouldn't be");
                return;
            }

            try
            {
                var offer = this.OfferService.GetOffer(guid);

                if (offer == null)
                {
                    this.Logger.Info(guid, $"[{nameof(LayoutViewModel)}] Offer not found");
                    return;
                }

                var cacheData = new OfferCacheDataModel(offer);
                this.RequestCacheService.SaveOffer(guid, cacheData);
                this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] Offer data stored in cache");
            }
            catch (EndpointNotFoundException exception)
            {
                this.Logger.Fatal(guid, $"[{nameof(LayoutViewModel)}] Cannot retrieve data for offer", exception);
                this.UserService.Logout(guid);
            }
        }

        protected void InitializeUser(string guid, Rendering rendering, bool isLoginPage)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ContextWrapper.GetQueryValue(Constants.QueryKeys.RENEW_SESSION)))
                {
                    return;
                }

                var user = this.UserService.GetUser();

                if (this.UserService.TryAuthenticateUser(guid, user))
                {
                    if (this.UserService.IsUserValid(guid, user))
                    {
                        this.UserService.RefreshAuthorizationIfNeeded(guid, user);
                    }
                    else
                    {
                        this.Logger.Info(guid, $"[{nameof(LayoutViewModel)}] {user} is not valid in current context, calling logout ...");
                        this.UserService.Logout(guid, user, AUTH_METHODS.COGNITO);
                    }
                }

                //if (!this.UserService.IsUserValid(guid, user))
                //{
                //    this.Logger.Info(guid, $"[{nameof(LayoutViewModel)}] {user} is not valid in current context, calling logout ...");
                //    this.UserService.Logout(guid, user, AUTH_METHODS.COGNITO);
                //}
                //else
                //{
                //    this.UserService.RefreshAuthorizationIfNeeded(guid, user);
                //}

                //if (isLoginPage)
                //{
                //    if (!this.UserService.CanAuthenticate(guid))
                //    {
                //        this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] {user} cannot be authenticated");
                //        return;
                //    }

                //    if (!this.UserService.IsAuthorizedFor(guid))
                //    {
                //        this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] User is not authorized for this guid, trying to udpate his data from context ..");

                //        if (this.UserService.TryUpdateUserFromContext(guid, user))
                //        {
                //            this.UserService.SaveUser(guid, user);

                //            if (this.OfferService.CanReadOffer(guid, user, OFFER_TYPES.QUOTPRX))
                //            {
                //                user.SetAuth(guid, AUTH_METHODS.COGNITO);
                //                this.UserService.Authenticate(guid, user);
                //                this.Logger.Debug(guid, $"[{nameof(LayoutViewModel)}] User '{user}' has been authenticated");
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                this.Logger.Error(guid, $"[{nameof(LayoutViewModel)}] Cannot correctly initialize user", ex);
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
