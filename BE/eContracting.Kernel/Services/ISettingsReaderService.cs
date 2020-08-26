using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.GlassItems;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.Services
{
    public interface ISettingsReaderService
    {
        AuthenticationSettingsModel GetAuthenticationSettings();

        GeneralSettings GetGeneralSettings();

        Link GetPageLink(PageLinkType type);

        SiteRootModel GetSiteSettings();
    }
}
