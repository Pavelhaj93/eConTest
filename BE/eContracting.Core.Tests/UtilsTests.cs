using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using eContracting.Models;
using eContracting.Services;
using eContracting.Tests;
using Xunit;

namespace eContracting.Core.Tests
{
    public class UtilsTests : BaseTestClass
    {
        [Theory(DisplayName = "Gets readable file size")]
        [Trait("Utils", "GetReadableFileSize")]
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
        [Trait("Utils", "SetQuery")]
        [InlineData("http://econtracting2.innogy.cz.local/page", "q", "john", "http://econtracting2.innogy.cz.local/page?q=john")]
        [InlineData("http://econtracting2.innogy.cz.local/page?q=john", "u", "uu", "http://econtracting2.innogy.cz.local/page?q=john&u=uu")]
        public void SetQuery_With_Text_Url_Append_To_AbsoluteUrl(string absoluteUrl, string key, string value, string expected)
        {
            var result = Utils.SetQuery(absoluteUrl, key, value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Utils", "SetQuery")]
        [InlineData("http://econtracting2.innogy.cz.local/page", "q", "john", "http://econtracting2.innogy.cz.local/page?q=john")]
        [InlineData("http://econtracting2.innogy.cz.local/page?q=john", "u", "uu", "http://econtracting2.innogy.cz.local/page?q=john&u=uu")]
        public void SetQuery_Append_To_AbsoluteUrl(string absoluteUrl, string key, string value, string expected)
        {
            var result = Utils.SetQuery(new Uri(absoluteUrl), key, value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Utils", "SetQuery")]
        [InlineData("/page", "q", "john")]
        [InlineData("/page?q=john", "u", "uu")]
        public void SetQuery_Append_To_RelativeUrl_Throws_UriFormatException(string relativeUrl, string key, string value)
        {
            Assert.Throws<UriFormatException>(() => Utils.SetQuery(relativeUrl, key, value));
        }

        [Fact(DisplayName = "Throws ArgumentNullException when original url is null")]
        [Trait("Utils", "SetQuery")]
        public void SetQuery_With_Null_Url_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Utils.SetQuery((string)null, "k", "v"));
        }

        [Fact(DisplayName = "Returns empty query string when given collection is empty")]
        [Trait("Utils", "GetQueryString")]
        public void GetQueryString_Returns_Empty_String_When_Null_Collection()
        {
            var result = Utils.GetQueryString((NameValueCollection)null);

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "Returns empty string when given collection is empty")]
        [Trait("Utils", "GetQueryString")]
        public void GetQueryString_Returns_Empty_String_When_Empty_Collection()
        {
            var collection = new NameValueCollection();

            var result = Utils.GetQueryString(collection);

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "Returns query string without leading questionmark")]
        [Trait("Utils", "GetQueryString")]
        public void GetQueryString_Returns_Query_Without_Leading_Questionmark()
        {
            var collection = new NameValueCollection();
            collection.Add("k1", "v1");
            collection.Add("k2", "v2");
            collection.Add("k3", "v3");

            var result = Utils.GetQueryString(collection);

            Assert.False(result.StartsWith("?"));
        }

        [Fact(DisplayName = "Checks composed query string from given collection")]
        [Trait("Utils", "GetQueryString")]
        public void GetQueryString_Returns_Query_String()
        {
            var collection = new NameValueCollection();
            collection.Add("k1", "v1");
            collection.Add("k2", "v2");
            collection.Add("k3", "v3");

            var result = Utils.GetQueryString(collection);

            Assert.Equal("k1=v1&k2=v2&k3=v3", result);
        }

        [Theory(DisplayName = "Checks if query values are encoded")]
        [Trait("Utils", "GetQueryString")]
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
        [Trait("Utils", "GetUniqueKey")]
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

        [Theory(DisplayName = "Check correct generated unique key")]
        [Trait("Utils", "GetUniqueKey")]
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

        [Fact(DisplayName = "Throws ArgumentNullException when LoginTypeModel is empty")]
        [Trait("Utils", "GetUniqueKey")]
        public void GetUniqueKey_Throws_ArgumentNullException_When_LoginType_Null()
        {
            var offerId = Guid.NewGuid();

            var offer = this.CreateOffer(offerId);

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey((LoginTypeModel)null, offer); });
        }

        [Fact(DisplayName = "Throws ArgumentNullException when OfferModel is empty")]
        [Trait("Utils", "GetUniqueKey")]
        public void GetUniqueKey_Throws_ArgumentNullException_When_Offer_Null()
        {
            var loginType = new LoginTypeModel();
            loginType.ID = Guid.NewGuid();

            Assert.Throws<ArgumentNullException>(() => { Utils.GetUniqueKey(loginType, (OfferModel)null); });
        }

        [Fact(DisplayName = "Returns null when ZCCH_ST_FILE is null")]
        [Trait("Utils", "GetRawXml")]
        public void GetRawXml_Returns_Null_When_Given_File_Is_Null()
        {
            ZCCH_ST_FILE file = null;

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact(DisplayName = "Returns null when given file content is null. Do not throws an exception.")]
        [Trait("Utils", "GetRawXml")]
        public void GetRawXml_Returns_Null_When_Given_File_Content_Is_Null()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = null;

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact(DisplayName = "Returns null when given file content is empty (0). Do not throws an exception.")]
        [Trait("Utils", "GetRawXml")]
        public void GetRawXml_Returns_Null_When_Given_File_Content_Is_0()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = new byte[] { };

            var result = Utils.GetRawXml(file);

            Assert.Null(result);
        }

        [Fact]
        [Trait("Utils", "GetRawXml")]
        public void GetRawXml_Returns_String_Representation()
        {
            var expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = Encoding.UTF8.GetBytes(expected);

            var result = Utils.GetRawXml(file);

            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "Return unmodified input text")]
        [Trait("Utils", "GetReplacedTextTokens")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetReplacedTextTokens_Returns_Not_Modified_Given_Text_Because_Text_Is_Empty(string text)
        {
            var dic = new Dictionary<string, string>();

            var result = Utils.GetReplacedTextTokens(text, dic);

            Assert.Equal(text, result);
        }

        [Fact(DisplayName = "Do NOT replace {text} token when value doesn't exist")]
        [Trait("Utils", "GetReplacedTextTokens")]
        public void GetReplacedTextTokens_Returns_Not_Modified_Given_Text_With_Input_Dictionary_Is_Null()
        {
            var expected = "Hello {FIRSTNAME}";
            Dictionary<string, string> dic = null;

            var result = Utils.GetReplacedTextTokens(expected, dic);

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Replace {text} token with real value")]
        [Trait("Utils", "GetReplacedTextTokens")]
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

        [Fact(DisplayName = "Set IDATTACH attribute")]
        [Trait("Utils", "CreateAttributesFromTemplate")]
        public void CreateAttributesFromTemplate_Returns_Attributes_IDATTACH_From_Template()
        {
            var idAttachValue = "XYZ";
            var template = new DocumentTemplateModel();
            template.IdAttach = idAttachValue;

            var result = Utils.CreateAttributesFromTemplate(template);

            Assert.Contains(result, (attr) => { return attr.ATTRID == Constants.FileAttributes.TYPE && attr.ATTRVAL == idAttachValue; });
        }

        [Theory(DisplayName = "Set attribute from template, verify key and check output value")]
        [Trait("Utils", "CreateAttributesFromTemplate")]
        [InlineData(Constants.FileAttributes.ADDINFO, null)]
        [InlineData(Constants.FileAttributes.CONSENT_TYPE, "P")]
        [InlineData(Constants.FileAttributes.DESCRIPTION, "Smlouva o sdružených službách dodávky plynu")]
        [InlineData(Constants.FileAttributes.GROUP, "COMMODITY")]
        [InlineData(Constants.FileAttributes.GROUP_OBLIG, "X")]
        [InlineData(Constants.FileAttributes.TYPE, "EPS")]
        [InlineData(Constants.FileAttributes.ITEM_GUID, "06D969E88C3C1EEB8FC4EEF623ABF45D")]
        [InlineData(Constants.FileAttributes.OBLIGATORY, "X")]
        [InlineData(Constants.FileAttributes.PRINTED, "X")]
        [InlineData(Constants.FileAttributes.SEQUENCE_NUMBER, "002")]
        [InlineData(Constants.FileAttributes.SIGN_REQ, null)]
        [InlineData(Constants.FileAttributes.TEMPL_ALC_ID, "CRM009E")]
        [InlineData(Constants.FileAttributes.TMST_REQ, "X")]
        public void CreateAttributesFromTemplate_Returns_Attributes_With_All_Properties_From_Template(string key, string value)
        {
            var template = new DocumentTemplateModel();
            var t = template.GetType();
            var props = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (System.Reflection.PropertyInfo prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(XmlElementAttribute), false).FirstOrDefault();

                if (attr != null)
                {
                    var xmlAttr = attr as XmlElementAttribute;

                    if (xmlAttr.ElementName == key)
                    {
                        prop.SetValue(template, value);
                    }
                }
            }

            var result = Utils.CreateAttributesFromTemplate(template);

            Assert.Contains(result, x => x.ATTRID == key);

            var attrib = result.First(x => x.ATTRID == key);

            Assert.Equal(value ?? string.Empty, attrib.ATTRVAL);
        }

        [Theory(DisplayName = "Remove style attribute from inner XML content")]
        [Trait("Utils", "ReplaceXmlAttributes")]
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
