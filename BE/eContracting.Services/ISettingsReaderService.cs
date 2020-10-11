using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Services
{
    public interface ISettingsReaderService
    {
        IEnumerable<LoginTypeModel> GetAuthenticationSettings();

        [Obsolete]
        SiteSettingsModel GetGeneralSettings();

        Link GetPageLink(PageLinkTypes type);

        SiteSettingsModel GetSiteSettings();

        string GetMainText(OfferModel offer);
    }
}
