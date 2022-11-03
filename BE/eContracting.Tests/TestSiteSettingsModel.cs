using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Tests
{
    public class TestSiteSettingsModel : ISiteSettingsModel
    {
        public Link SessionExpired { get; set; }
        public Link UserBlocked { get; set; }
        public Link AcceptedOffer { get; set; }
        public Link WrongUrl { get; set; }
        public Link Offer { get; set; }
        public Link ExpiredOffer { get; set; }
        public Link ThankYou { get; set; }
        public Link SystemError { get; set; }
        public Link Welcome { get; set; }
        public Link Login { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceUser { get; set; }
        public string ServicePassword { get; set; }
        public string AllowedDocumentTypes { get; set; }
        public string AllowedDocumentTypesDescription { get; set; }
        public string MaxUploadSizeDescription { get; set; }
        public int GroupResultingFileSizeLimitKBytes { get; set; }
        public int TotalResultingFilesSizeLimitKBytes { get; set; }
        public int GroupFileCountLimit { get; set; }
        public int MaxImageWidthAfterResize { get; set; }
        public int MaxImageHeightAfterResize { get; set; }
        public int MinImageWidthNoResize { get; set; }
        public int MinImageHeightNoResize { get; set; }
        public int MaxGroupOptimizationRounds { get; set; }
        public int MaxAllGroupsOptimizationRounds { get; set; }
        public int SingleUploadFileSizeLimitKBytes { get; set; }
        public string SigningServiceUrl { get; set; }
        public string SigningServiceUser { get; set; }
        public string SigningServicePassword { get; set; }
        public string ApplicationUnavailableTitle { get; set; }
        public string ApplicationUnavailableText { get; set; }
        public string GoogleTagManager { get; set; }
        public ICookieLawSettingsModel CookieLawSettings { get; set; }
        public bool EnableCleanupApiTrigger { get; set; }
        public double CleanupFilesOlderThanDays { get; set; }
        public double CleanupLogsOlderThanDays { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public string CookieBotId { get; set; }
        public Guid TemplateId { get; set; }
    }
}
