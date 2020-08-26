using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.GlassItems;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Kernel.Services
{
    public class SitecoreSettingsReaderService : ISettingsReaderService
    {
        public AuthenticationSettingsModel GetAuthenticationSettings()
        {
            using (var context = new SitecoreContext())
            {
                if (context == null)
                {
                    Log.Error("Error when getting sitecore context in GetGeneralSettings()", this);
                    return null;
                }

                var authSetttings = context.GetItem<AuthenticationSettings>(ItemPaths.AuthenticationSettings);

                if (authSetttings == null)
                {
                    Log.Error("Error when getting general sitecore settings for eContracting", this);
                    return null;
                }

                var authsettingsModel = new AuthenticationSettingsModel()
                {
                    AuthFields = authSetttings.AuthenticationSettingItems.Select(a => new AuthenticationSettingsItemModel
                    {
                        Label = a.Label,
                        Key = a.Key,
                        HelpText = a.HelpText,
                        Placeholder = a.Placeholder,
                        EnableForDefault = a.EnableForDefault,
                        EnableForRetention = a.EnableForRetention,
                        EnableForAcquisition = a.EnableForAcquisition,
                        ValidationMessage = a.ValidationMessage,
                        ValidationRegex = a.ValidationRegex
                    }).ToList()
                };

                return authsettingsModel;
            }
        }

        public GeneralSettings GetGeneralSettings()
        {
            using (var context = new SitecoreContext())
            {
                if (context == null)
                {
                    Log.Error("Error when getting sitecore context in GetGeneralSettings()", this);
                    return null;
                }

                var generalSetttings = context.GetItem<GeneralSettings>(ItemPaths.GeneralSettings);
                if (generalSetttings == null)
                {
                    Log.Error("Error when getting general sitecore settings for eContracting", this);
                    return null;
                }

                return generalSetttings;
            }
        }

        public Link GetPageLink(PageLinkType type)
        {
            var generalSettings = this.GetGeneralSettings();

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
                case PageLinkType.Login:
                    return generalSettings.Login;
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
        public SiteRootModel GetSiteSettings()
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
