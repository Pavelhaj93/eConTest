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
        /// <summary>
        /// Gets definition by <see cref="OfferModel.Process"/> and <see cref="OfferModel.ProcessType"/> from '/sitecore/content/eContracting2/Definitions'.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>Definition or <see cref="ApplicationException"/> when not found.</returns>
        DefinitionCombinationModel GetDefinition(OfferModel offer);

        /// <summary>
        /// Gets available login types for <paramref name="offer"/> defined in <see cref="DefinitionCombinationModel"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>List of defined login types or if there is not defined any, one random login type from all of them.</returns>
        IEnumerable<LoginTypeModel> GetLoginTypes(OfferModel offer);

        /// <summary>
        /// Gets all processes from '/sitecore/content/eContracting2/Settings/Processes'.
        /// </summary>
        /// <returns>List of all defined processes.</returns>
        IEnumerable<ProcessModel> GetAllProcesses();

        /// <summary>
        /// Gets all process types from '/sitecore/content/eContracting2/Settings/ProcessTypes'.
        /// </summary>
        /// <returns>List of all defined process types.</returns>
        IEnumerable<ProcessTypeModel> GetAllProcessTypes();

        /// <summary>
        /// Gets all login types from '/sitecore/content/eContracting2/Settings/LoginTypes'.
        /// </summary>
        /// <returns>List of all defined login types.</returns>
        IEnumerable<LoginTypeModel> GetAllLoginTypes();

        /// <summary>
        /// Gets configuration for <see cref="IApiService"/>.
        /// </summary>
        /// <returns>The configuration.</returns>
        CacheApiServiceOptions GetApiServiceOptions();

        RichTextModel GetMainTextForLogin(OfferModel offer);

        [Obsolete]
        SiteSettingsModel GetGeneralSettings();

        string GetPageLink(PageLinkTypes type);

        SiteSettingsModel GetSiteSettings();
    }
}
