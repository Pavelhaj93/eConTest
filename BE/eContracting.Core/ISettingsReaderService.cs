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
        /// Gets a value indicating whether to save group file to <c>debug</c> folder.
        /// </summary>
        bool SaveFilesToDebugFolder { get; }

        /// <summary>
        /// Gets a value indicating whether to show debug messages to frontend.
        /// </summary>
        bool ShowDebugMessages { get; }

        /// <summary>
        /// Gets the session timeout in minutes.
        /// </summary>
        int SessionTimeout { get; }

        /// <summary>
        /// Gets minimum seconds to refresh access token.
        /// </summary>
        /// <returns>Number of seconds.</returns>
        int CognitoMinSecondsToRefreshToken { get; }

        /// <summary>
        /// Gets delay in seconds after submit process to SAP is done, so user wait for extra time.
        /// </summary>
        int SubmitOfferDelay { get; }

        /// <summary>
        /// Gets delay in seconds after cancel process to SAP is done, so user wait for extra time.
        /// </summary>
        int CancelOfferDelay { get; }

        /// <summary>
        /// Gets <c>default</c> definition. Cannot be null.
        /// </summary>
        /// <returns>The definition.</returns>
        /// <exception cref="EcontractingMissingDatasourceException">Default definition combination not found. Cannot proceed with other execution without appropriate data.</exception>
        IDefinitionCombinationModel GetDefinitionDefault();

        /// <summary>
        /// Gets definition by <see cref="OfferModel.Process"/> and <see cref="OfferModel.ProcessType"/> from '/sitecore/content/eContracting2/Definitions'.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>Definition or <see cref="ApplicationException"/> when not found.</returns>
        IDefinitionCombinationModel GetDefinition(OfferModel offer);

        /// <summary>
        /// Gets definition by <paramref name="process"/> and <paramref name="processType"/> from '/sitecore/content/eContracting2/Definitions'.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns>Definition matched by <paramref name="process"/> and <paramref name="processType"/> or default one.</returns>
        /// <exception cref="ApplicationException">When even default definition was not found.</exception>
        IDefinitionCombinationModel GetDefinition(string process, string processType);

        /// <summary>
        /// Gets all defined matrix definitions.
        /// </summary>
        IDefinitionCombinationModel[] GetAllDefinitions();

        /// <summary>
        /// Gets all define product information.
        /// </summary>
        /// <returns></returns>
        IProductInfoModel[] GetAllProductInfos();

        /// <summary>
        /// Gets available login types for <paramref name="offer"/> defined in <see cref="IDefinitionCombinationModel"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>List of defined login types or if there is not defined any, one random login type from all of them.</returns>
        IEnumerable<ILoginTypeModel> GetLoginTypes(OfferModel offer);

        /// <summary>
        /// Gets all processes from '/sitecore/content/eContracting2/Settings/Processes'.
        /// </summary>
        /// <returns>List of all defined processes.</returns>
        /// <exception cref="EcontractingMissingDatasourceException">No processes found.</exception>
        IEnumerable<IProcessModel> GetAllProcesses();

        /// <summary>
        /// Gets all process types from '/sitecore/content/eContracting2/Settings/ProcessTypes'.
        /// </summary>
        /// <returns>List of all defined process types.</returns>
        /// <exception cref="EcontractingMissingDatasourceException">No process types found</exception>
        IEnumerable<IProcessTypeModel> GetAllProcessTypes();

        /// <summary>
        /// Gets all login types from '/sitecore/content/eContracting2/Settings/LoginTypes'.
        /// </summary>
        /// <returns>List of all defined login types.</returns>
        /// <exception cref="EcontractingMissingDatasourceException">No login types found</exception>
        IEnumerable<ILoginTypeModel> GetAllLoginTypes();

        /// <summary>
        /// Gets configuration for <see cref="IOfferService"/>.
        /// </summary>
        /// <returns>The configuration.</returns>
        CacheApiServiceOptions GetApiServiceOptions();

        /// <summary>
        /// Gets configuration for <see cref="ISignService"/>.
        /// </summary>
        /// <returns>The configuration.</returns>
        SignApiServiceOptions GetSignApiServiceOptions();

        [Obsolete]
        IRichTextModel GetMainTextForLogin(OfferModel offer);

        /// <summary>
        /// Gets url from site settings.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Url or exception.</returns>
        /// <exception cref="InvalidOperationException">When <see cref="PAGE_LINK_TYPES"/> cannot be resolved.</exception>
        string GetPageLink(PAGE_LINK_TYPES type);

        string GetPageLink(PAGE_LINK_TYPES type, string guid);

        /// <summary>
        /// Gets the site settings.
        /// </summary>
        /// <returns>Model or exception.</returns>
        /// <exception cref="EcontractingMissingDatasourceException">When settings cannot be found.</exception>
        ISiteSettingsModel GetSiteSettings();

        /// <summary>
        /// Get all steps from collection where <paramref name="currentStep"/> is in and mark current as '<see cref="IStepModel.IsSelected"/> = <c>true</c>'.
        /// </summary>
        /// <param name="currentStep">The current step.</param>
        /// <returns>Array of all related steps.</returns>
        [Obsolete]
        IStepModel[] GetSteps(IStepModel currentStep);

        string[] GetXmlNodeNamesExcludeHtml();

        /// <summary>
        /// Gets connection string for <see cref="IUserFileCacheService"/>.
        /// </summary>
        /// <returns></returns>
        string GetCustomDatabaseConnectionString();

        /// <summary>
        /// Gets mapping from old keys to new ones with respect to offer <paramref name="version"/>.
        /// </summary>
        /// <remarks>
        ///     <para><see cref="KeyValuePair{TKey, TValue}.Key"/> is what you have.</para>
        ///     <para><see cref="KeyValuePair{TKey, TValue}.Value"/> is what you need.</para>
        /// </remarks>
        /// <param name="version">Offer version.</param>
        KeyValuePair<string, string>[] GetBackCompatibleTextParametersKeys(int version);

        /// <summary>
        /// Gets settings for Cognito Auth service.
        /// </summary>
        CognitoSettingsModel GetCognitoSettings();
    }
}
