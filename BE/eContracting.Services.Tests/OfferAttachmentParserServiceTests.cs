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
    public class OfferAttachmentParserServiceTests
    {
        [Fact]
        public void Parse_Returns_Empty_When_No_Offer_Attachments()
        {
            var logger = new TestLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = "XYZ";
            offerXmlModel.Content.Body.BusProcessType = "123";
            offerXmlModel.Content.Body.Attachments = new DocumentTemplateModel[] { };
            var offerHeader = new OfferHeaderModel("NABIDKA", guid, "3", "2020-11-16");
            var offer = new OfferModel(offerXmlModel, 2, offerHeader, new OfferAttributeModel[] { });

            var files = new List<ZCCH_ST_FILE>();

            var service = new OfferAttachmentParserService(logger);
            var result = service.Parse(offer, files.ToArray());

            Assert.Empty(result);

            var log = logger.Fatals.First();
            Assert.Equal(guid, log.Key);
        }
    }
}
