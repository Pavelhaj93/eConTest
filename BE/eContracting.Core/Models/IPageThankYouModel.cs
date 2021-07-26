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
    /// <summary>
    /// Represents model for thank you page.
    /// </summary>
    /// <seealso cref="IBasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{4F3B54BA-3DBC-408C-9573-F9F86AC0C3C7}", AutoMap = true)]
    public interface IPageThankYouModel : IBasePageWithStepsModel
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Link1 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Link2 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link Link3 { get; set; }

        #region Google Tag Manager Data

        [SitecoreField]
        string IndividualLabel { get; set; }

        [SitecoreField]
        string CampaignLabel { get; set; }

        [SitecoreField]
        string ElectricityLabel { get; set; }

        [SitecoreField]
        string GasLabel { get; set; }

        [SitecoreField("eCat")]
        string GoogleAnalytics_eCat { get; set; }

        [SitecoreField("eAct")]
        string GoogleAnalytics_eAct { get; set; }

        [SitecoreField("eLab")]
        string GoogleAnalytics_eLab { get; set; }

        #endregion
    }
}
