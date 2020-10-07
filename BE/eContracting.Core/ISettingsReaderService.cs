﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Abstractions
{
    public interface ISettingsReaderService
    {
        IEnumerable<LoginTypeModel> GetAuthenticationSettings();

        [Obsolete]
        SiteSettingsModel GetGeneralSettings();

        Link GetPageLink(PageLinkTypes type);

        SiteSettingsModel GetSiteSettings();
    }
}
