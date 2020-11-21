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
    [Trait("Service", "OfferParserService")]
    public class OfferParserServiceTests
    {
        [Fact]
        [Trait("Category", "Services")]
        public void IsAccepted_Returns_False_Because_Attribute_Doesnt_Exist()
        {
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new ZCCH_ST_ATTRIB[] { };
            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            var result = service.IsAccepted(response);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void IsAccepted_Returns_False_Because_Attribute_Doenst_Have_Digic()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.ACCEPTED_DATE;
            attr.ATTRVAL = "";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new [] { attr };
            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            var result = service.IsAccepted(response);

            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void IsAccepted_Returns_True_Because_Attribute_Haves_Digic()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.ACCEPTED_DATE;
            attr.ATTRVAL = "20201015225809";
            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = new [] { attr };
            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            var result = service.IsAccepted(response);

            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [Trait("Category", "Services")]
        public void MakeCompatible_Set_Default_Value_00_For_BusProcess(string value)
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcess = value;

            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            service.MakeCompatible(offerXml);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS, offerXml.Content.Body.BusProcess);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void MakeCompatible_Set_Default_Value_A_For_BusProcessTyoe_When_Campaing_Missing()
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Campaign = null;

            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            service.MakeCompatible(offerXml);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS_TYPE_A, offerXml.Content.Body.BusProcessType);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void MakeCompatible_Set_Default_Value_B_For_BusProcessTyoe_When_Campaing_Set()
        {
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.Campaign = "any value";

            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            service.MakeCompatible(offerXml);

            Assert.Equal(Constants.OfferDefaults.BUS_PROCESS_TYPE_B, offerXml.Content.Body.BusProcessType);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void MakeCompatible_Do_Not_Set_Value_For_BusProcess_When_Already_Exists()
        {
            var value = Guid.NewGuid().ToString("N");
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcess = value;

            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            service.MakeCompatible(offerXml);

            Assert.Equal(value, offerXml.Content.Body.BusProcess);
        }

        [Fact]
        [Trait("Category", "Services")]
        public void MakeCompatible_Do_Not_Set_Value_For_BusProcessType_When_Already_Exists()
        {
            var value = Guid.NewGuid().ToString("N");
            var offerXml = new OfferXmlModel();
            offerXml.Content = new OfferContentXmlModel();
            offerXml.Content.Body = new OfferBodyXmlModel();
            offerXml.Content.Body.BusProcessType = value;

            var logger = new TestLogger();

            var service = new OfferParserService(logger);
            service.MakeCompatible(offerXml);

            Assert.Equal(value, offerXml.Content.Body.BusProcessType);
        }
    }
}
