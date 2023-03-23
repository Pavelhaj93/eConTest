using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using JSNLog.Infrastructure;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class OfferModelTests : BaseTestClass
    {
        [Theory]
        [InlineData("https://test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        [InlineData("//test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        [InlineData("test.innogy.cz/innosvet/registrace", "https://test.innogy.cz/innosvet/registrace")]
        public void RegistrationLink_Returns_Absolute_Url(string value, string expected)
        {
            var offer = this.CreateOffer();
            offer.TextParameters[Constants.OfferTextParameters.REGISTRATION_LINK] = value;

            var result = offer.RegistrationLink;

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

            var result = offer.RegistrationLink;

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

            var result = offer.RegistrationLink;

            Assert.Null(result);
        }
    }
}
