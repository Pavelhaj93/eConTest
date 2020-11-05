using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting
{
    /// <summary>
    /// Global settings reader.
    /// </summary>
    public interface ISettingsReaderService
    {
        /// <summary>
        /// Gets definition by <see cref="OfferModel.Process"/> and <see cref="OfferModel.ProcessType"/> from '/sitecore/content/eContracting2/Definitions'.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>Definition or <see cref="ApplicationException"/> when not found.</returns>
        DefinitionCombinationModel GetDefinition(OfferModel offer);

        /// <summary>
        /// Gets definition by <paramref name="process"/> and <paramref name="processType"/> from '/sitecore/content/eContracting2/Definitions'.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns>Definition or null.</returns>
        DefinitionCombinationModel GetDefinition(string process, string processType);

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
        /// <exception cref="MissingDatasourceException">No processes found.</exception>
        IEnumerable<ProcessModel> GetAllProcesses();

        /// <summary>
        /// Gets all process types from '/sitecore/content/eContracting2/Settings/ProcessTypes'.
        /// </summary>
        /// <returns>List of all defined process types.</returns>
        /// <exception cref="MissingDatasourceException">No process types found</exception>
        IEnumerable<ProcessTypeModel> GetAllProcessTypes();

        /// <summary>
        /// Gets all login types from '/sitecore/content/eContracting2/Settings/LoginTypes'.
        /// </summary>
        /// <returns>List of all defined login types.</returns>
        /// <exception cref="MissingDatasourceException">No login types found</exception>
        IEnumerable<LoginTypeModel> GetAllLoginTypes();

        /// <summary>
        /// Gets configuration for <see cref="IApiService"/>.
        /// </summary>
        /// <returns>The configuration.</returns>
        CacheApiServiceOptions GetApiServiceOptions();

        RichTextModel GetMainTextForLogin(OfferModel offer);

        /// <summary>
        /// Gets url from site settings.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Url or exception.</returns>
        /// <exception cref="InvalidOperationException">When <see cref="PageLinkTypes"/> cannot be resolved.</exception>
        string GetPageLink(PageLinkTypes type);

        /// <summary>
        /// Gets the site settings.
        /// </summary>
        /// <returns>Model or exception.</returns>
        /// <exception cref="MissingDatasourceException">When settings cannot be found.</exception>
        SiteSettingsModel GetSiteSettings();

        /// <summary>
        /// Get all steps from collection where <paramref name="currentStep"/> is in and mark current as '<see cref="ProcessStepModel.IsSelected"/> = <c>true</c>'.
        /// </summary>
        /// <param name="currentStep">The current step.</param>
        /// <returns>Array of all related steps.</returns>
        ProcessStepModel[] GetSteps(ProcessStepModel currentStep);
    }
}
