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

        [Fact]
        public void GetFileByTemplate_Returns_First_Match()
        {
            var iddattach = "X1";

            var logger = new MemoryLogger();

            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.FileAttributes.TYPE;
            attr.ATTRVAL = iddattach;

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var template = new OfferAttachmentXmlModel();
            template.IdAttach = iddattach;

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetFileByTemplate(template, new[] { file });

            Assert.NotNull(result);
        }

        [Fact]
        public void GetFileByTemplate_Returns_Null_When_Not_Match()
        {
            var logger = new MemoryLogger();

            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.FileAttributes.TYPE;
            attr.ATTRVAL = "AAA";

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var template = new OfferAttachmentXmlModel();
            template.IdAttach = "BBB";

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetFileByTemplate(template, new[] { file });

            Assert.Null(result);
        }

        [Theory]
        [InlineData("ST_FILE_FROM_CACHE.pdf", "My labeled file name", "My labeled file name.pdf")]
        public void GetFileName_Returns_Readable_Label_From_ZCCH_ST_FILE_File(string realFileName, string labeledFileName, string expected)
        {
            var logger = new MemoryLogger();

            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.FileAttributes.LABEL;
            attr.ATTRVAL = labeledFileName;
            
            var file = new ZCCH_ST_FILE();
            file.FILENAME = realFileName;
            file.ATTRIB = new[] { attr };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetFileName(file);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetIdAttach_Returns_Value_When_Found()
        {
            var idattach = "V02";
            var logger = new MemoryLogger();

            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.FileAttributes.TYPE;
            attr.ATTRVAL = idattach;

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetIdAttach(file);

            Assert.Equal(idattach, attr.ATTRVAL);
        }

        [Fact]
        public void GetIdAttach_Returns_Null_When_Not_Found()
        {
            var logger = new MemoryLogger();

            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = "whatever";
            attr.ATTRVAL = "V03";

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetIdAttach(file);

            Assert.Null(result);
        }

        [Fact]
        public void Equals_Returns_False_When_Template_Idattach_Missing()
        {
            var logger = new MemoryLogger();
            var template = new OfferAttachmentXmlModel();
            template.IdAttach = null;
            var file = new ZCCH_ST_FILE();

            var service = new OfferAttachmentParserService(logger);
            var result = service.Equals(template, file);

            Assert.False(result);
        }

        [Fact]
        public void Equals_Returns_False_If_Idattach_Not_Matches()
        {
            var logger = new MemoryLogger();
            var template = new OfferAttachmentXmlModel();
            template.IdAttach = "ID1";
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = "WRONG" } };

            var service = new OfferAttachmentParserService(logger);
            var result = service.Equals(template, file);

            Assert.False(result);
        }

        [Fact]
        public void Equals_Returns_True_If_Idattach_Matches()
        {
            var idattach = "A10";

            var logger = new MemoryLogger();
            var template = new OfferAttachmentXmlModel();
            template.IdAttach = idattach;
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = idattach } };

            var service = new OfferAttachmentParserService(logger);
            var result = service.Equals(template, file);

            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void MakeCompatible_Sets_Default_Group_When_Empty_For_Any_Version(int version)
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, version);

            var template = new OfferAttachmentXmlModel();
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

            var template = new OfferAttachmentXmlModel();
            template.Description = "Obchodní podmínky";
            template.Printed = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 0);

            Assert.Equal(Constants.FileAttributeValues.CHECK_VALUE, template.Printed);
            Assert.True(template.IsPrinted());
        }

        [Fact]
        public void MakeCompatible_Doesnt_Set_Printed_To_Templates_In_Version_2()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 2);

            var template = new OfferAttachmentXmlModel();
            template.Description = "Obchodní podmínky";
            template.Printed = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 0);

            Assert.Null(template.Printed);
            Assert.False(template.IsPrinted());
        }

        [Fact]
        public void MakeCompatible_Sets_Missing_Consent_Type_To_S_For_Sign_File_In_Version_1()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new OfferAttachmentXmlModel();
            template.Description = "Plná moc";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;
            template.ConsentType = null;

            var service = new OfferAttachmentParserService(logger);
            service.MakeCompatible(offer, template, 0);

            Assert.Equal(Constants.FileAttributeValues.CONSENT_TYPE_S, template.ConsentType);
        }

        [Fact]
        public void MakeCompatible_Sets_Missing_Consent_Type_To_S_When_Index_Equals_0()
        {
            var logger = new MemoryLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid, false, 1);

            var template = new OfferAttachmentXmlModel();
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

            var template = new OfferAttachmentXmlModel();
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

            var template = new OfferAttachmentXmlModel();
            template.Description = "Plná moc";
            template.IdAttach = "A10";
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;

            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Attachments = new[] { template };
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, "3", "04.01.2021");
            var offer = new OfferModel(offerXml, 1, offerHeader, false, false, new OfferAttributeModel[] { });

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

        [Fact]
        public void Check_Returns_AggregatedException_When_IdAttach_Is_Empty()
        {
            var logger = new MemoryLogger();

            var service = new OfferAttachmentParserService(logger);
        }

        [Fact]
        public void GetAttributes_Just_Converts_Data_To_Custom_Model_One_To_One()
        {
            var logger = new MemoryLogger();

            var attrs = new List<ZCCH_ST_ATTRIB>();
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "ATR_A", ATTRVAL = "AAA", ATTRINDX = "01" });
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "ATR_B", ATTRVAL = "BBB", ATTRINDX = "02" });
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "ATR_C", ATTRVAL = "CCC", ATTRINDX = "03" });

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = attrs.ToArray();

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetAttributes(file);

            Assert.True(result.Length == attrs.Count);
        }

        [Fact]
        public void GetAttributes_Just_Converts_Data_To_Custom_Model()
        {
            var logger = new MemoryLogger();
            var attr = new ZCCH_ST_ATTRIB() { ATTRID = "ATR_A", ATTRVAL = "AAA", ATTRINDX = "01" };
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetAttributes(file).First();

            Assert.Equal(attr.ATTRID, result.Key);
            Assert.Equal(attr.ATTRVAL, result.Value);
            Assert.Equal(1, result.Index);
        }

        [Fact]
        public void GetAttributes_Wont_Broke_When_Attributes_Are_Null()
        {
            var logger = new MemoryLogger();
            var file = new ZCCH_ST_FILE();

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetAttributes(file);

            Assert.Empty(result);
        }

        [Fact]
        public void GetModel_Returns_Null_When_No_Match_Found()
        {
            var logger = new MemoryLogger();
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = "AAA" } };
            var attachment = new OfferAttachmentXmlModel();
            attachment.IdAttach = "BBB";
            attachment.Printed = "X";
            attachment.Description = "file.pdf";
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.Attachments = new[] { attachment };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetModel(offer, attachment, new[] { file });

            Assert.Null(result);
        }

        [Fact]
        public void GetModel_Returns_Model_Created_With_File_Data_When_File_Is_Printed()
        {
            var logger = new MemoryLogger();
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = "AAA" } };
            file.MIMETYPE = "application/pdf-asfbasd";
            file.FILENAME = "BN_512312414_A10.pdf";
            var attachment = new OfferAttachmentXmlModel();
            attachment.IdAttach = "AAA";
            attachment.Printed = "X";
            attachment.Description = "file.pdf";
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.Attachments = new[] { attachment };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetModel(offer, attachment, new[] { file });

            Assert.NotNull(result);

            Assert.Equal(file.MIMETYPE, result.MimeType);
            Assert.True(result.Attributes.Length == file.ATTRIB.Length);
            Assert.Equal(file.FILENAME, result.OriginalFileName);
        }

        [Fact]
        public void GetModel_Returns_Model_As_Template_For_Uploading_Without_File()
        {
            var logger = new MemoryLogger();
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = "AAA" } };
            file.MIMETYPE = "application/pdf-asfbasd";
            file.FILENAME = "BN_512312414_A10.pdf";
            var attachment = new OfferAttachmentXmlModel();
            attachment.IdAttach = "AAA";
            attachment.Printed = null; // this is important
            attachment.Description = "file.pdf";
            var offer = this.CreateOffer();
            offer.Xml.Content.Body.Attachments = new[] { attachment };

            var service = new OfferAttachmentParserService(logger);
            var result = service.GetModel(offer, attachment, new[] { file });

            Assert.NotNull(result);

            Assert.Null(result.MimeType);
            Assert.True(result.Attributes.Length == 0);
            Assert.Equal(attachment.Description, result.OriginalFileName);
        }
    }
}
