﻿using System;
using System.Collections.Generic;
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
    public abstract class BaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets Sitecore item ID.
        /// </summary>
        /// <value>
        [SitecoreId]
        public virtual Guid ID { get; set; }

        /// <summary>
        /// Gets or sets Sitecore item name.
        /// </summary>
        [SitecoreInfo(Type = SitecoreInfoType.Name)]
        public virtual string Name { get; set; }
    }
}
