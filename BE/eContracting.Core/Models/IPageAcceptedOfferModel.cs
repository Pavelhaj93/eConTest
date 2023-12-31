﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{0F225E4F-AA1E-44CC-91F4-19D4FB5C859C}", AutoMap = true)]
    public interface IPageAcceptedOfferModel : IBaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        [SitecoreField]
        string PageTitle { get; set; }

        [SitecoreField]
        string ButtonDashboardLabel { get; set; }
    }
}
