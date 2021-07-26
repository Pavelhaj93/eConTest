using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Tests
{
    public class MemoryDefinitionCombinationModel : IDefinitionCombinationModel
    {
        public IProcessModel Process { get; set; }
        public IProcessTypeModel ProcessType { get; set; }
        public IEnumerable<ILoginTypeModel> LoginTypes { get; set; }
        public bool LoginTypesRandom { get; set; }
        public IRichTextModel MainTextLogin { get; set; }
        public IRichTextModel MainTextLoginAccepted { get; set; }
        public ISimpleTextModel OfferTitle { get; set; }
        public IRichTextModel OfferMainText { get; set; }
        public ISimpleTextModel OfferPerexTitle { get; set; }
        public ISimpleTextModel OfferGiftsTitle { get; set; }
        public ISimpleTextModel OfferBenefitsTitle { get; set; }
        public ISimpleTextModel OfferCommoditiesTitle { get; set; }
        public IRichTextModel OfferCommoditiesText { get; set; }
        public Image OfferVisitedAbMatrixPixel { get; set; }
        public ISimpleTextModel OfferCommoditiesAcceptTitle { get; set; }
        public IRichTextModel OfferCommoditiesAcceptText { get; set; }
        public ISimpleTextModel OfferCommoditiesSignTitle { get; set; }
        public IRichTextModel OfferCommoditiesSignText { get; set; }
        public ISimpleTextModel OfferUploadsTitle { get; set; }
        public IRichTextModel OfferUploadsExtraText { get; set; }
        public ISimpleTextModel OfferUploadsExtraHelp { get; set; }
        public IRichTextModel OfferUploadsNote { get; set; }
        public ISimpleTextModel OfferAdditionalServicesTitle { get; set; }
        public ISimpleTextModel OfferAdditionalServicesSummaryTitle { get; set; }
        public ISimpleTextModel OfferAdditionalServicesDocsTitle { get; set; }
        public IRichTextModel OfferAdditionalServicesDocsText { get; set; }
        public IRichTextModel OfferAdditionalServicesNote { get; set; }
        public ISimpleTextModel OfferOtherProductsTitle { get; set; }
        public ISimpleTextModel OfferOtherProductsSummaryTitle { get; set; }
        public ISimpleTextModel OfferOtherProductsDocsTitle { get; set; }
        public IRichTextModel OfferOtherProductsDocsText { get; set; }
        public IRichTextModel OfferOtherProductsNote { get; set; }
        public ISimpleTextModel OfferAcceptTitle { get; set; }
        public IRichTextModel OfferAcceptText { get; set; }
        public Image OfferAcceptedAbMatrixPixel { get; set; }
        public IRichTextModel OfferAcceptedMainText { get; set; }
        public IRichTextModel OfferExpiredMainText { get; set; }
        public Image OfferExpiredAbMatrixPixel { get; set; }
        public IRichTextModel MainTextThankYou { get; set; }
        public Image ThankYouAbMatrixPixel { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
    }
}
