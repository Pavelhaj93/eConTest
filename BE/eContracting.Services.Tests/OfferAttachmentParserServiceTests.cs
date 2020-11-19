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
            var logger = new TestLogger();
            var guid = Guid.NewGuid().ToString("N");
            var offer = this.CreateOffer(guid);// new OfferModel(offerXmlModel, 2, offerHeader, new OfferAttributeModel[] { });
            offer.Xml.Content.Body.BusProcess = "XYZ";
            offer.Xml.Content.Body.BusProcessType = "123";

            var files = new List<ZCCH_ST_FILE>();

            var service = new OfferAttachmentParserService(logger);
            var result = service.Parse(offer, files.ToArray());

            Assert.Empty(result);

            var log = logger.Infos.First();
            Assert.Equal(guid, log.Key);
        }
    }
}
