using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{212C0D14-4BDF-47A6-846A-CDF7A549B169}", AutoMap = true)]
    public class SiteSettingsModel : BaseSitecoreModel
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SessionExpired { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link UserBlocked { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link AcceptedOffer { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link WrongUrl { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Offer { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link OfferExpired { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ThankYou { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SystemError { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Welcome { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
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

        /// <summary>
        /// Gets texts by value name of <paramref name="offerType"/> from <see cref="Texts"/> collection or null if not found.
        /// </summary>
        /// <param name="offerType">Type of the offer.</param>
        /// <returns>Instance of <see cref="GeneralTextsSettings"/> or null when not found.</returns>
        public GeneralTextsSettings GetTexts(OfferTypes offerType)
        {
            var name = Enum.GetName(typeof(OfferTypes), offerType);
            return this.Texts.FirstOrDefault(x => x.Name == name);
        }

        public string GetSignInFailure(OfferTypes offerType)
        {
            var value = this.SignFailure;
            var texts = this.GetTexts(offerType);

            // should not happen, default offer type doesn't need this
            if (texts != null)
            {
                value = texts.SignFailure;
            }

            return value;
        }
    }
}
