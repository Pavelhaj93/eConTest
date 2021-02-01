using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Xunit;

namespace eContracting.Services.Tests
{
    public class OfferJsonDescriptorTests : BaseTestClass
    {
        [Fact]
        public void GetAccepted_Returns_OfferCommoditiesAcceptTitle_When_Group_COMMODITY_And_File_For_Accept()
        {
            var offer = this.CreateOffer();
            var definition = new DefinitionCombinationModel();
            definition.OfferCommoditiesAcceptTitle = new SimpleTextModel();
            definition.OfferCommoditiesAcceptTitle.Text = "Dokumenty k akceptaci";
            var template = new OfferAttachmentXmlModel();
            template.Group = "COMMODITY";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferCommoditiesAcceptTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferCommoditiesSignTitle_When_Group_COMMODITY_And_File_For_Sign()
        {
            var offer = this.CreateOffer();
            var definition = new DefinitionCombinationModel();
            definition.OfferCommoditiesSignTitle = new SimpleTextModel();
            definition.OfferCommoditiesSignTitle.Text = "Plná moc";
            var template = new OfferAttachmentXmlModel();
            template.Group = "COMMODITY";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferCommoditiesSignTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferAdditionalServicesTitle_When_Group_DSL()
        {
            var offer = this.CreateOffer();
            var definition = new DefinitionCombinationModel();
            definition.OfferAdditionalServicesTitle = new SimpleTextModel();
            definition.OfferAdditionalServicesTitle.Text = "Smlouva o pronájmu";
            var template = new OfferAttachmentXmlModel();
            template.Group = "DSL";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferAdditionalServicesTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferOtherProductsDocsTitle_When_Group_Not_COMMODITY_Or_DSL()
        {
            var offer = this.CreateOffer();
            var definition = new DefinitionCombinationModel();
            definition.OfferOtherProductsDocsTitle = new SimpleTextModel();
            definition.OfferOtherProductsDocsTitle.Text = "Smlouva o pronájmu";
            var template = new OfferAttachmentXmlModel();
            template.Group = "NONCOMMODITY";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferOtherProductsDocsTitle.Text, result.Groups.First().Title);
        }


        //[Fact]
        public void GetAccepted_Returns_COMMODITY_Attachments()
        {
            var offer = this.CreateOffer();
            var definition = new DefinitionCombinationModel();
            definition.OfferCommoditiesAcceptTitle = new SimpleTextModel();
            definition.OfferCommoditiesAcceptTitle.Text = "abc";

            var template1 = new OfferAttachmentXmlModel();
            template1.Group = "COMMODITY";
            template1.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            template1.SignReq = null;
            var attachment1 = new OfferAttachmentModel(template1, "applicatin/pdf", "aaa", null, null);

            var template2 = new OfferAttachmentXmlModel();
            template2.Group = "DSL";
            template2.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            template1.SignReq = null;
            var attachment2 = new OfferAttachmentModel(template2, "applicatin/pdf", "bbb", null, null);

            var template3 = new OfferAttachmentXmlModel();
            template3.Group = "NONCOMMODITY";
            template3.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            template1.SignReq = null;
            var attachment3 = new OfferAttachmentModel(template3, "applicatin/pdf", "ccc", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new[] { attachment1, attachment2, attachment3 });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferCommoditiesAcceptTitle.Text, result.Groups.First().Title);
        }

        [Theory]
        [InlineData(1, false, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Produkt", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE", "elektřina Garance 24")]
        [InlineData(1, false, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_1", "Platnost nabídky do", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1", "23.12.2020")]
        [InlineData(1, false, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_2", "Délka fixace", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_2", "24 měsíců od platnosti smlouvy")]
        [InlineData(1, false, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_3", "Platnost smlouvy", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_3", "Dnem podpisusmlouvy zákazníkem")]
        [InlineData(1, false, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_4", "Předpokládaná účinnost smlouvy", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_4", "Zatím neurčeno")]
        [InlineData(2, true, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Produkt", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE", "elektřina Garance 24")]
        [InlineData(2, true, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_1", "Platnost nabídky do", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1", "23.12.2020")]
        [InlineData(2, true, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_2", "Délka fixace", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_2", "24 měsíců od platnosti smlouvy")]
        [InlineData(2, true, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_3", "Platnost smlouvy", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_3", "Dnem podpisusmlouvy zákazníkem")]
        [InlineData(2, true, "COMMODITY_OFFER_SUMMARY_ATRIB_NAME_4", "Předpokládaná účinnost smlouvy", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_4", "Zatím neurčeno")]
        public void GetNew_Calls_GetPerex_In_Version_Greater_Than_1(int version, bool expectedPerex, string nameKey, string nameValue, string valueKey, string valueValue)
        {
            var offer = this.CreateOffer(version);
            offer.TextParameters.Add(nameKey, nameValue);
            offer.TextParameters.Add(valueKey, valueValue);
            var definition = new DefinitionCombinationModel();
            definition.OfferPerexTitle = new SimpleTextModel();
            definition.OfferPerexTitle.Text = "Perex";

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetNew(offer);

            if (expectedPerex)
            {
                Assert.True(result.Perex.Parameters.Length == 1);
                Assert.Contains(result.Perex.Parameters, x => x.Title == offer.TextParameters[nameKey]);
                Assert.Contains(result.Perex.Parameters, x => x.Value == offer.TextParameters[valueKey]);
            }
            else
            {
                Assert.Null(result.Perex);
            }
        }

        [Theory]
        [InlineData(1, false, "BENEFITS_NOW"      , "X", "BENEFITS_NOW_NAME"      , "Poukázka Kaufland 100 Kč", "BENEFITS_NOW_COUNT"      , "1", "BENEFITS_NOW_IMAGE"      , "PKZ", "BENEFITS_NOW_INTRO"      , "Děkujeme za váš čas ...")]
        [InlineData(1, false, "BENEFITS_NEXT_SIGN", "X", "BENEFITS_NEXT_SIGN_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_SIGN_COUNT", "2", "BENEFITS_NEXT_SIGN_IMAGE", "LED", "BENEFITS_NEXT_SIGN_INTRO", "Děkujeme za váš čas ...")]
        [InlineData(1, false, "BENEFITS_NEXT_TZD" , "X", "BENEFITS_NEXT_TZD_NAME" , "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_TZD_COUNT" , "3", "BENEFITS_NEXT_TZD_IMAGE" , "DET", "BENEFITS_NEXT_TZD_INTRO" , "Děkujeme za váš čas ...")]
        [InlineData(2, true, "BENEFITS_NOW", "X", "BENEFITS_NOW_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NOW_COUNT", "1", "BENEFITS_NOW_IMAGE", "PKZ", "BENEFITS_NOW_INTRO", "Děkujeme za váš čas ...")]
        [InlineData(2, true, "BENEFITS_NEXT_SIGN", "X", "BENEFITS_NEXT_SIGN_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_SIGN_COUNT", "2", "BENEFITS_NEXT_SIGN_IMAGE", "LED", "BENEFITS_NEXT_SIGN_INTRO", "Děkujeme za váš čas ...")]
        [InlineData(2, true, "BENEFITS_NEXT_TZD", "X", "BENEFITS_NEXT_TZD_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_TZD_COUNT", "3", "BENEFITS_NEXT_TZD_IMAGE", "DET", "BENEFITS_NEXT_TZD_INTRO", "Děkujeme za váš čas ...")]
        public void GetNew_Calls_GetGifts_In_Version_Greater_Than_1(int version, bool expectedGifts, string checkKey, string checkValue, string nameKey, string nameValue, string countKey, string countValue, string imageKey, string imageValue, string introKey, string introValue)
        {
            var offer = this.CreateOffer(version);
            offer.TextParameters.Add("BENEFITS", "X");
            offer.TextParameters.Add(checkKey, checkValue);
            offer.TextParameters.Add(nameKey, nameValue);
            offer.TextParameters.Add(countKey, countValue);
            offer.TextParameters.Add(imageKey, imageValue);
            offer.TextParameters.Add(introKey, introValue);
            var definition = new DefinitionCombinationModel();
            definition.OfferBenefitsTitle = new SimpleTextModel();
            definition.OfferBenefitsTitle.Text = "Dárečky";

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetNew(offer);

            if (expectedGifts)
            {
                Assert.True(result.Gifts.Groups.Count() == 1);
                Assert.True(result.Gifts.Groups.First().Params.Count() == 1);
                Assert.Contains(result.Gifts.Groups, x => x.Params.First().Count == Convert.ToInt32(countValue));
                Assert.Contains(result.Gifts.Groups, x => x.Params.First().Icon == imageValue);
                Assert.Contains(result.Gifts.Groups, x => x.Params.First().Title == nameValue);
            }
            else
            {
                Assert.Null(result.Gifts);
            }
        }

        [Theory]
        [InlineData(1, false, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE", "garance poklesu ceny")]
        [InlineData(1, false, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_1", "ve druhém i třetím roce garance")]
        [InlineData(1, false, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_2", "férová nabídka")]
        [InlineData(1, false, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_3", "předem známá neměnná výše ceny")]
        [InlineData(1, false, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_4", "v prvním roce nižší cena")]
        [InlineData(2, true, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE", "garance poklesu ceny")]
        [InlineData(2, true, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_1", "ve druhém i třetím roce garance")]
        [InlineData(2, true, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_2", "férová nabídka")]
        [InlineData(2, true, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_3", "předem známá neměnná výše ceny")]
        [InlineData(2, true, "COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_4", "v prvním roce nižší cena")]
        public void GetNew_Calls_GetCommoditySalesArguments_In_Version_Greater_Than_1(int version, bool expectedArguments, string argKey, string argValue)
        {
            var offer = this.CreateOffer(version);
            offer.TextParameters.Add(argKey, argValue);

            var definition = new DefinitionCombinationModel();
            definition.OfferBenefitsTitle = new SimpleTextModel();
            definition.OfferBenefitsTitle.Text = "Dárečky";

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetNew(offer);

            if (expectedArguments)
            {
                Assert.True(result.SalesArguments.Params.Count() == 1);
                Assert.Contains(result.SalesArguments.Params, x => x.Value == argValue);
            }
            else
            {
                Assert.Null(result.SalesArguments);
            }
        }

        [Fact]
        public void GetPerex_Returns_Null_When_Parameters_Not_Found()
        {
            var offer = this.CreateOffer(2);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetPerex(offer.TextParameters, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetPerex_Returns_Null_When_Parameter_Value_Not_Found()
        {
            var offer = this.CreateOffer(2);
            offer.TextParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Produkt");
            // COMMODITY_OFFER_SUMMARY_ATRIB_VALUE not included

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetPerex(offer.TextParameters, null);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public void GetPerex_Returns_Not_Limited_Number_Of_Parameters(int count)
        {
            var offer = this.CreateOffer(2);

            for (int i = 0; i < count; i++)
            {
                var name = "COMMODITY_OFFER_SUMMARY_ATRIB_NAME";
                var value = "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE";

                if (i > 0)
                {
                    name += "_" + i; // for example COMMODITY_OFFER_SUMMARY_ATRIB_NAME_1
                    value += "_" + i; // for example COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1
                }

                offer.TextParameters.Add(name, "Produkt");
                offer.TextParameters.Add(value, "elektřina Garance 26");
            }

            var definition = new DefinitionCombinationModel();
            definition.OfferPerexTitle = new SimpleTextModel();
            definition.OfferPerexTitle.Text = "Perex";

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetPerex(offer.TextParameters, definition);

            Assert.Equal(count, result.Parameters.Length);
        }

        [Fact]
        public void GetGifts_Returns_Null_When_Parameteres_Doenst_Contain_BENEFITS_Equal_X()
        {
            var offer = this.CreateOffer(2);
            // this is missing: offer.TextParameters.Add("BENEFITS", "X");

            var definition = new DefinitionCombinationModel();
            definition.OfferPerexTitle = new SimpleTextModel();
            definition.OfferPerexTitle.Text = "Perex";

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreContext.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetGifts(offer.TextParameters, definition);

            Assert.Null(result);
        }
    }
}
