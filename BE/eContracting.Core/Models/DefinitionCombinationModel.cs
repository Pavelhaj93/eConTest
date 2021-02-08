using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Shell.Applications.ContentEditor;

namespace eContracting.Models
{
    /// <summary>
    /// Represents combination of <see cref="Process"/> and <see cref="ProcessType"/> with their relevant settings.
    /// </summary>
    /// <seealso cref="eContracting.Models.BaseSitecoreModel" />
    //[SitecoreType(TemplateId = "{B9B48C83-1724-43B5-8200-9CDE955D6835}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class DefinitionCombinationModel : DefinitionCombinationDataModel
    {
        /// <summary>
        /// Gets or sets related process definition.
        /// </summary>
        [SitecoreField]
        public virtual ProcessModel Process { get; set; }

        /// <summary>
        /// Gets or sets related process type definition.
        /// </summary>
        [SitecoreField]
        public virtual ProcessTypeModel ProcessType { get; set; }

        /// <summary>
        /// Gets a value indicating whether this definition is default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get
            {
                return this.Process == null || this.ProcessType == null;
            }
        }
    }
}
