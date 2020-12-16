﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using eContracting.Tests;
using Xunit;

namespace eContracting.Core.Tests
{
    public class UtilsTests : BaseTestClass
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
        [InlineData("http://econtracting2.innogy.cz.local/page", "q", "john", "http://econtracting2.innogy.cz.local/page?q=john")]
        [InlineData("http://econtracting2.innogy.cz.local/page?q=john", "u", "uu", "http://econtracting2.innogy.cz.local/page?q=john&u=uu")]
        public void SetQuery_With_Text_Url_Append_To_AbsoluteUrl(string absoluteUrl, string key, string value, string expected)
        {
            var result = Utils.SetQuery(absoluteUrl, key, value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("http://econtracting2.innogy.cz.local/page", "q", "john", "http://econtracting2.innogy.cz.local/page?q=john")]
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

        [Fact]
        public void GetQueryString_Returns_Empty_String_When_Null_Collection()
        {
            var result = Utils.GetQueryString((NameValueCollection)null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetQueryString_Returns_Empty_String_When_Empty_Collection()
        {
            var collection = new NameValueCollection();

            var result = Utils.GetQueryString(collection);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetQueryString_Returns_Query_Without_Leading_Questionmark()
        {
            var collection = new NameValueCollection();
            collection.Add("k1", "v1");
            collection.Add("k2", "v2");
            collection.Add("k3", "v3");

            var result = Utils.GetQueryString(collection);

            Assert.False(result.StartsWith("?"));
        }

        [Fact]
        public void GetQueryString_Returns_Query_String()
        {
            var collection = new NameValueCollection();
            collection.Add("k1", "v1");
            collection.Add("k2", "v2");
            collection.Add("k3", "v3");

            var result = Utils.GetQueryString(collection);

            Assert.Equal("k1=v1&k2=v2&k3=v3", result);
        }

        [Theory]
        [InlineData("key","aaa", "key=aaa")]
        [InlineData("key", "a b", "key=a+b")]
        [InlineData("key", "žluťoučký kůň", "key=%c5%belu%c5%a5ou%c4%8dk%c3%bd+k%c5%af%c5%88")]
        public void GetQueryString_Returns_Encoded_Values(string key, string value, string expected)
        {
            var collection = new NameValueCollection();
            collection.Add(key, value);

            var result = Utils.GetQueryString(collection);

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Returns non empty result with 32 characters")]
        public void GetUniqueKey_With_Random_Ids()
        {
            var offerId = Guid.NewGuid();
            var loginTypeId = Guid.NewGuid();

            var loginType = new LoginTypeModel();
            loginType.ID = loginTypeId;

            var offer = this.CreateOffer();

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

            var offer = this.CreateOffer(offerId);
            var result = Utils.GetUniqueKey(loginType, offer);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetUniqueKey_Throws_ArgumentNullException_When_LoginType_Null()
        {
            var offerId = Guid.NewGuid();

            var offer = this.CreateOffer(offerId);

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey((LoginTypeModel)null, offer); });
        }

        [Fact]
        public void GetUniqueKey_Throws_ArgumentNullException_When_Offer_Null()
        {
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey(loginType, (OfferModel)null); });
        }

        [Fact]
        public void GetRawXml_Returns_Null_When_Given_File_Is_Null()
        {
            ZCCH_ST_FILE file = null;

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact]
        public void GetRawXml_Returns_Null_When_Given_File_Content_Is_Null()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = null;

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact]
        public void GetRawXml_Returns_Null_When_Given_File_Content_Is_0()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = new byte[] { };

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact]
        public void GetRawXml_Returns_String_Representation()
        {
            var expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = Encoding.UTF8.GetBytes(expected);

            var result = Utils.GetRawXml(file);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetReplacedTextTokens_Returns_Not_Modified_Given_Text_Because_Text_Is_Empty(string text)
        {
            var dic = new Dictionary<string, string>();

            var result = Utils.GetReplacedTextTokens(text, dic);

            Assert.Equal(text, result);
        }

        [Fact]
        public void GetReplacedTextTokens_Returns_Not_Modified_Given_Text_With_Input_Dictionary_Is_Null()
        {
            var expected = "Hello {FIRSTNAME}";
            Dictionary<string, string> dic = null;

            var result = Utils.GetReplacedTextTokens(expected, dic);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetReplacedTextTokens_Returns_Text_With_Replaced_Tokens()
        {
            var token1 = "John";
            var token2 = "Škoda Superb";
            var text = "Hello {FIRSTNAME}, how is working your {PRODUCT_NAME}?";
            var expected = $"Hello {token1}, how is working your {token2}?";
            var dic = new Dictionary<string, string>();
            dic.Add("FIRSTNAME", token1);
            dic.Add("PRODUCT_NAME", token2);

            var result = Utils.GetReplacedTextTokens(text, dic);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateAttributesFromTemplate_Returns_Attributes_IDATTACH_From_Template()
        {
            var idAttachValue = "XYZ";
            var template = new DocumentTemplateModel();
            template.IdAttach = idAttachValue;

            var result = Utils.CreateAttributesFromTemplate(template);

            Assert.Contains(result, (attr) => { return attr.ATTRID == Constants.FileAttributes.TYPE && attr.ATTRVAL == idAttachValue; });
        }

        [Theory]
        [InlineData(" style=\"text-align: center;\"", "")]
        [InlineData("<p style=\"margin-top:0pt;margin-bottom:0pt\">", "<p>")]
        [InlineData("data=\"mydata\" style=\"text-align: center;\"", "data=\"mydata\"")]
        public void ReplaceXmlAttributes_Remove_Style_Attribute_From_Given_String(string input, string expected)
        {
            var result = Utils.ReplaceXmlAttributes(input);

            Assert.Equal(expected, result);
        }
    }
}
