using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    /// <summary>
    /// Represents data for combination.
    /// </summary>
    /// <seealso cref="eContracting.Models.BaseSitecoreModel" />
    [SitecoreType(TemplateId = "{20BB0322-6045-4CE9-8B1D-F36CC1303622}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class DefinitionCombinationDataModel : BaseSitecoreModel
    {
        #region Login

        [SitecoreField]
        public virtual IEnumerable<LoginTypeModel> LoginTypes { get; set; }

        [SitecoreField]
        public virtual bool LoginTypesRandom { get; set; }

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
        public virtual SimpleTextModel OfferGiftsTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferBenefitsTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferCommoditiesTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferCommoditiesText { get; set; }

        [SitecoreField]
        public virtual Image OfferVisitedAbMatrixPixel { get; set; }


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
        public virtual SimpleTextModel OfferAdditionalServicesSummaryTitle { get; set; }

        [SitecoreField]
        public virtual SimpleTextModel OfferAdditionalServicesDocsTitle { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferAdditionalServicesDocsText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferAdditionalServicesNote { get; set; }

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

        [SitecoreField]
        public virtual Image OfferAcceptedAbMatrixPixel { get; set; }

        #endregion

        [SitecoreField]
        public virtual RichTextModel OfferAcceptedMainText { get; set; }

        [SitecoreField]
        public virtual RichTextModel OfferExpiredMainText { get; set; }

        [SitecoreField]
        public virtual RichTextModel MainTextThankYou { get; set; }

        [SitecoreField]
        public virtual Image ThankYouAbMatrixPixel { get; set; }

    }
}
