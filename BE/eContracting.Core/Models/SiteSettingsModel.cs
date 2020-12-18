using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{212C0D14-4BDF-47A6-846A-CDF7A549B169}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class SiteSettingsModel : BaseSitecoreModel
    {
        [SitecoreField("SessionExpired", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SessionExpired { get; set; }

        [SitecoreField("UserBlocked", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link UserBlocked { get; set; }

        [SitecoreField("AcceptedOffer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link AcceptedOffer { get; set; }

        [SitecoreField("WrongUrl", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link WrongUrl { get; set; }

        [SitecoreField("Offer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Offer { get; set; }

        [SitecoreField("OfferExpired", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link OfferExpired { get; set; }

        [SitecoreField("ThankYou", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ThankYou { get; set; }

        [SitecoreField("SystemError", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SystemError { get; set; }

        [SitecoreField("Welcome", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Welcome { get; set; }

        [SitecoreField("Login", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Login { get; set; }

        [SitecoreField]
        public virtual string IAmInformed { get; set; }

        [SitecoreField]
        public virtual string IAgree { get; set; }

        [SitecoreField]
        public virtual string Accept { get; set; }

        [SitecoreField]
        public virtual string AppNotAvailable { get; set; }

        [SitecoreField]
        public virtual string IdentityCardNumber { get; set; }

        [SitecoreField]
        public virtual string UsedPostalCode { get; set; }

        [SitecoreField]
        public virtual string PermanentResidencePostalCode { get; set; }

        [SitecoreField]
        public virtual string AccountNumber { get; set; }

        [SitecoreField]
        public virtual string DefaultSalutation { get; set; }

        [SitecoreField]
        public virtual string SignFailure { get; set; }

        [SitecoreChildren(IsLazy = false)]
        public virtual IEnumerable<GeneralTextsSettings> Texts { get; set; }

        [SitecoreField]
        public string ServiceUrl { get; set; }

        [SitecoreField]
        public string ServiceUser { get; set; }

        [SitecoreField]
        public string ServicePassword { get; set; }

        [SitecoreField]
        public string SigningServiceUrl { get; set; }

        [SitecoreField]
        public string SigningServiceUser { get; set; }

        [SitecoreField]
        public string SigningServicePassword { get; set; }

        [SitecoreField]
        public int MaxFailedAttempts { get; set; }

        [SitecoreField]
        public string DelayAfterFailedAttempts { get; set; }

        [SitecoreField]
        public virtual string ApplicationUnavailableTitle { get; set; }

        [SitecoreField]
        public virtual string ApplicationUnavailableText { get; set; }

        [SitecoreField]
        public string GoogleTagManager { get; set; }

        [SitecoreField]
        public virtual CookieLawSettingsModel CookieLawSettings { get; set; }

        /// <summary>
        /// Gets the delay after failed attempts as <see cref="TimeSpan"/>
        /// </summary>
        /// <value>
        /// Parsed value from <see cref="DelayAfterFailedAttempts"/>. If parsing failed, return default value '00:15:00'
        /// </value>
        [SitecoreIgnore]
        public TimeSpan DelayAfterFailedAttemptsTimeSpan
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.DelayAfterFailedAttempts))
                {
                    TimeSpan value;

                    if (TimeSpan.TryParse(this.DelayAfterFailedAttempts, out value))
                    {
                        return value;
                    }
                }

                return new TimeSpan(0, 15, 0);
            }
        }
    }
}
