using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Sites;

namespace eContracting
{
    /// <summary>
    /// Represents wrapper over context (static) values.
    /// </summary>
    public interface IContextWrapper
    {
        /// <summary>
        /// Sets the display mode.
        /// </summary>
        /// <param name="model">The model.</param>
        void SetDisplayMode(DisplayMode model);

        /// <summary>
        /// Gets value from 'Sitecore.Configuration.Settings.GetSetting()'.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>String or null.</returns>
        string GetSetting(string name);

        /// <summary>
        /// Gets the site root path.
        /// </summary>
        /// <returns>Value from 'Sitecore.Context.Site.RootPath'.</returns>
        string GetSiteRoot();

        /// <summary>
        /// Gets current browser agent description.
        /// </summary>
        /// <returns>Browser agent identifier or null.</returns>
        string GetBrowserAgent();

        /// <summary>
        /// Gets current IP address.
        /// </summary>
        /// <returns></returns>
        string GetIpAddress();

        /// <summary>
        /// Determines whether Sitecore context page mode is in normal mode.
        /// </summary>
        bool IsNormalMode();

        /// <summary>
        /// Determines whether Sitecore context page mode is in preview mode.
        /// </summary>
        bool IsPreviewMode();

        /// <summary>
        /// Determines whether Sitecore context page mode is in editing mode.
        /// </summary>
        bool IsEditMode();
    }
}
