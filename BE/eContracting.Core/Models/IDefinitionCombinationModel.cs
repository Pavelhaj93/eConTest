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
    /// <seealso cref="eContracting.Models.IBaseSitecoreModel" />
    //[SitecoreType(TemplateId = "{B9B48C83-1724-43B5-8200-9CDE955D6835}", AutoMap = true)]
    public interface IDefinitionCombinationModel : IDefinitionCombinationDataModel
    {
        /// <summary>
        /// Gets or sets related <c>BUS_PROCESS</c> definition.
        /// </summary>
        [SitecoreField]
        IProcessModel Process { get; set; }

        /// <summary>
        /// Gets or sets related <c>BUS_TYPE</c> definition.
        /// </summary>
        [SitecoreField]
        IProcessTypeModel ProcessType { get; set; }

        /// <summary>
        /// Gets or sets if this is related to offer with <see cref="Constants.OfferAttributes.ORDER_ORIGIN"/> = "X".
        /// </summary>
        [SitecoreField("IsCreatedInInnosvet")]
        bool IsOrderOrigin { get; set; }

        /// <summary>
        /// Gets or sets if this model is related to offer with <see cref="Constants.OfferAttributes.NO_PROD_CHNG"/> = "X".
        /// </summary>
        [SitecoreField("IsNoProductChange")]
        bool IsNoProductChange { get; set; }
    }
}
