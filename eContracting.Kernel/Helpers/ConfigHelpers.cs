﻿using System;
using eContracting.Kernel.GlassItems.Settings;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Kernel.Helpers
{
    public static class ConfigHelpers
    {
        public static GeneralSettings GetGeneralSettings()
        {
            using (var context = new SitecoreContext())
            {
                if (context == null)
                {
                    Log.Error("Error when getting sitecore context in GetGeneralSettings()",Type.DefaultBinder);
                    return null;
                }

                var generalSetttings = context.GetItem<GeneralSettings>(ItemPaths.GeneralSettings);
                if (generalSetttings == null)
                {
                    Log.Error("Error when getting general sitecore settings for eContracting",Type.DefaultBinder);
                    return null;
                }

                return generalSetttings;
            }
        }

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
                default:
                    throw new InvalidOperationException("Invalid page type.");
            }
        }
    }
}
