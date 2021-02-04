using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using eContracting.Models;
using eContracting.Tests;
using Moq;
using Xunit;

namespace eContracting.Services.Tests
{
    [ExcludeFromCodeCoverage]
    [Trait("Service", "OfferParserService")]
    public class OfferParserServiceTests
    {
        [Fact]
        [Trait("Method", "IsAccepted")]
        public void IsAccepted_Returns_False_Because_Attribute_Doesnt_Exist()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new ZCCH_ST_ATTRIB[] { };
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "BD540583CB134FC297139FDD6773DA60";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.IsAccepted(response);

            Assert.False(result);
        }

        [Fact]
        [Trait("Method", "IsAccepted")]
        public void IsAccepted_Returns_False_Because_Attribute_Doenst_Have_Digic()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.ACCEPTED_DATE;
            attr.ATTRVAL = "";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new[] { attr };
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "3562A7B150484AC280E83E40F9B9A71C";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.IsAccepted(response);

            Assert.False(result);
        }

        [Fact]
        [Trait("Method", "IsAccepted")]
        public void IsAccepted_Returns_True_Because_Attribute_Haves_Digic()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.ACCEPTED_DATE;
            attr.ATTRVAL = "20201015225809";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new[] { attr };
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "8F74AA9622244827A4086A9977313B0B";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.IsAccepted(response);

            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [Trait("Method", "MakeCompatible")]
        public void MakeCompatible_Set_Default_Value_00_For_BusProcess(string value)
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcess = value;
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            service.MakeCompatible(offerXml, 1);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS, offerXml.Content.Body.BusProcess);
        }

        [Fact]
        [Trait("Method", "MakeCompatible")]
        public void MakeCompatible_Set_Default_Value_A_For_BusProcessTyoe_When_Campaing_Missing()
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Campaign = null;
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            service.MakeCompatible(offerXml, 1);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS_TYPE_A, offerXml.Content.Body.BusProcessType);
        }

        [Fact]
        [Trait("Method", "MakeCompatible")]
        public void MakeCompatible_Set_Default_Value_B_For_BusProcessTyoe_When_Campaing_Set()
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Campaign = "any value";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            service.MakeCompatible(offerXml, 1);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS_TYPE_B, offerXml.Content.Body.BusProcessType);
        }

        [Fact]
        [Trait("Method", "MakeCompatible")]
        public void MakeCompatible_Do_Not_Set_Value_For_BusProcess_When_Already_Exists()
        {
            var value = Guid.NewGuid().ToString("N");
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcess = value;
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            service.MakeCompatible(offerXml, 1);

            Assert.Equal(value, offerXml.Content.Body.BusProcess);
        }

        [Fact]
        [Trait("Method", "MakeCompatible")]
        public void MakeCompatible_Do_Not_Set_Value_For_BusProcessType_When_Already_Exists()
        {
            var value = Guid.NewGuid().ToString("N");
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcessType = value;
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            service.MakeCompatible(offerXml, 1);

            Assert.Equal(value, offerXml.Content.Body.BusProcessType);
        }

        [Fact]
        [Trait("Method", "GetVersion")]
        public void GetVersion_Returns_1_When_No_Attribute_Exists()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new ZCCH_ST_ATTRIB[] { };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetVersion(response);

            Assert.Equal(1, result);
        }

        [Fact]
        [Trait("Method", "GetVersion")]
        public void GetVersion_Returns_2_When_Attribute_MODELO_OFERTA_Exists_With_Value_01()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.VERSION;
            attr.ATTRVAL = "01";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new [] { attr };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetVersion(response);

            Assert.Equal(2, result);
        }

        [Fact]
        [Trait("Method", "GetVersion")]
        public void GetVersion_Throws_ApplicationException_When_Attribute_MODELO_OFERTA_Doesnt_Have_Value_01()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.VERSION;
            attr.ATTRVAL = "02";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new [] { attr };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);

            Assert.Throws<ApplicationException>(() => { service.GetVersion(response); });
        }

        [Fact]
        [Trait("Method", "GetCoreFile")]
        public void GetCoreFile_Always_Return_First_When_Only_1_Exists()
        {
            var file = new ZCCH_ST_FILE();
            file.FILENAME = "myfile";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_FILES = new[] { file };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetCoreFile(response);

            Assert.Equal(file, result.File);
        }

        [Fact]
        [Trait("Method", "GetCoreFile")]
        public void GetCoreFile_Returns_First_When_Version_1()
        {
            var file1 = new ZCCH_ST_FILE();
            file1.FILENAME = "file1";
            var file2 = new ZCCH_ST_FILE();
            file2.FILENAME = "file2";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new ZCCH_ST_ATTRIB[] { };
            response.ET_FILES = new[] { file1, file2 };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetCoreFile(response);

            Assert.Equal(file1, result.File);
        }

        [Fact]
        [Trait("Method", "GetCoreFile")]
        public void GetCoreFile_Returns_File_Not_AD1_For_Version_2()
        {
            var coreFile = new ZCCH_ST_FILE();
            coreFile.FILENAME = "SBN";
            coreFile.ATTRIB = new ZCCH_ST_ATTRIB[] { };
            var ad1File = new ZCCH_ST_FILE();
            ad1File.FILENAME = "AD1";
            ad1File.ATTRIB = new ZCCH_ST_ATTRIB[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = Constants.FileAttributeValues.TEXT_PARAMETERS } };
            var response = new ZCCH_CACHE_GETResponse();
            // defines version 2
            response.ET_ATTRIB = new ZCCH_ST_ATTRIB[] { new ZCCH_ST_ATTRIB() { ATTRID = Constants.OfferAttributes.VERSION, ATTRVAL = Constants.OfferAttributeValues.VERSION_2 } };
            response.ET_FILES = new[] { coreFile, ad1File };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetCoreFile(response);

            Assert.Equal(coreFile, result.File);
        }

        [Fact]
        [Trait("Method", "GetTextParameters")]
        public void GetTextParameters_Return_Empty_When_Invalid_Path()
        {
            var xml = new StringBuilder();
            xml.Append("<parameters>");
            xml.Append("</parameters>");
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = Encoding.UTF8.GetBytes(xml.ToString());
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetTextParameters(new[] { new OfferFileXmlModel(file) });

            Assert.Empty(result);
        }

        [Fact]
        [Trait("Method", "GetTextParameters")]
        public void GetTextParameters_Overwrites_Parameter_When_Previous_Empty_In_Multiple_Files()
        {
            var xml1 = new StringBuilder();
            xml1.Append("<form>");
            xml1.Append("<parameters>");
            xml1.Append("<PARAM/>");
            xml1.Append("</parameters>");
            xml1.Append("</form>");
            var xml2 = new StringBuilder();
            xml2.Append("<form>");
            xml2.Append("<parameters>");
            xml2.Append("<PARAM>TEXT_B</PARAM>");
            xml2.Append("</parameters>");
            xml2.Append("</form>");
            var file1 = new ZCCH_ST_FILE();
            file1.FILECONTENT = Encoding.UTF8.GetBytes(xml1.ToString());
            var file2 = new ZCCH_ST_FILE();
            file2.FILECONTENT = Encoding.UTF8.GetBytes(xml2.ToString());
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetTextParameters(new[] { new OfferFileXmlModel(file1), new OfferFileXmlModel(file2) });

            Assert.Equal("TEXT_B", result["PARAM"]);
        }

        [Fact]
        [Trait("Method", "GetHeader")]
        public void GetHeader_Sets_CCHKEY_Value()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "myguid";
            response.ES_HEADER.CCHSTAT = "3";
            response.ES_HEADER.CCHTYPE = "NABIDKA";
            response.ES_HEADER.CCHVALTO = "20201122";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetHeader(response);

            Assert.Equal(response.ES_HEADER.CCHKEY, result.CCHKEY);
        }

        [Fact]
        [Trait("Method", "GetHeader")]
        public void GetHeader_Sets_CCHSTAT_Value()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "myguid";
            response.ES_HEADER.CCHSTAT = "3";
            response.ES_HEADER.CCHTYPE = "NABIDKA";
            response.ES_HEADER.CCHVALTO = "20201122";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetHeader(response);

            Assert.Equal(response.ES_HEADER.CCHSTAT, result.CCHSTAT);
        }

        [Fact]
        [Trait("Method", "GetHeader")]
        public void GetHeader_Sets_CCHTYPE_Value()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "myguid";
            response.ES_HEADER.CCHSTAT = "3";
            response.ES_HEADER.CCHTYPE = "NABIDKA";
            response.ES_HEADER.CCHVALTO = "20201122";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetHeader(response);

            Assert.Equal(response.ES_HEADER.CCHTYPE, result.CCHTYPE);
        }

        [Fact]
        [Trait("Method", "GetHeader")]
        public void GetHeader_Sets_CCHVALTO_Value()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ES_HEADER = new ZCCH_ST_HEADER();
            response.ES_HEADER.CCHKEY = "myguid";
            response.ES_HEADER.CCHSTAT = "3";
            response.ES_HEADER.CCHTYPE = "NABIDKA";
            response.ES_HEADER.CCHVALTO = "20201122";
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetHeader(response);

            Assert.Equal(response.ES_HEADER.CCHVALTO, result.CCHVALTO);
        }

        [Fact]
        [Trait("Method", "GetAttributes")]
        public void GetAttributes_Returns_All_Attributes()
        {
            var attr1 = new ZCCH_ST_ATTRIB();
            attr1.ATTRID = "DUMMY_ONE";
            attr1.ATTRVAL = "lorem";
            var attr2 = new ZCCH_ST_ATTRIB();
            attr2.ATTRID = "DUMMY_TWO";
            attr2.ATTRVAL = "lorem";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new[] { attr1, attr2 };
            var logger = new MemoryLogger();
            var settingsService = new Mock<ISettingsReaderService>().Object;

            var service = new OfferParserService(settingsService, logger);
            var result = service.GetAttributes(response);

            Assert.Contains(result, x => x.Key == attr1.ATTRID);
            Assert.Contains(result, x => x.Value == attr1.ATTRVAL);
            Assert.Contains(result, x => x.Key == attr2.ATTRID);
            Assert.Contains(result, x => x.Value == attr2.ATTRVAL);
        }

        public void ProcessResponse()
        {
            throw new NotImplementedException();
        }
    }
}
