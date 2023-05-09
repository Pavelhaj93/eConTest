using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using eContracting.Tests;
using JSNLog.Infrastructure;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class OfferModelTests : BaseTestClass
    {
        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("8")]
        [InlineData("9")]
        public void Guid_Returns_Parameter_From_Header_CCHSTAT(string state)
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            var zcchStHeader = new ZCCH_ST_HEADER();
            zcchStHeader.CCHSTAT = state;
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.State;

            Assert.Equal(state, result);
        }

        [Fact]
        public void Guid_Returns_Parameter_From_Header_CCHKEY()
        {
            var exceptedGuid = Guid.NewGuid().ToString("B");

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            var zcchStHeader = new ZCCH_ST_HEADER();
            zcchStHeader.CCHKEY = exceptedGuid;
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.Guid;

            Assert.Equal(exceptedGuid, result);
        }

        [Fact]
        public void GDPRKey_Returns_Value_From_Attributes()
        {
            var expectedValue = "some value";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.KEY_GDPR;
            zcchStAttrib.ATTRVAL = expectedValue;
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            var result = offer.GDPRKey;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void HasGDPR_Returns_True_When_Attribute_With_Value_Exists()
        {
            var expectedValue = "some value";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.KEY_GDPR;
            zcchStAttrib.ATTRVAL = expectedValue;
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            Assert.True(offer.HasGDPR);
        }

        [Fact]
        public void McfuRegStat_Returns_Value_From_Attributes()
        {
            var expectedValue = "some value";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.MCFU_REG_STAT;
            zcchStAttrib.ATTRVAL = expectedValue;
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            var result = offer.First().McfuRegStat;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void HasMcfu_Returns_True_When_Attribute_With_Value_Exists()
        {
            var expectedValue = "some value";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.MCFU_REG_STAT;
            zcchStAttrib.ATTRVAL = expectedValue;
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            Assert.True(offer.HasMcfu);
        }

        [Fact]
        public void HasGDPR_Returns_False_When_Attribute_Value_Missing()
        {
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.KEY_GDPR;
            zcchStAttrib.ATTRVAL = "";
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            Assert.False(offer.HasGDPR);
        }

        [Fact]
        public void HasGDPR_Returns_False_When_Attribute_Missing()
        {
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = "fake atribute";
            zcchStAttrib.ATTRVAL = "xyz";
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            Assert.False(offer.HasGDPR);
        }

        [Fact]
        public void Campaign_Returns_Value_From_Attributes()
        {
            var expectedValue = "some value";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.CAMPAIGN;
            zcchStAttrib.ATTRVAL = expectedValue;
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = this.CreateOffer(Guid.NewGuid().ToString("B"), false, 3, "6", DateTime.Now.AddDays(1).ToString(), new[] { attribute });

            var result = offer.Campaign;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void Commodity_Returns_Value_From_Xml_Content_Body_EanOrAndEic()
        {
            var expectedValue = "234632453453";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.EanOrAndEic = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.Commodity;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void Birthday_Returns_Value_From_Xml_Content_Body_BIRTHDT()
        {
            var expectedValue = "12.3.1987";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BIRTHDT = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.Birthday;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void PartnerNumber_Returns_Value_From_Xml_Content_Body_PARTNER()
        {
            var expectedValue = "ACTUM";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.PARTNER = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.PartnerNumber;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void PostNumber_Returns_Value_From_Xml_Content_Body_PscTrvaleBydliste()
        {
            var expectedValue = "Plynární 10";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.PscTrvaleBydliste = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.PostNumber;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void PostNumberConsumption_Returns_Value_From_Xml_Content_Body_PscMistaSpotreby()
        {
            var expectedValue = "Plynární 10";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.PscMistaSpotreby = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.PostNumberConsumption;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void Process_Returns_Value_From_Xml_Content_Body_BusProcess()
        {
            var expectedValue = "01";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.Process;

            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void ProcessType_Returns_Value_From_Xml_Content_Body_BusProcessType()
        {
            var expectedValue = "A";

            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcessType = expectedValue;
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new OfferAttributeModel[] { });

            var result = offer.ProcessType;

            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData("https://test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        [InlineData("//test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        [InlineData("test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        public void RegistrationLink_Returns_Absolute_Url(string value, string expected)
        {
            var offer = this.CreateOffer();
            offer.First().TextParameters[Constants.OfferTextParameters.REGISTRATION_LINK] = value;

            var result = offer.First().RegistrationLink;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData((string)null)]
        //[InlineData("nejaka-blbost")] this behaves as absolute url
        public void RegistrationLink_Returns_Null_For_Incorrect_(string value)
        {
            var offer = this.CreateOffer();
            offer.TextParameters[Constants.OfferTextParameters.REGISTRATION_LINK] = value;

            var result = offer.First().RegistrationLink;

            Assert.Null(result);
        }

        [Fact]
        public void RegistrationLink_Returns_Null_When_Key_Doesnt_Exist()
        {
            var offer = this.CreateOffer();

            if (offer.TextParameters.ContainsKey(Constants.OfferTextParameters.REGISTRATION_LINK))
            {
                offer.TextParameters.Remove(Constants.OfferTextParameters.REGISTRATION_LINK);
            }

            var result = offer.First().RegistrationLink;

            Assert.Null(result);
        }

        [Fact]
        public void CanBeCanceled_Returns_True_Only_For_Process_01_And_Attribute_ORDER_ORIGIN_Equals_2()
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = "01";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.ORDER_ORIGIN;
            zcchStAttrib.ATTRVAL = "2";
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new [] { attribute });

            var result = offer.CanBeCanceled;

            Assert.True(result);
        }

        [Theory]
        [InlineData("00")]
        [InlineData("02")]
        [InlineData("03")]
        [InlineData("04")]
        public void CanBeCanceled_Returns_False_For_Other_Processes_And_Attribute_ORDER_ORIGIN_Equals_2(string process)
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = process;
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.ORDER_ORIGIN;
            zcchStAttrib.ATTRVAL = "2";
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new[] { attribute });

            var result = offer.CanBeCanceled;

            Assert.False(result);
        }

        [Fact]
        public void CanBeCanceled_Returns_False_For_Process_01_And_Attribute_ORDER_ORIGIN_Not_Equals_2()
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = "01";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.ORDER_ORIGIN;
            zcchStAttrib.ATTRVAL = "1";
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new[] { attribute });

            var result = offer.CanBeCanceled;

            Assert.False(result);
        }

        [Fact]
        public void CanBeCanceled_Returns_False_For_Process_01_And_Missing_Attribute_ORDER_ORIGIN()
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = "01";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = "fake attribute";
            zcchStAttrib.ATTRVAL = "2";
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new[] { attribute });

            var result = offer.CanBeCanceled;

            Assert.False(result);
        }

        [Fact]
        public void CanBeCanceled_Returns_False_For_Other_Process_01_And_Attribute_ORDER_ORIGIN_Not_Equals_2()
        {
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();
            offerXmlModel.Content.Body.BusProcess = "00";
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.ORDER_ORIGIN;
            zcchStAttrib.ATTRVAL = "1";
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var attribute = new OfferAttributeModel(zcchStAttrib);

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new[] { attribute });

            var result = offer.CanBeCanceled;

            Assert.False(result);
        }

        [Fact]
        public void ShowPrices_Returns_False_When_NO_PROD_CHNG_Equals_Checked()
        {
            var zcchStAttrib = new ZCCH_ST_ATTRIB();
            zcchStAttrib.ATTRID = Constants.OfferAttributes.NO_PROD_CHNG;
            zcchStAttrib.ATTRVAL = Constants.CHECKED;
            var attribute = new OfferAttributeModel(zcchStAttrib);
            var zcchStHeader = new ZCCH_ST_HEADER();
            var offerHeaderModel = new OfferHeaderModel(zcchStHeader);
            var offerXmlModel = new OfferXmlModel();
            offerXmlModel.Content = new OfferContentXmlModel();
            offerXmlModel.Content.Body = new OfferBodyXmlModel();

            var offer = new OfferModel(offerXmlModel, 3, offerHeaderModel, false, false, DateTime.Now.AddMinutes(1), new[] { attribute });

            var result = offer.ShowPrices;

            Assert.False(result);
        }
    }
}
