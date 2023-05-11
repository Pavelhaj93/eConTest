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
    public class OfferServiceTests : BaseTestClass
    {
        public void AcceptOffer_Set_Status_5()
        {
            var attributes = new List<OfferAttributeModel>();
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, "yes"));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray()).First();
            var offerSubmitModel = new OfferSubmitDataModel();
            var user = new UserCacheDataModel();
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns("127.0.0.0");

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);
            
            offerService.AcceptOffer(offer, offerSubmitModel, user, "hf7439rh83h9h018hd");


        }

        [Fact]
        public void GetAttributesForAccept_Has_ACCEPTED_AT()
        {
            var attributes = new List<OfferAttributeModel>();
            var datetime = DateTime.UtcNow;
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, "yes"));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray());
            var offerSubmitModel = new OfferSubmitDataModel();
            var user = this.CreateTwoSecretsUser(offer);
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns("127.0.0.0");

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);

            var result = offerService.GetAttributesForAccept(offer.First(), offerSubmitModel, user, datetime);

            string timestampString = datetime.ToString(Constants.TimeStampFormat);
            Decimal outValue = 1M;
            Decimal.TryParse(timestampString, out outValue);

            Assert.Contains(result, x => x.ATTRID == "ACCEPTED_AT" && x.ATTRVAL == outValue.ToString());
        }

        [Fact]
        public void GetAttributesForAccept_Has_IP_ADDRESS()
        {
            var expected = "127.0.0.0";
            var attributes = new List<OfferAttributeModel>();
            var datetime = DateTime.UtcNow;
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, "yes"));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray());
            var offerSubmitModel = new OfferSubmitDataModel();
            var user = this.CreateTwoSecretsUser(offer);
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns(expected);

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);

            var result = offerService.GetAttributesForAccept(offer.First(), offerSubmitModel, user, datetime);

            Assert.Contains(result, x => x.ATTRID == "IP_ADDRESS" && x.ATTRVAL == expected);
        }

        [Fact]
        public void GetAttributesForAccept_Has_ZIDENTITYID()
        {
            var expected = "6940342";
            var attributes = new List<OfferAttributeModel>();
            var datetime = DateTime.UtcNow;
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, expected));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray());
            var offerSubmitModel = new OfferSubmitDataModel();
            var user = this.CreateTwoSecretsUser(offer);
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns("127.0.0.0");

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);

            var result = offerService.GetAttributesForAccept(offer.First(), offerSubmitModel, user, datetime);

            Assert.Contains(result, x => x.ATTRID == Constants.OfferAttributes.ZIDENTITYID && x.ATTRVAL == expected);
        }

        [Fact]
        public void GetAttributesForAccept_Has_SERVPROV_OLD()
        {
            var expected = "6940342";
            var attributes = new List<OfferAttributeModel>();
            var datetime = DateTime.UtcNow;
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, expected));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray());
            var offerSubmitModel = new OfferSubmitDataModel();
            offerSubmitModel.Supplier = "ČESKÁ PLYNÁRENSKÁ";
            var user = this.CreateTwoSecretsUser(offer);
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns("127.0.0.0");

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);

            var result = offerService.GetAttributesForAccept(offer.First(), offerSubmitModel, user, datetime);

            Assert.Contains(result, x => x.ATTRID == "SERVPROV_OLD" && x.ATTRVAL == offerSubmitModel.Supplier);
        }

        [Fact]
        public void GetAttributesForAccept_Has_ACCEPTED_BY_ALIAS()
        {
            var expected = "6940342";
            var attributes = new List<OfferAttributeModel>();
            var datetime = DateTime.UtcNow;
            attributes.Add(new OfferAttributeModel(0, Constants.OfferAttributes.ZIDENTITYID, expected));
            var offer = this.CreateOffer(this.CreateGuid(), false, 3, "6", "21000101", attributes.ToArray());
            var offerSubmitModel = new OfferSubmitDataModel();
            var user = this.CreateCognitoUser(offer);
            var logger = new MemoryLogger();
            var mockUserFileCacheService = new Mock<IUserFileCacheService>();
            var mockSettingsReaderService = new Mock<ISettingsReaderService>();
            var mockServiceFactory = new Mock<IServiceFactory>();
            var mockOfferDataService = new Mock<IOfferDataService>();
            var mockOfferParserService = new Mock<IOfferParserService>();
            var mockOfferAttachmentParserService = new Mock<IOfferAttachmentParserService>();
            var mockDataRequestCacheService = new Mock<IDataRequestCacheService>();
            var mockContextWrapper = new Mock<IContextWrapper>();
            mockContextWrapper.Setup(x => x.GetIpAddress()).Returns("127.0.0.0");

            var offerService = new OfferService(
                logger,
                mockUserFileCacheService.Object,
                mockSettingsReaderService.Object,
                mockServiceFactory.Object,
                mockOfferDataService.Object,
                mockOfferParserService.Object,
                mockOfferAttachmentParserService.Object,
                mockDataRequestCacheService.Object,
                mockContextWrapper.Object);

            var result = offerService.GetAttributesForAccept(offer.First(), offerSubmitModel, user, datetime);

            Assert.Contains(result, x => x.ATTRID == "ACCEPTED_BY_ALIAS" && x.ATTRVAL == user.CognitoUser.PreferredUsername);
        }
    }
}
