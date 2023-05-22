using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using eContracting.Models;
using eContracting.Tests;
using JSNLog.Infrastructure;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace eContracting.Services.Tests
{
    public class OfferJsonDescriptorTests : BaseTestClass
    {
        [Fact]
        public void GetAccepted_Returns_OfferCommoditiesAcceptTitle_When_Group_COMMODITY_And_File_For_Accept()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Dokumenty k akceptaci");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferDocumentsForAcceptanceTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;
            var template = new OfferAttachmentXmlModel();
            template.Group = "COMMODITY";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer, user);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferDocumentsForAcceptanceTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferCommoditiesSignTitle_When_Group_COMMODITY_And_File_For_Sign()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Plná moc");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferDocumentsForSignTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var template = new OfferAttachmentXmlModel();
            template.Group = "COMMODITY";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer, user);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferDocumentsForSignTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferAdditionalServicesTitle_When_Group_DSL()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Smlouva o pronájmu");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferAdditionalServicesTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var template = new OfferAttachmentXmlModel();
            template.Group = "DSL";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer, user);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferAdditionalServicesTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_OfferOtherProductsDocsTitle_When_Group_Not_COMMODITY_Or_DSL()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Smlouva o pronájmu");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferOtherProductsDocsTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var template = new OfferAttachmentXmlModel();
            template.Group = "NONCOMMODITY";
            template.SignReq = null;
            var attachment = new OfferAttachmentModel(template, "applicatin/pdf", "asdfas", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new[] { attachment });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer, user);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferOtherProductsDocsTitle.Text, result.Groups.First().Title);
        }

        [Fact]
        public void GetAccepted_Returns_COMMODITY_Attachments()
        {
            var offer = this.CreateOffer();
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "abc");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferDocumentsForAcceptanceTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var template1 = new OfferAttachmentXmlModel();
            template1.Group = "COMMODITY";
            template1.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            template1.SignReq = null;
            var attachment1 = new OfferAttachmentModel(template1, "applicatin/pdf", "aaa", null, null);

            //var template2 = new OfferAttachmentXmlModel();
            //template2.Group = "DSL";
            //template2.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            //template1.SignReq = null;
            //var attachment2 = new OfferAttachmentModel(template2, "applicatin/pdf", "bbb", null, null);

            //var template3 = new OfferAttachmentXmlModel();
            //template3.Group = "NONCOMMODITY";
            //template3.Printed = Constants.FileAttributeValues.CHECK_VALUE;
            //template1.SignReq = null;
            //var attachment3 = new OfferAttachmentModel(template3, "applicatin/pdf", "ccc", null, null);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new[] { attachment1 });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAccepted(offer, user);

            Assert.True(result.Groups.Count() == 1);
            Assert.Equal(definition.OfferDocumentsForAcceptanceTitle.Text, result.Groups.First().Title);
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
            var user = this.CreateAnonymousUser(offer);
            offer.First().TextParameters.Add(nameKey, nameValue);
            offer.First().TextParameters.Add(valueKey, valueValue);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Perex");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferPerexShow, true);
            mockDefinition.SetupProperty(x => x.OfferPerexTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetNew(offer, user);

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

        //[Theory]
        //[InlineData(1, false, "BENEFITS_NOW", "X", "BENEFITS_NOW_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NOW_COUNT", "1", "BENEFITS_NOW_IMAGE", "PKZ", "BENEFITS_NOW_INTRO", "Děkujeme za váš čas ...")]
        //[InlineData(1, false, "BENEFITS_NEXT_SIGN", "X", "BENEFITS_NEXT_SIGN_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_SIGN_COUNT", "2", "BENEFITS_NEXT_SIGN_IMAGE", "LED", "BENEFITS_NEXT_SIGN_INTRO", "Děkujeme za váš čas ...")]
        //[InlineData(1, false, "BENEFITS_NEXT_TZD", "X", "BENEFITS_NEXT_TZD_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_TZD_COUNT", "3", "BENEFITS_NEXT_TZD_IMAGE", "DET", "BENEFITS_NEXT_TZD_INTRO", "Děkujeme za váš čas ...")]
        //[InlineData(2, true, "BENEFITS_NOW", "X", "BENEFITS_NOW_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NOW_COUNT", "1", "BENEFITS_NOW_IMAGE", "PKZ", "BENEFITS_NOW_INTRO", "Děkujeme za váš čas ...")]
        //[InlineData(2, true, "BENEFITS_NEXT_SIGN", "X", "BENEFITS_NEXT_SIGN_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_SIGN_COUNT", "2", "BENEFITS_NEXT_SIGN_IMAGE", "LED", "BENEFITS_NEXT_SIGN_INTRO", "Děkujeme za váš čas ...")]
        //[InlineData(2, true, "BENEFITS_NEXT_TZD", "X", "BENEFITS_NEXT_TZD_NAME", "Poukázka Kaufland 100 Kč", "BENEFITS_NEXT_TZD_COUNT", "3", "BENEFITS_NEXT_TZD_IMAGE", "DET", "BENEFITS_NEXT_TZD_INTRO", "Děkujeme za váš čas ...")]
        //public void GetNew_Calls_GetGifts_In_Version_Greater_Than_1(int version, bool expectedGifts, string checkKey, string checkValue, string nameKey, string nameValue, string countKey, string countValue, string imageKey, string imageValue, string introKey, string introValue)
        //{
        //    var offer = this.CreateOffer(version);
        //    var user = this.CreateAnonymousUser(offer);
        //    offer.TextParameters.Add("BENEFITS", "X");
        //    offer.TextParameters.Add(checkKey, checkValue);
        //    offer.TextParameters.Add(nameKey, nameValue);
        //    offer.TextParameters.Add(countKey, countValue);
        //    offer.TextParameters.Add(imageKey, imageValue);
        //    offer.TextParameters.Add(introKey, introValue);

        //    var mockSimpleText = new Mock<ISimpleTextModel>();
        //    mockSimpleText.SetupProperty(x => x.Text, "Dárečky");
        //    var mockDefinition = new Mock<IDefinitionCombinationModel>();
        //    mockDefinition.SetupProperty(x => x.OfferGiftsShow, true);
        //    mockDefinition.SetupProperty(x => x.OfferGiftsTitle, mockSimpleText.Object);
        //    var definition = mockDefinition.Object;

        //    var logger = new MemoryLogger();
        //    var textService = new MemoryTextService();
        //    var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
        //    var mockOfferService = new Mock<IOfferService>();
        //    mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
        //    var mockSettingsReaderService = new Mock<ISettingsReaderService>();
        //    mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

        //    var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
        //    var result = service.GetNew(offer, user);

        //    if (expectedGifts)
        //    {
        //        Assert.True(result.Gifts.Groups.Count() == 1);
        //        Assert.True(result.Gifts.Groups.First().Params.Count() == 1);
        //        Assert.Contains(result.Gifts.Groups, x => x.Params.First().Count == Convert.ToInt32(countValue));
        //        Assert.Contains(result.Gifts.Groups, x => x.Params.First().Icon == imageValue);
        //        Assert.Contains(result.Gifts.Groups, x => x.Params.First().Title == nameValue);
        //    }
        //    else
        //    {
        //        Assert.Null(result.Gifts);
        //    }
        //}

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
            offer.First().TextParameters.Add(argKey, argValue);
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Dárečky");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferBenefitsTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetDefinition(offer)).Returns(definition);

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetNew(offer, user);

            if (expectedArguments)
            {
                Assert.True(result.SalesArguments.Arguments.Count() == 1);
                Assert.Contains(result.SalesArguments.Arguments, x => x.Value == argValue);
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
            var user = this.CreateAnonymousUser(offer);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetPerex(offer.TextParameters, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetPerex_Returns_Null_When_Parameter_Value_Not_Found()
        {
            var offer = this.CreateOffer(2);
            offer.TextParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Produkt");
            // COMMODITY_OFFER_SUMMARY_ATRIB_VALUE not included
            var user = this.CreateAnonymousUser(offer);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
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

                offer.First().TextParameters.Add(name, "Produkt");
                offer.First().TextParameters.Add(value, "elektřina Garance 26");
            }

            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Perex");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferPerexTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetPerex(offer.TextParameters, definition);

            Assert.Equal(count, result.Parameters.Length);
        }

        [Fact]
        public void GetGifts_Returns_Null_When_Parameteres_Doenst_Contain_BENEFITS_Equal_X()
        {
            var offer = this.CreateOffer(2);
            // this is missing: offer.TextParameters.Add("BENEFITS", "X");
            var user = this.CreateAnonymousUser(offer);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Perex");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferPerexTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            mockOfferService.Setup(x => x.GetAttachments(offer, user)).Returns(new OfferAttachmentModel[] { });
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetGifts(offer.TextParameters, definition);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetGifts_Returns_Note(string group)
        {
            var expected = "Donec imperdiet lorem orci";
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("BENEFITS", "X");
            textParameters.Add(group, "X");
            textParameters.Add(group + "_NAME", "custom group");
            textParameters.Add("BENEFITS_CLOSE", expected);

            var mockSimpleText = new Mock<ISimpleTextModel>();
            mockSimpleText.SetupProperty(x => x.Text, "Gifts");
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupProperty(x => x.OfferGiftsTitle, mockSimpleText.Object);
            var definition = mockDefinition.Object;

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetGifts(textParameters, definition);

            Assert.Equal(expected, result.Note);
        }

        [Theory(DisplayName = "Skip BENEFITS section when not equals X")]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Null_When_Section_Key_Not_Equals_X(string section)
        {
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(section, new Dictionary<string, string>());

            Assert.Null(result);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Title(string group)
        {
            var expected = "Lorem ipsum dolor sit amet";
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(group, "X");
            textParameters.Add(group + "_INTRO", expected);
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(group, textParameters);

            Assert.NotNull(result);
            Assert.Equal(expected, result.Title);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Empty_Params_When_Name_Missing(string group)
        {
            var expected = 25;
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(group, "X");
            textParameters.Add(group + "_COUNT", $"{expected}");
            // textParameters.Add(group + "_NAME", "abc"); // this is important
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(group, textParameters);

            Assert.Empty(result.Params);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Params_Count(string group)
        {
            var expected = 25;
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(group, "X");
            textParameters.Add(group + "_COUNT", $"{expected}");
            textParameters.Add(group + "_NAME", "group name"); // existing valueName is prerequisite for pass the logic
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(group, textParameters);

            Assert.Equal(expected, result.Params.First().Count);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Params_Title(string group)
        {
            var expected = "Vaše innogy";
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(group, "X");
            textParameters.Add(group + "_NAME", expected);
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(group, textParameters);

            Assert.Equal(expected, result.Params.First().Title);
        }

        [Theory]
        [InlineData("BENEFITS_NOW")]
        [InlineData("BENEFITS_NEXT_SIGN")]
        [InlineData("BENEFITS_NEXT_TZD")]
        public void GetBenefitGroup_Returns_Params_Icon(string group)
        {
            var expected = "PKZ";
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(group, "X");
            textParameters.Add(group + "_IMAGE", expected);
            textParameters.Add(group + "_NAME", "group name"); // existing valueName is prerequisite for pass the logic
            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetBenefitGroup(group, textParameters);

            Assert.Equal(expected, result.Params.First().Icon);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_ACCEPT_LABELs_Are_Striped_From_Html(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Nesnižování záloh<br /></p></body>");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "Služba na míru pro zákazníky");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.Single(result);
            Assert.Equal("Nesnižování záloh", result.First().Title);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_ACCEPT_LABELs_Accept_Only_Sequence_Numbers(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Nesnižování záloh<br /></p></body>");
            textParameters.Add(prefix + "_ACCEPT_LABEL_GUID", "0635F899B3111EECB5A13BA5004CA624");
            textParameters.Add(prefix + "_ACCEPT_LABEL_GUID_1", "0635F899B3111EECB5A13BA500538624");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "Služba na míru pro zákazníky");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.Single(result);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_Gets_Only_With_SALES_ARGUMENTS_ATRIB_VALUE(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Nesnižování záloh<br /></p></body>");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "Služba na míru pro zákazníky");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_Skip_When_SALES_ARGUMENTS_ATRIB_VALUE_Missing(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Nesnižování záloh<br /></p></body>");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_Skip_When_Prefix_Not_Equals_X(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">innogy Domácí asistence<br /></p></body>");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "limit plnění až 10 000 Kč");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_Takes_Only_Summary_Values(string prefix)
        {
            var textParameters = new Dictionary<string, string>();

            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">innogy Domácí asistence<br /></p></body>");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "limit plnění až 10 000 Kč");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_NAME", "Služba");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_NAME_1", "Varianta");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_NAME_2", "Platnost nabídky do");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_VALUE", "innogy Domácí asistence");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_VALUE_1", "Premium");
            textParameters.Add(prefix + "_OFFER_SUMMARY_ATRIB_VALUE_2", "31.05.2022");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            var first = result.FirstOrDefault();
            Assert.NotNull(first);
            Assert.IsType<JsonSalesArgumentsExtendedModel>(first);
            Assert.Equal(3, ((JsonSalesArgumentsExtendedModel)first).Summary.Count());
        }

        [Theory]
        [InlineData("ADD_SERVICES")]
        [InlineData("NONCOMMODITY")]
        [InlineData("COMMODITY")]
        public void GetSalesArgumentsWithPrefix_Returns_Empty_List_When_No_Labels_Found(string prefix)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(prefix, "X");
            textParameters.Add(prefix + "_SALES_ARGUMENTS_ATRIB_VALUE", "limit plnění až 10 000 Kč");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetSalesArgumentsWithPrefix(textParameters, prefix);

            Assert.Empty(result);
        }

        [Fact]
        public void GetOtherProducts_Finds_3_Files()
        {
            var offer = this.CreateOffer(2);

            var files = new List<OfferAttachmentModel>();
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "COMMODITY", Printed = "", SignReq = "", Description = "Ostatní" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Přihláška k pojištění" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Pojistná smlouva" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Informační dokument o pojistném produktu" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "DSL", Printed = "X", SignReq = "", Description = "Dohoda o sjednání doplňkové služby Investor" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "COMMODITY", Printed = "X", SignReq = "", Description = "Dokument prokazující vztah k nemovitosti" }));

            var productInfos = new List<IProductInfoModel>();

            var mockOfferOtherProductsTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsTitle.SetupGet(x => x.Text).Returns("Ostatní");
            var mockOfferOtherProductsSummaryText = new Mock<IRichTextModel>();
            mockOfferOtherProductsSummaryText.SetupGet(x => x.Text).Returns("Dokumenty");
            var mockOfferOtherProductsDescription = new Mock<IRichTextModel>();
            mockOfferOtherProductsDescription.SetupGet(x => x.Text).Returns("Popis u dokumentů");
            var mockOfferOtherProductsNote = new Mock<IRichTextModel>();
            mockOfferOtherProductsNote.SetupGet(x => x.Text).Returns("Poznámky dokumenty");
            var mockOfferOtherProductsSummaryTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsSummaryTitle.SetupGet(x => x.Text).Returns("Název");
            var mockOfferOtherProductsDocsTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsDocsTitle.SetupGet(x => x.Text).Returns("Dokumenty název");
            var mockOfferOtherProductsDocsText = new Mock<IRichTextModel>();
            mockOfferOtherProductsDocsText.SetupGet(x => x.Text).Returns("Text pod názvem");

            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupGet(x => x.OfferOtherProductsTitle).Returns(mockOfferOtherProductsTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsSummaryText).Returns(mockOfferOtherProductsSummaryText.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDescription).Returns(mockOfferOtherProductsDescription.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsNote).Returns(mockOfferOtherProductsNote.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsSummaryTitle).Returns(mockOfferOtherProductsSummaryTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDocsTitle).Returns(mockOfferOtherProductsDocsTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDocsText).Returns(mockOfferOtherProductsDocsText.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetOtherProducts(offer, files.ToArray(), mockDefinition.Object, productInfos.ToArray());

            Assert.Equal(3, result.Files.Count());
        }

        [Fact]
        public void GetOtherProducts_Add_2_Files_To_Same_Mandatory_Group()
        {
            var offer = this.CreateOffer(2);

            var files = new List<OfferAttachmentModel>();
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "",  ItemGuid = "0635F899B3111EDD87A566295264FD77", Group = "COMMODITY", Printed = "", SignReq = "", Description = "Ostatní" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "",  ItemGuid = "0635F899B3111EDD87A566959885DD77", Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Přihláška k pojištění" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "X", ItemGuid = "0635F899B3111EDD87A566959885DD71", Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Pojistná smlouva" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "X", ItemGuid = "0635F899B3111EDD87A566959885DD77", Group = "NONCOMMODITY", Printed = "X", SignReq = "", Description = "Informační dokument o pojistném produktu" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "",  ItemGuid = "0635F899B3111EDD87A5669598963D77", Group = "DSL", Printed = "X", SignReq = "", Description = "Dohoda o sjednání doplňkové služby Investor" }));
            files.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { GroupObligatory = "",  ItemGuid = "0635F899B3111EDD87A566295264FD77", Group = "COMMODITY", Printed = "X", SignReq = "", Description = "Dokument prokazující vztah k nemovitosti" }));

            var productInfos = new List<IProductInfoModel>();

            var mockOfferOtherProductsTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsTitle.SetupGet(x => x.Text).Returns("Ostatní");
            var mockOfferOtherProductsSummaryText = new Mock<IRichTextModel>();
            mockOfferOtherProductsSummaryText.SetupGet(x => x.Text).Returns("Dokumenty");
            var mockOfferOtherProductsDescription = new Mock<IRichTextModel>();
            mockOfferOtherProductsDescription.SetupGet(x => x.Text).Returns("Popis u dokumentů");
            var mockOfferOtherProductsNote = new Mock<IRichTextModel>();
            mockOfferOtherProductsNote.SetupGet(x => x.Text).Returns("Poznámky dokumenty");
            var mockOfferOtherProductsSummaryTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsSummaryTitle.SetupGet(x => x.Text).Returns("Název");
            var mockOfferOtherProductsDocsTitle = new Mock<ISimpleTextModel>();
            mockOfferOtherProductsDocsTitle.SetupGet(x => x.Text).Returns("Dokumenty název");
            var mockOfferOtherProductsDocsText = new Mock<IRichTextModel>();
            mockOfferOtherProductsDocsText.SetupGet(x => x.Text).Returns("Text pod názvem");

            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            mockDefinition.SetupGet(x => x.OfferOtherProductsTitle).Returns(mockOfferOtherProductsTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsSummaryText).Returns(mockOfferOtherProductsSummaryText.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDescription).Returns(mockOfferOtherProductsDescription.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsNote).Returns(mockOfferOtherProductsNote.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsSummaryTitle).Returns(mockOfferOtherProductsSummaryTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDocsTitle).Returns(mockOfferOtherProductsDocsTitle.Object);
            mockDefinition.SetupGet(x => x.OfferOtherProductsDocsText).Returns(mockOfferOtherProductsDocsText.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetOtherProducts(offer, files.ToArray(), mockDefinition.Object, productInfos.ToArray());

            Assert.Equal(2, result.MandatoryGroups.Count);
        }

        [Fact]
        public void GetAllSalesArguments()
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("ADD_SERVICES", "X");
            textParameters.Add("ADD_SERVICES_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Nesnižování záloh<br /></p></body>");
            textParameters.Add("ADD_SERVICES_ACCEPT_LABEL_1", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">iKARTA<br /></p></body>");
            textParameters.Add("ADD_SERVICES_SALES_ARGUMENTS_1_ATRIB_VALUE", "innogy Karta pro všechny zákazníky innogy, kteří mají rádi film, lyžování nebo cestování");
            textParameters.Add("ADD_SERVICES_SALES_ARGUMENTS_1_ATRIB_VALUE_1", "Získejte u vybraných partnerů slevu až 20 %");
            textParameters.Add("ADD_SERVICES_SALES_ARGUMENTS_ATRIB_VALUE", "Služba na míru pro zákazníky, kteří si nepřejí snižovat své zálohy na energie");
            textParameters.Add("NONCOMMODITY", "X");
            textParameters.Add("NONCOMMODITY_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">innogy Domácí asistence<br /></p></body>");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Služba");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_NAME_1", "Varianta");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_NAME_2", "Platnost nabídky do");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_VALUE", "innogy Domácí asistence");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1", "Premium");
            textParameters.Add("NONCOMMODITY_OFFER_SUMMARY_ATRIB_VALUE_2", "31.05.2022");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_NAME", "První");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_NAME_1", "Druhé");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_NAME_2", "Třetí");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_NAME_3", "Čtvrté");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_VALUE", "limit plnění až 10 000 Kč");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_1", "doprava hrazená v plné výši");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_2", "drobný spotřební materiál v ceně služby");
            textParameters.Add("NONCOMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_3", "non-stop telefonická podpora");
            textParameters.Add("COMMODITY", "X");
            textParameters.Add("COMMODITY_ACCEPT_LABEL", "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"margin-top:0pt;margin-bottom:0pt\">Smlouva / dodatek<br /></p></body>");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME", "Produkt");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_1", "Platnost nabídky do");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_2", "Délka fixace");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_3", "Platnost dodatku");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_4", "Předpokládaná účinnost dodatku");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE", "elektřina Start 15");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1", "16.05.2022");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_2", "15 měsíců od účinnosti dodatku");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_3", "Dnem akceptace dodatku zákazníkem");
            textParameters.Add("COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_4", "21.10.2022");
            textParameters.Add("COMMODITY_SALES_ARGUMENTS_ATRIB_NAME", "První");
            textParameters.Add("COMMODITY_SALES_ARGUMENTS_ATRIB_NAME_1", "Druhé");
            textParameters.Add("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE", "naše ceny energií i smlouvy jsou závazné");
            textParameters.Add("COMMODITY_SALES_ARGUMENTS_ATRIB_VALUE_1", "pevná cena silové elektřiny po dobu 15 měsíců");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);
            var result = service.GetAllSalesArguments(textParameters, false);

            Assert.Equal(4, result.Count());   
        }

        [Fact]
        public void GetAcceptance_Finds_ACCEPT_LABEL_by_ACCEPT_LABEL_GUID()
        {
            var expectedResult = "innogy Pojištění domácnosti";

            var offer = this.CreateOffer();
            offer.First().TextParameters.Add("NONCOMMODITY_ACCEPT_LABEL_GUID", "0635F899B3111EDD87A566959885DD77");
            offer.First().TextParameters.Add("NONCOMMODITY_ACCEPT_LABEL", expectedResult);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockDefinition = new Mock<IDefinitionCombinationModel>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetAcceptance(offer, mockDefinition.Object);

            Assert.Equal(expectedResult, result.Parameters.First().Title);
        }

        [Fact]
        public void GetAcceptance_Return_Empty_Parameters_When_ACCEPT_LABEL_not_found_by_ACCEPT_LABEL_GUID()
        {
            var expectedResult = "innogy Pojištění domácnosti";

            var offer = this.CreateOffer();
            offer.TextParameters.Add("NONCOMMODITY_ACCEPT_LABEL_GUID", "0635F899B3111EDD87A566959885DD77");
            offer.TextParameters.Add("COMMODITY_ACCEPT_LABEL", expectedResult);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockDefinition = new Mock<IDefinitionCombinationModel>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetAcceptance(offer, mockDefinition.Object);

            Assert.Empty(result.Parameters);
        }

        [Theory]
        [InlineData("COMMODITY_OFFER_SUMMARY_ATRIB_NAME",   "Produkt",             "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE",   "plyn Optimal")]
        [InlineData("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_1", "Platnost nabídky do", "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_1", "31.10.2022")]
        [InlineData("COMMODITY_OFFER_SUMMARY_ATRIB_NAME_2", "Délka fixace",        "COMMODITY_OFFER_SUMMARY_ATRIB_VALUE_2", "36 měsíců od účinnosti smlouvy")]
        public void GetEnumPairValue_Finds_Value(string keyName, string valueName, string keyValue, string expectedResult)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(keyName, valueName);
            textParameters.Add(keyValue, expectedResult);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetEnumPairValue(keyName, textParameters);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetTemplateHelp_Returns_String_Value()
        {
            var idattach = "EPO";
            var expected = "MY_HELP";
            var textParameters = new Dictionary<string, string>();
            textParameters.Add($"USER_ATTACH_{idattach}_HELP", expected);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetTemplateHelp(idattach, textParameters);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanDisplayPreviousPrice_Returns_True()
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("DOUBLE_1", "15.3");
            textParameters.Add("DOUBLE_2", "15.6");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.CanDisplayPreviousPrice(textParameters, "DOUBLE_1", "DOUBLE_2");

            Assert.True(result);
        }

        [Fact]
        public void CanDisplayPreviousPrice_Returns_False()
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("DOUBLE_1", "17.1");
            textParameters.Add("DOUBLE_2", "17.1");

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.CanDisplayPreviousPrice(textParameters, "DOUBLE_1", "DOUBLE_2");

            Assert.False(result);
        }

        [Fact]
        public void UpdateProductInfo_Do_Nothing_When_Files_Empty()
        {
            var files = new List<JsonAcceptFileModel>();

            var productInfos = new List<IProductInfoModel>();
            var pi = new Mock<IProductInfoModel>();
            pi.SetupGet(x => x.Key).Returns("GROUP_1");
            pi.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());
        }

        [Fact]
        public void UpdateProductInfo_Do_Nothing_When_Files_Null()
        {
            IEnumerable< JsonAcceptFileModel> files = null;

            var productInfos = new List<IProductInfoModel>();
            var pi = new Mock<IProductInfoModel>();
            pi.SetupGet(x => x.Key).Returns("GROUP_1");
            pi.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());
        }

        [Fact]
        public void UpdateProductInfo_Updates_1_File_In_1_Group()
        {
            var files = new List<JsonAcceptFileModel>();
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 1" })));
            
            var productInfos = new List<IProductInfoModel>();
            
            var piXmlAttributes = new NameValueCollection();
            piXmlAttributes.Add("GROUP", "GROUP_1");
            var pi = new Mock<IProductInfoModel>();
            pi.SetupGet(x => x.XmlAttributes).Returns(piXmlAttributes);
            pi.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());

            Assert.Equal("GROUP_1_NOTE", files[0].Note);
        }

        [Fact]
        public void UpdateProductInfo_Updates_Last_File_In_1_Group()
        {
            var files = new List<JsonAcceptFileModel>();
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 1" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 2" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 3" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 4" })));

            var productInfos = new List<IProductInfoModel>();
            var piXmlAttributes = new NameValueCollection();
            piXmlAttributes.Add("GROUP", "GROUP_1");
            var pi = new Mock<IProductInfoModel>();
            pi.SetupGet(x => x.XmlAttributes).Returns(piXmlAttributes);
            pi.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());

            Assert.True(string.IsNullOrEmpty(files[0].Note));
            Assert.True(string.IsNullOrEmpty(files[1].Note));
            Assert.True(string.IsNullOrEmpty(files[2].Note));
            Assert.Equal("GROUP_1_NOTE", files[3].Note);
        }

        [Fact]
        public void UpdateProductInfo_Updates_Last_File_In_2_Groups_With_More_Files()
        {
            var files = new List<JsonAcceptFileModel>();
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 1" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 2" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 3" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 4" })));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("GROUP", "GROUP_1");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi1.Object);

            var pi2xmlAttributes = new NameValueCollection();
            pi2xmlAttributes.Add("GROUP", "GROUP_2");
            var pi2 = new Mock<IProductInfoModel>();
            pi2.SetupGet(x => x.XmlAttributes).Returns(pi2xmlAttributes);
            pi2.SetupGet(x => x.Note).Returns("GROUP_2_NOTE");
            productInfos.Add(pi2.Object);

            var pi3xmlAttributes = new NameValueCollection();
            pi3xmlAttributes.Add("GROUP", "GROUP_3");
            var pi3 = new Mock<IProductInfoModel>();
            pi3.SetupGet(x => x.XmlAttributes).Returns(pi3xmlAttributes);
            pi3.SetupGet(x => x.Note).Returns("GROUP_3_NOTE");
            productInfos.Add(pi3.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());

            Assert.True(string.IsNullOrEmpty(files[0].Note));
            Assert.Equal("GROUP_1_NOTE", files[1].Note);
            Assert.True(string.IsNullOrEmpty(files[2].Note));
            Assert.Equal("GROUP_2_NOTE", files[3].Note);
        }

        [Fact]
        public void UpdateProductInfo_Updates_Last_File_In_Groups_With_1_File()
        {
            var files = new List<JsonAcceptFileModel>();
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 1" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 2" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_3", Description = "File 3" })));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("GROUP", "GROUP_1");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi1.Object);

            var pi2xmlAttributes = new NameValueCollection();
            pi2xmlAttributes.Add("GROUP", "GROUP_2");
            var pi2 = new Mock<IProductInfoModel>();
            pi2.SetupGet(x => x.XmlAttributes).Returns(pi2xmlAttributes);
            pi2.SetupGet(x => x.Note).Returns("GROUP_2_NOTE");
            productInfos.Add(pi2.Object);

            var pi3xmlAttributes = new NameValueCollection();
            pi3xmlAttributes.Add("GROUP", "GROUP_3");
            var pi3 = new Mock<IProductInfoModel>();
            pi3.SetupGet(x => x.XmlAttributes).Returns(pi3xmlAttributes);
            pi3.SetupGet(x => x.Note).Returns("GROUP_3_NOTE");
            productInfos.Add(pi3.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());

            Assert.Equal("GROUP_1_NOTE", files[0].Note);
            Assert.Equal("GROUP_2_NOTE", files[1].Note);
            Assert.Equal("GROUP_3_NOTE", files[2].Note);
        }

        [Fact]
        public void UpdateProductInfo_Updates_Last_File_In_Mixed_Same_Groups()
        {
            var files = new List<JsonAcceptFileModel>();
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 1" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 2" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 3" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 4" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 5" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_3", Description = "File 6" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_2", Description = "File 7" })));
            files.Add(new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Group = "GROUP_1", Description = "File 8" })));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("GROUP", "GROUP_1");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("GROUP_1_NOTE");
            productInfos.Add(pi1.Object);

            var pi2xmlAttributes = new NameValueCollection();
            pi2xmlAttributes.Add("GROUP", "GROUP_2");
            var pi2 = new Mock<IProductInfoModel>();
            pi2.SetupGet(x => x.XmlAttributes).Returns(pi2xmlAttributes);
            pi2.SetupGet(x => x.Note).Returns("GROUP_2_NOTE");
            productInfos.Add(pi2.Object);

            var pi3xmlAttributes = new NameValueCollection();
            pi3xmlAttributes.Add("GROUP", "GROUP_3");
            var pi3 = new Mock<IProductInfoModel>();
            pi3.SetupGet(x => x.XmlAttributes).Returns(pi3xmlAttributes);
            pi3.SetupGet(x => x.Note).Returns("GROUP_3_NOTE");
            productInfos.Add(pi3.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            service.UpdateProductInfo(files, productInfos.ToArray());

            Assert.Equal("GROUP_1_NOTE", files[0].Note);
            Assert.Equal("GROUP_2_NOTE", files[1].Note);
            Assert.Equal("GROUP_1_NOTE", files[2].Note);
            Assert.True(string.IsNullOrEmpty(files[3].Note));
            Assert.Equal("GROUP_2_NOTE", files[4].Note);
            Assert.Equal("GROUP_3_NOTE", files[5].Note);
            Assert.Equal("GROUP_2_NOTE", files[6].Note);
            Assert.Equal("GROUP_1_NOTE", files[7].Note);
        }

        [Fact]
        public void GetMatchedProductInfo_Finds_Match_With_1_Attribute()
        {
            var file = new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Product = "G_START12", Description = "File 1" }));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("PRODUCT", "G_START12");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("G_START12 NOTE");
            productInfos.Add(pi1.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMatchedProductInfo(file, productInfos.ToArray());

            Assert.Same(pi1.Object, result);
        }

        [Fact]
        public void GetMatchedProductInfo_Finds_Match_With_More_Attribute()
        {
            var file = new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Product = "G_START12", IdAttach = "EPO", Printed = "X", Description = "File 1" }));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("PRODUCT", "G_START12");
            pi1xmlAttributes.Add("IDATTACH", "EPO");
            pi1xmlAttributes.Add("PRINTED", "X");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("G_START12 NOTE");
            productInfos.Add(pi1.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMatchedProductInfo(file, productInfos.ToArray());

            Assert.Same(pi1.Object, result);
        }

        [Fact]
        public void GetMatchedProductInfo_Finds_Multiple_Matches_And_Selects_With_More_Attributes()
        {
            var file = new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Product = "G_START12", IdAttach = "EPO", Printed = "X", Description = "File 1" }));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("PRODUCT", "G_START12");
            pi1xmlAttributes.Add("IDATTACH", "EPO");
            pi1xmlAttributes.Add("PRINTED", "X");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("G_START12 NOTE");
            productInfos.Add(pi1.Object);

            var pi2xmlAttributes = new NameValueCollection();
            pi2xmlAttributes.Add("PRODUCT", "G_START12");
            pi2xmlAttributes.Add("IDATTACH", "EPO");
            var pi2 = new Mock<IProductInfoModel>();
            pi2.SetupGet(x => x.XmlAttributes).Returns(pi2xmlAttributes);
            pi2.SetupGet(x => x.Note).Returns("G_START12 NOTE");
            productInfos.Add(pi2.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMatchedProductInfo(file, productInfos.ToArray());

            Assert.Same(pi1.Object, result);
        }

        [Fact]
        public void GetMatchedProductInfo_Will_Not_Find_Match()
        {
            var file = new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Product = "G_START12", Description = "File 1" }));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("PRODUCT", "NESNIZOVANI_ZALOH");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("NESNIZOVANI_ZALOH NOTE");
            productInfos.Add(pi1.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMatchedProductInfo(file, productInfos.ToArray());

            Assert.Null(result);
        }

        [Fact]
        public void GetMatchedProductInfo_Converts_Value_Dash_To_Empty_String_As_Defined_In_Xml_And_Finds_Match()
        {
            var file = new JsonAcceptFileModel(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Product = "G_START12", SignReq = "", Description = "File 1" }));

            var productInfos = new List<IProductInfoModel>();

            var pi1xmlAttributes = new NameValueCollection();
            pi1xmlAttributes.Add("PRODUCT", "G_START12");
            pi1xmlAttributes.Add("SIGN_REQ", "-");
            var pi1 = new Mock<IProductInfoModel>();
            pi1.SetupGet(x => x.XmlAttributes).Returns(pi1xmlAttributes);
            pi1.SetupGet(x => x.Note).Returns("G_START12 NOTE");
            productInfos.Add(pi1.Object);

            var logger = new MemoryLogger();
            var textService = new MemoryTextService();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMatchedProductInfo(file, productInfos.ToArray());

            Assert.Same(pi1.Object, result);
        }

        [Fact]
        public void GetProductInfos_Get_Only_For_CALC_COMP_GAS()
        {
            var offer = this.CreateOffer(3);
            offer.First().TextParameters.Add("CALC_COMP_GAS", "2 500,00");
            offer.First().TextParameters.Add("CALC_COMP_GAS_DESCRIPTION", "Odebraný plyn");
            offer.First().TextParameters.Add("CALC_COMP_GAS_DISPLAY_UNIT", "Kč/MWh");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE", "2 555,00");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE_DESCRIPTION", "Odebraný plyn");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE_DISPLAY_UNIT", "Kč/MWh");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE_VISIBILITY", "S");

            var mockTextService = new Mock<ITextService>();
            mockTextService.Setup(x => x.FindByKey("CONSUMED_GAS")).Returns("Plyn");

            var logger = new MemoryLogger();
            var textService = mockTextService.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductInfos(offer);

            Assert.Single(result);
            Assert.Equal("2 500,00", result[0].Price);
            Assert.Equal("Kč/MWh", result[0].PriceUnit);
            Assert.True(string.IsNullOrEmpty(result[0].PreviousPrice));
        }

        [Fact]
        public void GetProductInfos_Get_For_CALC_COMP_GAS_and_CALC_COMP_GAS_PRICE()
        {
            var offer = this.CreateOffer(3);
            offer.First().TextParameters.Add("CALC_COMP_GAS", "2 500,00");
            offer.First().TextParameters.Add("CALC_COMP_GAS_DESCRIPTION", "Odebraný plyn");
            offer.First().TextParameters.Add("CALC_COMP_GAS_DISPLAY_UNIT", "Kč/MWh");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE", "2 555,00");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE_DESCRIPTION", "Odebraný plyn");
            offer.First().TextParameters.Add("CALC_COMP_GAS_PRICE_DISPLAY_UNIT", "Kč/MWh");
            //offer.TextParameters.Add("CALC_COMP_GAS_PRICE_VISIBILITY", "S");

            var mockTextService = new Mock<ITextService>();
            mockTextService.Setup(x => x.FindByKey("CONSUMED_GAS")).Returns("Plyn");

            var logger = new MemoryLogger();
            var textService = mockTextService.Object;
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductInfos(offer);

            Assert.Single(result);
            Assert.Equal("2 500,00", result[0].Price);
            Assert.Equal("Kč/MWh", result[0].PriceUnit);
            Assert.Equal("2 555,00 Kč/MWh", result[0].PreviousPrice);
        }

        [Fact]
        public void GetProductNote_Returns_InfoGas_Value_When_G()
        {
            var expected = "GAS_NOTE";

            var mockProductInfo = new Mock<IProductInfoRootModel>();
            mockProductInfo.SetupGet(x => x.InfoGas).Returns(expected);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns(mockProductInfo.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote("G");

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetProductNote_Returns_Null_For_G_When_InfoGas_Empty(string infoGasValue)
        {
            var mockProductInfo = new Mock<IProductInfoRootModel>();
            mockProductInfo.SetupGet(x => x.InfoGas).Returns(infoGasValue);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns(mockProductInfo.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote("G");

            Assert.Null(result);
        }

        [Fact]
        public void GetProductNote_Returns_InfoElectricity_Value_When_E()
        {
            var expected = "ELECTRICITY_NOTE";

            var mockProductInfo = new Mock<IProductInfoRootModel>();
            mockProductInfo.SetupGet(x => x.InfoElectricity).Returns(expected);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns(mockProductInfo.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote("E");

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetProductNote_Returns_Null_For_G_When_InfoElectricity_Empty(string infoElectricityValue)
        {
            var mockProductInfo = new Mock<IProductInfoRootModel>();
            mockProductInfo.SetupGet(x => x.InfoElectricity).Returns(infoElectricityValue);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns(mockProductInfo.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote("E");

            Assert.Null(result);
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetProductNote_Returns_Null_For_G_When_Type_Empty(string typeValue)
        {
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote(typeValue);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("G")]
        [InlineData("E")]
        public void GetProductNote_Returns_Null_When_Root_Item_Does_Not_Exist(string typeValue)
        {
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns((IProductInfoRootModel)null);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote(typeValue);

            Assert.Null(result);
        }

        [Fact]
        public void GetProductNote_Returns_Null_When_Type_Invalid()
        {
            var invalidValue = "X";
            var mockProductInfo = new Mock<IProductInfoRootModel>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IProductInfoRootModel>(Constants.SitecorePaths.PRODUCT_INFOS)).Returns(mockProductInfo.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductNote(invalidValue);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("CALC_COMP_GAS")]
        [InlineData("CALC_CAP_PRICE")]
        [InlineData("CALC_COMP_FIX")]
        public void GetProductType_Returns_G(string xmlAttribute)
        {
            var offer = this.CreateOffer(3);
            offer.First().Xml.Content.Body = new OfferBodyXmlModel();
            offer.First().Xml.Content.Body.EanOrAndEic = "";
            //offer.TextParameters.Add(xmlAttribute + "_VISIBILITY", "S");
            offer.First().TextParameters.Add(xmlAttribute, "100");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductType(offer);

            Assert.Equal("G", result);
        }

        [Theory]
        [InlineData("CALC_COMP_VT")]
        [InlineData("CALC_COMP_NT")]
        [InlineData("CALC_COMP_KC")]
        public void GetProductType_Returns_E(string xmlAttribute)
        {
            var offer = this.CreateOffer(3);
            offer.First().Xml.Content.Body = new OfferBodyXmlModel();
            offer.First().Xml.Content.Body.EanOrAndEic = "";
            //offer.TextParameters.Add(xmlAttribute + "_VISIBILITY", "S");
            offer.First().TextParameters.Add(xmlAttribute, "100");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductType(offer);

            Assert.Equal("E", result);
        }

        [Theory]
        [InlineData("859182400506916978")]
        [InlineData("859182400891447477")]
        [InlineData("859182400606500558")]
        [InlineData("859182400212202808")]
        public void GetProductType_Returns_E_For_EXT_UI(string ean)
        {
            var offer = this.CreateOffer(3);
            offer.First().Xml.Content.Body = new OfferBodyXmlModel();
            offer.First().Xml.Content.Body.EanOrAndEic = ean; //27ZG500Z0253419Q
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductType(offer);

            Assert.Equal("E", result);
        }

        [Theory]
        [InlineData("27ZG700Z0652990R")]
        [InlineData("27ZG700Z0395472V")]
        [InlineData("27ZG700Z0118223P")]
        [InlineData("27ZG600Z07264929")]
        public void GetProductType_Returns_G_For_EXT_UI(string eic)
        {
            var offer = this.CreateOffer(3);
            offer.First().Xml.Content.Body = new OfferBodyXmlModel();
            offer.First().Xml.Content.Body.EanOrAndEic = eic;
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductType(offer);

            Assert.Equal("G", result);
        }
        
        [Fact]
        public void IsVisible_Returns_True_When_Key_Missing()
        {
            var textParameters = new Dictionary<string, string>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.IsVisible(textParameters, "CALC_COMP_FIX_PRICE");

            Assert.True(result);
        }

        [Theory]
        [InlineData("CALC_COMP_FIX_PRICE")]
        [InlineData("CALC_COMP_GAS_PRICE")]
        [InlineData("CALC_FIN_REW")]
        [InlineData("CALC_TOTAL_SAVE")]
        public void IsVisible_Returns_True_When_Value_Is_Not_S(string key)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key + "_VISIBILITY", "");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.IsVisible(textParameters, key);

            Assert.True(result);
        }

        [Theory]
        [InlineData("CALC_COMP_FIX_PRICE")]
        [InlineData("CALC_COMP_GAS_PRICE")]
        [InlineData("CALC_FIN_REW")]
        [InlineData("CALC_TOTAL_SAVE")]
        public void IsVisible_Returns_False_When_Value_Is_S(string key)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key + "_VISIBILITY", Constants.HIDDEN);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.IsVisible(textParameters, key);

            Assert.False(result);
        }

        [Fact]
        public void HasValue_Returns_False_When_Key_Missing()
        {
            var textParameters = new Dictionary<string, string>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.HasValue(textParameters, "CALC_COMP_FIX_PRICE", 0.1);

            Assert.False(result);
        }

        [Fact]
        public void HasValue_Returns_False_When_Its_Not_Double()
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("PRICE", "10e");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.HasValue(textParameters, "PRICE", 0.1);

            Assert.False(result);
        }

        [Theory]
        [InlineData("0,09", 0.1)]
        [InlineData("0.09", 0.1)]
        public void HasValue_Returns_False_When_Value_Is_Less_Than_Minimum(string value, double minValue)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("PRICE", value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.HasValue(textParameters, "PRICE", minValue);

            Assert.False(result);
        }

        [Theory]
        [InlineData("0,1", 0.1)]
        [InlineData("0.1", 0.1)]
        [InlineData("1", 1.0)]
        [InlineData("1.000001", 0.1)]
        public void HasValue_Returns_True_When_Value_Match_Minimum_Value(string value, double minValue)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add("PRICE", value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.HasValue(textParameters, "PRICE", minValue);

            Assert.True(result);
        }

        [Theory]
        [InlineData("CALC_TOTAL_SAVE", "0.1")]
        [InlineData("CALC_TOTAL_SAVE", "0,2")]
        [InlineData("CALC_TOTAL_SAVE", "10")]
        [InlineData("CALC_TOTAL_SAVE", "0.1000001")]
        public void CanDisplayPrice_Returns_True(string key, string value)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key, value);
            textParameters.Add(key + "_VISIBILITY", "");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.CanDisplayPrice(textParameters, key);

            Assert.True(result);
        }

        [Theory]
        [InlineData("CALC_COMP_FIX_PRICE")]
        [InlineData("CALC_COMP_GAS_PRICE")]
        [InlineData("CALC_FIN_REW")]
        [InlineData("CALC_TOTAL_SAVE")]
        public void CanDisplayPrice_Returns_False_When_Not_Visible(string key)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key + "_VISIBILITY", "S");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.CanDisplayPrice(textParameters, key);

            Assert.False(result);
        }

        [Theory]
        [InlineData("CALC_TOTAL_SAVE", "")]
        [InlineData("CALC_TOTAL_SAVE", "a")]
        [InlineData("CALC_TOTAL_SAVE", "0.09")]
        [InlineData("CALC_TOTAL_SAVE", "0,09")]
        [InlineData("CALC_TOTAL_SAVE", " ")]
        public void CanDisplayPrice_Returns_False_When_Has_Not_Correct_Value(string key, string value)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key, value);
            textParameters.Add(key + "_VISIBILITY", "");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.CanDisplayPrice(textParameters, key);

            Assert.False(result);
        }

        [Fact]
        public void GetBenefits_Returns_Empty_Array_When_Empty_Text_Parameters()
        {
            var offer = this.CreateOffer();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetBenefits(offer);

            Assert.Empty(result);
        }

        [Fact]
        public void GetBenefits_Returns_Empty_Array_When_Offer_Null()
        {
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetBenefits((OffersModel)null);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("SA01_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA02_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA03_MIDDLE_TEXT", "sadfdsaf")]
        public void GetBenefits_Returns_One_Value(string key, string value)
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add(key, value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetBenefits(offer);

            Assert.Contains(value, result);
        }

        [Theory]
        [InlineData("SA00_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA04_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA05_MIDDLE_TEXT", "sadfdsaf")]
        public void GetBenefits_Returns_Empty_Array_When_Keys_Not_Match(string key, string value)
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add(key, value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetBenefits(offer);

            Assert.Empty(result);
        }

        [Fact]
        public void GetDistributorChange_Returns_Null_When_Key_Missing()
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add("BLABLA", "EON");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetDistributorChange(offer);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("00")]
        [InlineData("02")]
        [InlineData("03")]
        [InlineData("04")]
        [InlineData("05")]
        [InlineData("06")]
        [InlineData("07")]
        [InlineData("08")]
        public void GetDistributorChange_Returns_Null_When_Process_Not_Equals_01(string process)
        {
            var offer = this.CreateOffer();
            offer.First().Xml.Content.Body.BusProcess = process;
            offer.First().TextParameters.Add("PERSON_COMPETITOR_NAME", "EON");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetDistributorChange(offer);

            Assert.Null(result);
        }

        [Fact]
        public void GetDistributorChange_Returns_Data()
        {
            var title = "Předchozí distributor";
            var description = "Description";
            var distributor = "ECON";
            var offer = this.CreateOffer();
            offer.First().Xml.Content.Body.BusProcess = "01";
            offer.First().TextParameters.Add("PERSON_COMPETITOR_NAME", distributor);
            var targetIdGuid = Guid.NewGuid();
            var logger = new MemoryLogger();
            var mockSiteSettings = new Mock<ISiteSettingsModel>();
            mockSiteSettings.SetupGet(x => x.Summary).Returns(new Glass.Mapper.Sc.Fields.Link() { TargetId = targetIdGuid });
            var mockPageSummaryOfferModel = new Mock<IPageSummaryOfferModel>();
            mockPageSummaryOfferModel.SetupGet(x => x.DistributorChange_Title).Returns(title);
            mockPageSummaryOfferModel.SetupGet(x => x.DistributorChange_Text).Returns(description);
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            mockSitecoreService.Setup(x => x.GetItem<IPageSummaryOfferModel>(targetIdGuid)).Returns(mockPageSummaryOfferModel.Object);
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            mockSettingsReaderService.Setup(x => x.GetSiteSettings()).Returns(mockSiteSettings.Object);

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetDistributorChange(offer);

            Assert.NotNull(result);

            Assert.Equal(title, result.Title);
            Assert.Equal(description, result.Description);
            Assert.Equal(distributor, result.Name);
        }

        [Fact]
        public void GetMiddleTexts_Returns_Empty_Array_When_Empty_Text_Parameters()
        {
            var offer = this.CreateOffer();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts(offer);

            Assert.Empty(result);
        }

        [Fact]
        public void GetMiddleTexts_Returns_Empty_Array_When_Offer_Null()
        {
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts((OffersModel)null);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("SA04_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA05_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA06_MIDDLE_TEXT", "sadfdsaf")]
        public void GetMiddleTexts_Returns_One_Value(string key, string value)
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add(key, value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts(offer);

            Assert.Contains(value, result);
        }

        [Theory]
        [InlineData("SA00_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA01_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA02_MIDDLE_TEXT", "sadfdsaf")]
        [InlineData("SA03_MIDDLE_TEXT", "sadfdsaf")]
        public void GetMiddleTexts_Returns_Empty_Array_When_Keys_Not_Match(string key, string value)
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add(key, value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts(offer);

            Assert.Empty(result);
        }

        [Fact]
        public void GetMiddleTexts_Returns_SA06_MIDDLE_TEXT_Only_When_Other_Missing()
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add("SA06_MIDDLE_TEXT", "text");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts(offer);

            Assert.Single(result);
        }

        [Fact]
        public void GetMiddleTexts_Do_Not_Returns_SA06_MIDDLE_TEXT_Only_When_Other_Exists()
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters.Add("SA06_MIDDLE_TEXT", "text");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetMiddleTexts(offer);

            Assert.Single(result);
            Assert.DoesNotContain("SA06_MIDDLE_TEXT", result);
        }

        [Fact]
        public void GetUploads_Returns_Null_When_No_Templates_With_IsPrinted_Equals_False()
        {
            var offer = this.CreateOffer();
            var attachments = new List<OfferAttachmentModel>();
            attachments.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Printed = "X" }, "application/pdf", "File.pdf", new OfferAttributeModel[] {}, new byte[] { }));
            var mockDefinitionCombinationModel = new Mock<IDefinitionCombinationModel>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetUploads(offer, attachments.ToArray(), mockDefinitionCombinationModel.Object);

            Assert.Null(result);
        }

        [Fact]
        public void GetUploads_Returns_Not_Null_With_File()
        {
            var offer = this.CreateOffer();
            var attachments = new List<OfferAttachmentModel>();
            attachments.Add(new OfferAttachmentModel(new OfferAttachmentXmlModel() { Printed = "", IdAttach = "XXX" }, "application/pdf", "File.pdf", new OfferAttributeModel[] { }, new byte[] { }));
            var mockDefinitionCombinationModel = new Mock<IDefinitionCombinationModel>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetUploads(offer, attachments.ToArray(), mockDefinitionCombinationModel.Object);

            Assert.NotNull(result);
            Assert.Single(result.Types);
        }

        [Fact]
        public void GetUploads_Returns_File_As_Mandatory()
        {
            var offer = this.CreateOffer();
            var attachments = new List<OfferAttachmentModel>();
            attachments.Add(new OfferAttachmentModel(
                new OfferAttachmentXmlModel() { Printed = "", IdAttach = "XXX", Obligatory = Constants.FileAttributeValues.CHECK_VALUE },
                "application/pdf",
                "File.pdf",
                new OfferAttributeModel[] { },
                new byte[] { }));
            var mockDefinitionCombinationModel = new Mock<IDefinitionCombinationModel>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetUploads(offer, attachments.ToArray(), mockDefinitionCombinationModel.Object);

            Assert.NotNull(result);
            Assert.Single(result.Types);
        }

        [Fact]
        public void GetProductData_Returns_Null_When_ShowPrices_Equals_False()
        {
            var attributes = new List<OfferAttributeModel>();
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.NO_PROD_CHNG, Constants.CHECKED));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "20500101", attributes.ToArray());
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductData(offer);

            Assert.Null(result);
        }

        [Fact]
        public void GetProductData_Returns_Null_When_ProductInfos_And_MiddleTexts_And_Benefits_Are_Empty()
        {
            var attributes = new List<OfferAttributeModel>();
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "20500101", attributes.ToArray());
            offer.First().TextParameters.Add("CALC_TOTAL_SAVE", "1");
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductData(offer);

            Assert.Null(result);
        }

        [Fact]
        public void GetProductData_Returns_Model_With_MiddleTextsHelp()
        {
            var expected = "middle text";
            var attributes = new List<OfferAttributeModel>();
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "20500101", attributes.ToArray());
            offer.First().TextParameters.Add("CALC_TOTAL_SAVE", "1");
            offer.First().TextParameters.Add("SA04_MIDDLE_TEXT", "text");
            offer.First().TextParameters.Add("SA06_MIDDLE_TEXT", expected);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(logger, textService.Object, mockSitecoreService.Object, mockOfferService.Object, mockSettingsReaderService.Object);

            var result = service.GetProductData(offer);

            Assert.Equal(expected, result.MiddleTextsHelp);
        }

        [Theory]
        [InlineData("PERSON")]
        [InlineData("BENEFITS")]
        public void IsSectionChecked_Returns_True_If_Value_Is_X(string key)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key, Constants.FileAttributeValues.CHECK_VALUE);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(
                logger,
                textService.Object,
                mockSitecoreService.Object,
                mockOfferService.Object,
                mockSettingsReaderService.Object);

            var result = service.IsSectionChecked(textParameters, key);
        }

        [Fact]
        public void IsSectionChecked_Returns_False_When_Key_Doesnt_Exist()
        {
            var textParameters = new Dictionary<string, string>();
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(
                logger,
                textService.Object,
                mockSitecoreService.Object,
                mockOfferService.Object,
                mockSettingsReaderService.Object);

            var result = service.IsSectionChecked(textParameters, "DUMB_KEY");
        }

        [Theory]
        [InlineData("PERSON", "")]
        [InlineData("PERSON", " ")]
        [InlineData("PERSON", "S")]
        public void IsSectionChecked_Returns_False_When_Value_Is_Not_X(string key, string value)
        {
            var textParameters = new Dictionary<string, string>();
            textParameters.Add(key, value);
            var logger = new MemoryLogger();
            var textService = new Mock<ITextService>();
            var mockSitecoreService = new Mock<ISitecoreServiceExtended>();
            var mockOfferService = new Mock<IOfferService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();

            var service = new OfferJsonDescriptor(
                logger,
                textService.Object,
                mockSitecoreService.Object,
                mockOfferService.Object,
                mockSettingsReaderService.Object);

            var result = service.IsSectionChecked(textParameters, key);
        }
    }
}
