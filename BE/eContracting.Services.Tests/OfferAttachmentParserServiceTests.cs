using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using Xunit;

namespace eContracting.Services.Tests
{
    public class OfferAttachmentParserServiceTests : BaseTestClass
    {
        [Fact]
        public void Parse_Returns_Empty_When_No_Offer_Attachments()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);// new OfferModel(offerXmlModel, 2, offerHeader, new OfferAttributeModel[] { });
            offer.Xml.Content.Body.BusProcess = "XYZ";
            offer.Xml.Content.Body.BusProcessType = "123";

            var files = new List<ZCCH_ST_FILE>();

            var service = new OfferAttachmentParserService(logger);
            var result = service.Parse(offer, files.ToArray());

            Assert.Empty(result);

            var log = logger.Warns.First();
            Assert.Equal(guid, log.Key);
        }

        //TODO: TBI
        public void Parse_Return_Only_Files_Matching_Offer_Documents()
        {

        }

        //TODO: TBI
        public void GetModel_Finds_Printed_File_Matching_To_Template()
        {

        }

        //TODO: TBI
        public void GetModel_Return_Only_File_Template_When_No_Real_File_Exists()
        {

        }

        [Theory]
        [InlineData("ST_FILE_FROM_CACHE.pdf", "My labeled file name", "My labeled file name.pdf")]
        public void GetFileName_Returns_Readable_Label_From_ZCCH_ST_FILE_File(string realFileName, string labeledFileName, string expected)
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.FileAttributes.LABEL;
            attr.ATTRVAL = labeledFileName;
            
            var file = new ZCCH_ST_FILE();
            file.FILENAME = realFileName;
            file.ATTRIB = new[] { attr };

            var logger = new MemoryLogger();
            var service = new OfferAttachmentParserService(logger);

            var result = service.GetFileName(file);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeCompatible_Sets_Default_Group_When_Empty()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new DocumentTemplateModel();
            template.Description = "Obchodní podmínky";
            template.Group = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 1);

            Assert.Equal(Constants.OfferDefaults.GROUP, template.Group);
        }

        [Fact]
        public void MakeCompatible_Sets_Printed_To_Templates_In_Version_1()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new DocumentTemplateModel();
            template.Description = "Obchodní podmínky";
            template.Printed = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 1);

            Assert.Equal(Constants.FileAttributeValues.CHECK_VALUE, template.Printed);
        }

        [Fact]
        public void MakeCompatible_Sets_Missing_Consent_Type_To_S_For_Sign_File_In_Version_1()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new DocumentTemplateModel();
            template.Description = "Obchodní podmínky";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;
            template.ConsentType = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 1);

            Assert.Equal(Constants.FileAttributeValues.CONSENT_TYPE_S, template.ConsentType);
        }

        [Fact]
        public void MakeCompatible_Sets_Missing_Consent_Type_To_S_When_Index_Equals_0()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new DocumentTemplateModel();
            template.Description = "Obchodní podmínky";
            template.ConsentType = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 0);

            Assert.Equal(Constants.FileAttributeValues.CONSENT_TYPE_S, template.ConsentType);
        }

        [Fact]
        public void MakeCompatible_Sets_Missing_Consent_Type_To_S_When_Index_More_Than_0()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new DocumentTemplateModel();
            template.Description = "Obchodní podmínky";
            template.ConsentType = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 2);

            Assert.Equal(Constants.FileAttributeValues.CONSENT_TYPE_P, template.ConsentType);
        }

        [Fact]
        public void MakeCompatible_Sets_Correct_Idattach_Value_To_Template_When_Sign_File_In_Version_1()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");

            var template = new DocumentTemplateModel();
            template.Description = "Plná moc";
            template.IdAttach = "A10";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;

            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Attachments = new[] { template };
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, "3", "04.01.2021");
            var offer = new OfferModel(offerXml, 1, offerHeader, false, new OfferAttributeModel[] { });

            var attr1 = new ZCCH_ST_ATTRIB();
            attr1.ATTRID = Constants.FileAttributes.TYPE;
            attr1.ATTRVAL = "ELH"; // expected IDATTACH in a template

            var attr2 = new ZCCH_ST_ATTRIB();
            attr2.ATTRID = Constants.FileAttributes.TEMPLATE;
            attr2.ATTRVAL = "A10";

            var file = new ZCCH_ST_FILE();
            file.FILENAME = "Plná moc.pdf";
            file.ATTRIB = new[] { attr1, attr2 };

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, new[] { file });

            var changedTemplate = offer.Documents.First();

            Assert.Equal(attr1.ATTRVAL, changedTemplate.IdAttach);
        }
    }
}
