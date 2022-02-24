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
    /// <seealso cref="eContracting.Models.IBaseSitecoreModel" />
    [SitecoreType(TemplateId = "{20BB0322-6045-4CE9-8B1D-F36CC1303622}", AutoMap = true)]
    public interface IDefinitionCombinationDataModel : IBaseSitecoreModel
    {
        #region Login

        [SitecoreField]
        IEnumerable<ILoginTypeModel> LoginTypes { get; set; }

        [SitecoreField]
        bool LoginTypesRandom { get; set; }

        [SitecoreField]
        IRichTextModel MainTextLogin { get; set; }

        [SitecoreField]
        IRichTextModel MainTextLoginAccepted { get; set; }

        #endregion

        #region Offer

        [SitecoreField]
        ISimpleTextModel OfferTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferMainText { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferPerexTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferGiftsTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferBenefitsTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferCommoditiesTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferCommoditiesText { get; set; }

        [SitecoreField]
        Image OfferVisitedAbMatrixPixel { get; set; }


        #region Documents for accept

        [SitecoreField]
        ISimpleTextModel OfferCommoditiesAcceptTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferCommoditiesAcceptText { get; set; }

        #endregion

        #region Documents for sign

        [SitecoreField]
        ISimpleTextModel OfferCommoditiesSignTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferCommoditiesSignText { get; set; }

        #endregion

        #region Documents for upload

        [SitecoreField]
        ISimpleTextModel OfferUploadsTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferUploadsExtraText { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferUploadsExtraHelp { get; set; }

        [SitecoreField]
        IRichTextModel OfferUploadsNote { get; set; }

        #endregion

        #region Additional services

        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesSummaryTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesDocsTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferAdditionalServicesDocsText { get; set; }

        [SitecoreField]
        IRichTextModel OfferAdditionalServicesNote { get; set; }

        #endregion

        #region Other products

        [SitecoreField]
        ISimpleTextModel OfferOtherProductsTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferOtherProductsSummaryTitle { get; set; }

        [SitecoreField]
        ISimpleTextModel OfferOtherProductsDocsTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferOtherProductsDocsText { get; set; }

        [SitecoreField]
        IRichTextModel OfferOtherProductsNote { get; set; }

        #endregion

        [SitecoreField]
        ISimpleTextModel OfferAcceptTitle { get; set; }

        [SitecoreField]
        IRichTextModel OfferAcceptText { get; set; }

        [SitecoreField]
        Image OfferAcceptedAbMatrixPixel { get; set; }

        #endregion

        [SitecoreField]
        IRichTextModel OfferAcceptedMainText { get; set; }

        [SitecoreField]
        IRichTextModel OfferExpiredMainText { get; set; }

        [SitecoreField]
        Image OfferExpiredAbMatrixPixel { get; set; }

        [SitecoreField]
        IRichTextModel MainTextThankYou { get; set; }

        [SitecoreField]
        IRichTextModel MainTextThankYou2 { get; set; }

        [SitecoreField]
        Image ThankYouAbMatrixPixel { get; set; }

        [SitecoreField]
        string OfferSelectedListLabel { get; set; }

        [SitecoreField]
        IListCollectionModel OfferSelectedList { get; set; }
    }
}
