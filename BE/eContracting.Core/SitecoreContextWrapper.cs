using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Sites;

namespace eContracting
{
    /// <summary>
    /// Wrapper over 'Sitecore.Context'.
    /// </summary>
    /// <seealso cref="eContracting.IContextWrapper" />
    [ExcludeFromCodeCoverage]
    public class SitecoreContextWrapper : IContextWrapper
    {
        /// <inheritdoc/>
        public string GetSetting(string name)
        {
            return Sitecore.Configuration.Settings.GetSetting(name);
        }

        /// <inheritdoc/>
        public string GetSiteRoot()
        {
            return Sitecore.Context.Site.RootPath;
        }

        /// <inheritdoc/>
        public bool IsEditMode()
        {
            return Sitecore.Context.PageMode.IsExperienceEditorEditing;
        }

        /// <inheritdoc/>
        public bool IsNormalMode()
        {
            return Sitecore.Context.PageMode.IsNormal;
        }

        /// <inheritdoc/>
        public bool IsPreviewMode()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsPreviewModel()
        {
            return Sitecore.Context.PageMode.IsPreview;
        }

        /// <inheritdoc/>
        public void SetDisplayMode(DisplayMode model)
        {
            throw new NotImplementedException();
        }
    }
}
