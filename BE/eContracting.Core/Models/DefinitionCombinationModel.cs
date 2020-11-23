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
    [SitecoreType(TemplateId = "{B9B48C83-1724-43B5-8200-9CDE955D6835}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class DefinitionCombinationModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual ProcessModel Process { get; set; }

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
        public virtual SimpleTextModel OfferBenefitsTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferDocumentsForAcceptanceTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferDocumentsForAcceptanceText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferDocumentsForAcceptanceSection1 { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferDocumentsForAcceptanceSection2 { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferObligatoryDocumentsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferObligatoryAdditionalDocsText { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferObligatoryAdditionalDocsHelp { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferObligatoryDocumentsNote { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferSupplementDocumentsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferSupplementDocumentsText { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferOtherProductsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferOtherProductsAcceptDocumentsText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferOtherProductsText { get; set; }

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
