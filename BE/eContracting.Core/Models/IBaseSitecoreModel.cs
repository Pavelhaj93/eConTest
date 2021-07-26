using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Base model for Sitecore item.
    /// </summary>
    public interface IBaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets Sitecore item ID.
        /// </summary>
        /// <value>
        [SitecoreId]
        Guid ID { get; set; }

        /// <summary>
        /// Gets or sets Sitecore item name.
        /// </summary>
        [SitecoreInfo(Type = SitecoreInfoType.Name)]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets Sitecore item display name.
        /// </summary>
        [SitecoreInfo(Type = SitecoreInfoType.DisplayName)]
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets full path of the item.
        /// </summary>
        [SitecoreInfo(Type = SitecoreInfoType.FullPath)]
        string Path { get; set; }
    }
}
