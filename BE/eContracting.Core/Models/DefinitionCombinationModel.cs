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
    [SitecoreType(TemplateId = "{B9B48C83-1724-43B5-8200-9CDE955D6835}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class DefinitionCombinationModel : BaseSitecoreModel
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

        #region Login

        [SitecoreField]
        public virtual IEnumerable<LoginTypeModel> LoginTypes { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextLogin { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextLoginAccepted { get; set; }

        #endregion

        #region Offer

        [SitecoreField]
        public virtual SimpleTextModel OfferTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferMainText { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferPerexTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferBenefitsTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferCommoditiesTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferCommoditiesText { get; set; }

        #region Documents for accept

        [SitecoreField]
        public virtual SimpleTextModel OfferCommoditiesAcceptTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferCommoditiesAcceptText { get; set; }

        #endregion

        #region Documents for sign

        [SitecoreField]
        public virtual SimpleTextModel OfferCommoditiesSignTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferCommoditiesSignText { get; set; }

        #endregion

        #region Documents for upload

        [SitecoreField]
        public virtual SimpleTextModel OfferUploadsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferUploadsExtraText { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferUploadsExtraHelp { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferUploadsNote { get; set; }

        #endregion

        #region Additional services

        [SitecoreField]
        public virtual SimpleTextModel OfferAdditionalServicesTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferAdditionalServicesText { get; set; }

        #endregion

        #region Other products

        [SitecoreField]
        public virtual SimpleTextModel OfferOtherProductsTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferOtherProductsSummaryTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferOtherProductsDocsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferOtherProductsDocsText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferOtherProductsNote { get; set; }

        #endregion

        [SitecoreField]
        public virtual SimpleTextModel OfferAcceptTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferAcceptText { get; set; }

        #endregion

        [SitecoreField]
        public virtual RichTextModel OfferAcceptedMainText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferExpiredMainText { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextThankYou { get; set; }
    }
}
