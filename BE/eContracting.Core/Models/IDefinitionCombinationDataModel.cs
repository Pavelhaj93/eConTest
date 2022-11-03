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
        [SitecoreField("Steps_MODELO_OFERTA_01")]
        IStepsModel StepsDefault { get; set; }

        [SitecoreField("Steps_MODELO_OFERTA_02")]
        IStepsModel Steps { get; set; }

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

        #region Summary

        ISimpleTextModel SummaryTitle { get; set; }

        IRichTextModel SummaryMainText { get; set; }

        ISimpleTextModel SummaryUnfinishedOfferTitle { get; set; }

        IRichTextModel SummaryUnfinishedOfferText { get; set; }

        ICallMeBackModalWindow SummaryCallMeBack { get; set; }

        #endregion

        #region Offer

        /// <summary>
        /// Gets or sets <c>Hlavní nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Text pod hlavním nadpisem</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferMainText { get; set; }

        /// <summary>
        /// Gets or sets <c>Zobrazit Stručný přehled nabídky?</c>.
        /// </summary>
        [SitecoreField]
        bool OfferPerexShow { get; set; }

        /// <summary>
        /// Gets or sets <c>Stručný přehled nabídky - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferPerexTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Zobrazit zelený box s dárečky?</c>.
        /// </summary>
        [SitecoreField]
        bool OfferGiftsShow { get; set; }

        /// <summary>
        /// Gets or sets <c>Dárečky - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferGiftsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Benefity - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferBenefitsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Dokumenty - popis</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferDocumentsDescription { get; set; }

        #region Documents for electronic acceptance

        /// <summary>
        /// Gets or sets <c>Dokumenty k elektronické akceptaci - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferDocumentsForElectronicAcceptanceTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Dokumenty k elektronické akceptaci - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferDocumentsForElectronicAcceptanceText { get; set; }

        #endregion

        #region Documents for acceptance

        /// <summary>
        /// Gets or sets <c>Dokumenty k akceptaci - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferDocumentsForAcceptanceTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Dokumenty k akceptaci - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferDocumentsForAcceptanceText { get; set; }

        #endregion

        #region Documents for sign

        /// <summary>
        /// Gets or sets <c>Dokumenty k podepsání - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferDocumentsForSignTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Dokumenty k podepsání - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferDocumentsForSignText { get; set; }

        #endregion

        #region Documents for upload

        /// <summary>
        /// Gets or sets <c>Dokumenty k nahrání - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferUploadsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Vlastní dokumenty k nahrání - nadpis</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferUploadsExtraText { get; set; }

        /// <summary>
        /// Gets or sets <c>Vlastní dokumenty k nahrání - nápověda</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferUploadsExtraHelp { get; set; }

        /// <summary>
        /// Gets or sets <c>Dokumenty k nahrání - poznámka</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferUploadsNote { get; set; }

        #endregion

        #region Additional services

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - přehled - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesSummaryTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - přehled - nadpis</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAdditionalServicesSummaryText { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - popis</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAdditionalServicesDescription { get; set; }

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - dokumenty - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferAdditionalServicesDocsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAdditionalServicesDocsText { get; set; }

        /// <summary>
        /// Gets or sets <c>Doplňkové služby - dokumenty - poznámka</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAdditionalServicesNote { get; set; }

        #endregion

        #region Other products

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferOtherProductsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - přehled - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferOtherProductsSummaryTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní - přehled - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferOtherProductsSummaryText { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - popis</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferOtherProductsDescription { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - dokumenty - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferOtherProductsDocsTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - dokumenty - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferOtherProductsDocsText { get; set; }

        /// <summary>
        /// Gets or sets <c>Ostatní produkty - dokumenty - poznámka</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferOtherProductsNote { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets <c>Potvrzení akceptace - nadpis</c>.
        /// </summary>
        [SitecoreField]
        ISimpleTextModel OfferAcceptTitle { get; set; }

        /// <summary>
        /// Gets or sets <c>Potvrzení akceptace - text</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAcceptText { get; set; }
        
        /// <summary>
        /// Gets or sets <c>Vybraný doplňující seznam hodnot - popis</c>.
        /// </summary>
        [SitecoreField]
        string OfferSelectedListLabel { get; set; }

        /// <summary>
        /// Gets or sets <c>Vybraný doplňující seznam hodnot - nepovinné</c>.
        /// </summary>
        [SitecoreField]
        IListCollectionModel OfferSelectedList { get; set; }


        [SitecoreField]
        Image OfferVisitedAbMatrixPixel { get; set; }

        #endregion

        #region Accepted

        /// <summary>
        /// Gets or sets <c>Text pro akceptovanou nabídku</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferAcceptedMainText { get; set; }

        /// <summary>
        /// Gets or sets <c>Pixel pro AB testování - zobrazena stránka akceptované nabídky</c>.
        /// </summary>
        [SitecoreField]
        Image OfferAcceptedAbMatrixPixel { get; set; }

        #endregion

        #region Expired

        /// <summary>
        /// Gets or sets <c>Text pro vypršenou nabídku</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel OfferExpiredMainText { get; set; }

        /// <summary>
        /// Gets or sets <c>Pixel pro AB testování - zobrazena stránka expirované nabídky</c>.
        /// </summary>
        [SitecoreField]
        Image OfferExpiredAbMatrixPixel { get; set; }

        #endregion

        #region Thank you

        /// <summary>
        /// Gets or sets <c>Text na děkovací stránce pro 2 tajemství</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel MainTextThankYou { get; set; }

        /// <summary>
        /// Gets or sets <c>Pixel pro AB testování - zobrazena stránka s poděkováním</c>.
        /// </summary>
        [SitecoreField]
        IRichTextModel MainTextThankYou2 { get; set; }

        [SitecoreField]
        Image ThankYouAbMatrixPixel { get; set; }

        #endregion
    }
}
