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

        [SitecoreField("ExpiredOffer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ExpiredOffer { get; set; }

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
        public virtual string ApplicationUnavailableTitle { get; set; }

        [SitecoreField]
        public virtual string ApplicationUnavailableText { get; set; }

        [SitecoreField]
        public string GoogleTagManager { get; set; }

        [SitecoreField]
        public virtual CookieLawSettingsModel CookieLawSettings { get; set; }

        [SitecoreField]
        public int GroupResultingFileSizeLimitKBytes { get; set; }

        [SitecoreField]
        public int TotalResultingFilesSizeLimitKBytes { get; set; }

        [SitecoreField]
        public int MaxImageWidthAfterResize { get; set; }

        [SitecoreField]
        public int MaxImageHeightAfterResize { get; set; }

        [SitecoreField]
        public int MinImageWidthNoResize { get; set; }

        [SitecoreField]
        public int MinImageHeightNoResize { get; set; }

        [SitecoreField]
        public int MaxGroupOptimizationRounds { get; set; }

        [SitecoreField]
        public int MaxAllGroupsOptimizationRounds { get; set; }
    }
}
