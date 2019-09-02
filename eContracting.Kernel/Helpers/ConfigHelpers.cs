// <copyright file="ConfigHelpers.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Helpers
{
    using System;
    using eContracting.Kernel.GlassItems;
    using eContracting.Kernel.GlassItems.Settings;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Fields;
    using Log = Sitecore.Diagnostics.Log;

    /// <summary>
    /// Implementation of the configurtation helpers.
    /// </summary>
    public static class ConfigHelpers
    {
        /// <summary>
        /// Gets the general settings.
        /// </summary>
        /// <returns></returns>
        public static GeneralSettings GetGeneralSettings()
        {
            using (var context = new SitecoreContext())
            {
                if (context == null)
                {
                    Log.Error("Error when getting sitecore context in GetGeneralSettings()", Type.DefaultBinder);
                    return null;
                }

                var generalSetttings = context.GetItem<GeneralSettings>(ItemPaths.GeneralSettings);
                if (generalSetttings == null)
                {
                    Log.Error("Error when getting general sitecore settings for eContracting", Type.DefaultBinder);
                    return null;
                }

                return generalSetttings;
            }
        }

        /// <summary>
        /// Gets the page link.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Link GetPageLink(PageLinkType type)
        {
            var generalSettings = GetGeneralSettings();
            if (generalSettings == null)
            {
                throw new InvalidOperationException("Cannot load general settings.");
            }

            switch (type)
            {
                case PageLinkType.SessionExpired:
                    return generalSettings.SessionExpired;
                case PageLinkType.UserBlocked:
                    return generalSettings.UserBlocked;
                case PageLinkType.AcceptedOffer:
                    return generalSettings.AcceptedOffer;
                case PageLinkType.WrongUrl:
                    return generalSettings.WrongUrl;
                case PageLinkType.OfferExpired:
                    return generalSettings.OfferExpired;
                case PageLinkType.ThankYou:
                    return generalSettings.ThankYou;
                case PageLinkType.SystemError:
                    return generalSettings.SystemError;
                case PageLinkType.Welcome:
                    return generalSettings.Welcome;
                default:
                    throw new InvalidOperationException("Invalid page type.");
            }
        }

        /// <summary>
        /// Gets site root item '/sitecore/content/eContracting'.
        /// <para>
        ///     If any of value for 'ServiceUrl', 'ServiceUser', 'ServicePassword' or 'DelayAfterFailedAttempts' is empty,
        ///     it tries to take value from Sitecore configuration settings as fallback.
        /// </para>
        /// </summary>
        /// <returns>
        ///  Instance of <see cref="SiteRootModel"/> with values from Sitecore or from Sitecore configuration settings.
        /// </returns>
        public static SiteRootModel GetSiteSettings()
        {
            SitecoreContext context = new SitecoreContext();
            var model = context.GetRootItem<SiteRootModel>();

            if (string.IsNullOrWhiteSpace(model.ServiceUrl))
            {
                model.ServiceUrl = Sitecore.Configuration.Settings.GetAppSetting("eContracting.ServiceUrl");
            }

            if (string.IsNullOrWhiteSpace(model.ServiceUser))
            {
                model.ServiceUser = Sitecore.Configuration.Settings.GetAppSetting("eContracting.ServiceUser");
            }

            if (string.IsNullOrWhiteSpace(model.ServicePassword))
            {
                model.ServicePassword = Sitecore.Configuration.Settings.GetAppSetting("eContracting.ServicePassword");
            }

            if (string.IsNullOrWhiteSpace(model.DelayAfterFailedAttempts))
            {
                model.DelayAfterFailedAttempts = Sitecore.Configuration.Settings.GetAppSetting("eContracting.DelayAfterFailedAttempts");
            }

            return model;
        }
    }
}
