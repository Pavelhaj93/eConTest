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
    public interface ISiteSettingsModel : IBaseSitecoreModel
    {
        #region Pages

        [SitecoreField("SessionExpired", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link SessionExpired { get; set; }

        [SitecoreField("UserBlocked", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link UserBlocked { get; set; }

        [SitecoreField("AcceptedOffer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link AcceptedOffer { get; set; }

        [SitecoreField("WrongUrl", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link WrongUrl { get; set; }

        [SitecoreField("Summary", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Summary { get; set; }

        [SitecoreField("Offer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Offer { get; set; }

        [SitecoreField("ExpiredOffer", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link ExpiredOffer { get; set; }

        [SitecoreField("ThankYou", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link ThankYou { get; set; }

        [SitecoreField("SystemError", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link SystemError { get; set; }

        [SitecoreField("Welcome", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Welcome { get; set; }

        [SitecoreField("Login", UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Login { get; set; }

        #endregion

        #region Offer service

        [SitecoreField]
        string ServiceUrl { get; set; }

        [SitecoreField]
        string ServiceUser { get; set; }

        [SitecoreField]
        string ServicePassword { get; set; }

        #endregion

        #region Upload settings

        [SitecoreField]
        string AllowedDocumentTypes { get; set; }        

        [SitecoreField]
        string AllowedDocumentTypesDescription { get; set; }

        [SitecoreField]
        string MaxUploadSizeDescription { get; set; }

        [SitecoreField]
        int GroupResultingFileSizeLimitKBytes { get; set; }

        [SitecoreField]
        int TotalResultingFilesSizeLimitKBytes { get; set; }

        [SitecoreField]
        int GroupFileCountLimit { get; set; }

        [SitecoreField]
        int MaxImageWidthAfterResize { get; set; }

        [SitecoreField]
        int MaxImageHeightAfterResize { get; set; }

        [SitecoreField]
        int MinImageWidthNoResize { get; set; }

        [SitecoreField]
        int MinImageHeightNoResize { get; set; }

        [SitecoreField]
        int MaxGroupOptimizationRounds { get; set; }

        [SitecoreField]
        int MaxAllGroupsOptimizationRounds { get; set; }

        [SitecoreField]
        int SingleUploadFileSizeLimitKBytes { get; set; }
        

        #endregion

        #region Signing service

        [SitecoreField]
        string SigningServiceUrl { get; set; }

        [SitecoreField]
        string SigningServiceUser { get; set; }

        [SitecoreField]
        string SigningServicePassword { get; set; }

        #endregion

        [SitecoreField]
        string ApplicationUnavailableTitle { get; set; }

        [SitecoreField]
        string ApplicationUnavailableText { get; set; }

        [SitecoreField]
        string GoogleTagManager { get; set; }

        [SitecoreField]
        ICookieLawSettingsModel CookieLawSettings { get; set; }

        [SitecoreField]
        string CookieBotId { get; set; }

        #region Cleanup settings
        [SitecoreField]
        bool EnableCleanupApiTrigger { get; set; }

        [SitecoreField]
        double CleanupFilesOlderThanDays { get; set; }

        [SitecoreField]
        double CleanupLogsOlderThanDays { get; set; }

        #endregion

        [SitecoreField]
        IStepsModel Steps_Default { get; set; }

        [SitecoreField]
        IStepsModel Steps_NoLogin { get; set; }

        [SitecoreField]
        IStepsModel Steps_ProductChange { get; set; }
    }
}
