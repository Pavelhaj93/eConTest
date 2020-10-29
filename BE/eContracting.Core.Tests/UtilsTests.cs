using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData(-1, "0 B")]
        [InlineData(0, "0 B")]
        [InlineData(462, "462 B")]
        [InlineData(5523, "5.39 KB")]
        [InlineData(63093, "61.61 KB")]
        [InlineData(390123, "380.98 KB")]
        [InlineData(7842052, "7.48 MB")]
        public void GerReadableFileSize(int size, string expected)
        {
            var result = Utils.GetReadableFileSize(size);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("http://econtracting2.innogy.cz.local/page", "q","john", "http://econtracting2.innogy.cz.local/page?q=john")]
        [InlineData("http://econtracting2.innogy.cz.local/page?q=john", "u", "uu", "http://econtracting2.innogy.cz.local/page?q=john&u=uu")]
        public void SetQuery_Append_To_AbsoluteUrl(string absoluteUrl, string key, string value, string expected)
        {
            var result = Utils.SetQuery(new Uri(absoluteUrl), key, value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("/page", "q", "john")]
        [InlineData("/page?q=john", "u", "uu")]
        public void SetQuery_Append_To_RelativeUrl_Throws_UriFormatException(string relativeUrl, string key, string value)
        {
            Assert.Throws<UriFormatException>(() => Utils.SetQuery(relativeUrl, key, value));
        }

        [Fact]
        public void SetQuery_With_Null_Url_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Utils.SetQuery((string)null, "k", "v"));
        }

        [Fact(DisplayName = "Returns non empty result with 32 characters")]
        public void GetUniqueKey_With_Random_Ids()
        {
            var offerId = Guid.NewGuid();
            var loginTypeId = Guid.NewGuid();

            var loginType = new LoginTypeModel();
            loginType.ID = loginTypeId;

            var fakeOfferXmlModel = new OfferXmlModel();
            fakeOfferXmlModel.Content = new OfferContentXmlModel();

            var fakeHeaderModel = new OfferHeaderModel("NABIDKA", offerId.ToString("N"), "1", DateTime.Now.ToString("dd.MM.yyyy"));
            var fakeAttributes = new OfferAttributeModel[] { };

            var offer = new OfferModel(fakeOfferXmlModel, fakeHeaderModel, fakeAttributes);

            var result = Utils.GetUniqueKey(loginType, offer);

            Assert.NotEqual("", result);
            Assert.NotNull(result);
            Assert.Equal(32, result.Length);
        }

        [Theory]
        [InlineData("{ACF3043C-3BC3-4C86-BE4B-FEF58392E7F0}", "D38F03D409B14241BABC405AC2FD17F0", "E058CE5015129A38475164266A21DDDA")]
        public void GetUniqueKey(string login, string offerGuid, string expected)
        {
            var offerId = new Guid(offerGuid);
            var loginTypeId = new Guid(login);

            var loginType = new LoginTypeModel();
            loginType.ID = loginTypeId;

            var fakeOfferXmlModel = new OfferXmlModel();
            fakeOfferXmlModel.Content = new OfferContentXmlModel();

            var fakeHeaderModel = new OfferHeaderModel("NABIDKA", offerId.ToString("N"), "1", DateTime.Now.ToString("dd.MM.yyyy"));
            var fakeAttributes = new OfferAttributeModel[] { };

            var offer = new OfferModel(fakeOfferXmlModel, fakeHeaderModel, fakeAttributes);

            var result = Utils.GetUniqueKey(loginType, offer);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetUniqueKey_Throws_ArgumentNullException_When_LoginType_Null()
        {
            var offerId = Guid.NewGuid();

            var fakeOfferXmlModel = new OfferXmlModel();
            fakeOfferXmlModel.Content = new OfferContentXmlModel();

            var fakeHeaderModel = new OfferHeaderModel("NABIDKA", offerId.ToString("N"), "1", DateTime.Now.ToString("dd.MM.yyyy"));
            var fakeAttributes = new OfferAttributeModel[] { };

            var offer = new OfferModel(fakeOfferXmlModel, fakeHeaderModel, fakeAttributes);

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey((LoginTypeModel)null, offer); });
        }

        [Fact]
        public void GetUniqueKey_Throws_ArgumentNullException_When_Offer_Null()
        {
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey(loginType, (OfferModel)null); });
        }
    }
}
