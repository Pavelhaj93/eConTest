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
        #region Pages

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

        #endregion

        #region Offer service

        [SitecoreField]
        public virtual string ServiceUrl { get; set; }

        [SitecoreField]
        public virtual string ServiceUser { get; set; }

        [SitecoreField]
        public virtual string ServicePassword { get; set; }

        #endregion

        #region Upload settings

        [SitecoreField]
        public virtual string AllowedDocumentTypes { get; set; }

        [SitecoreIgnore]
        public string[] AllowedDocumentTypesList
        {
            get
            {
                var list = new List<string>();

                var ar = this.AllowedDocumentTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < ar.Length; i++)
                {
                    var v = ar[i].Trim();

                    if (!string.IsNullOrEmpty(v))
                    {
                        list.Add(v);
                    }
                }

                return list.ToArray();
            }
        }

        [SitecoreField]
        public virtual string AllowedDocumentTypesDescription { get; set; }

        [SitecoreField]
        public virtual string MaxUploadSizeDescription { get; set; }

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

        [SitecoreField]
        public int SingleUploadFileSizeLimitKBytes { get; set; }
        

        #endregion

        #region Signing service

        [SitecoreField]
        public virtual string SigningServiceUrl { get; set; }

        [SitecoreField]
        public virtual string SigningServiceUser { get; set; }

        [SitecoreField]
        public virtual string SigningServicePassword { get; set; }

        #endregion

        [SitecoreField]
        public virtual string ApplicationUnavailableTitle { get; set; }

        [SitecoreField]
        public virtual string ApplicationUnavailableText { get; set; }

        [SitecoreField]
        public string GoogleTagManager { get; set; }

        [SitecoreField]
        public virtual CookieLawSettingsModel CookieLawSettings { get; set; }


        #region Cleanup settings
        [SitecoreField]
        public virtual bool EnableCleanupApiTrigger { get; set; }

        [SitecoreField]
        public virtual double CleanupFilesOlderThanDays { get; set; }


        #endregion
    }
}
