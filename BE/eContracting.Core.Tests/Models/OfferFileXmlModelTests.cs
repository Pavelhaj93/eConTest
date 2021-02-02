using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class OfferFileXmlModelTests
    {
        [Fact(DisplayName = "Returns null when given file content is null. Do not throws an exception.")]
        [Trait("OfferFileXmlModel", "GetRawXml")]
        public void GetRawXml_Returns_Null_When_FILECONTENT_Is_Null()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = null;

            var result = new OfferFileXmlModel(file).GetRawXml();

            Assert.Null(result);
        }


        [Fact(DisplayName = "Returns null when given file content is empty (0). Do not throws an exception.")]
        [Trait("OfferFileXmlModel", "GetRawXml")]
        public void GetRawXml_Returns_Null_When_Given_File_Content_Is_0()
        {
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = new byte[] { };

            var result = new OfferFileXmlModel(file).GetRawXml();

            Assert.Null(result);
        }

        [Fact]
        [Trait("OfferFileXmlModel", "GetRawXml")]
        public void GetRawXml_Returns_String_Representation()
        {
            var expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";
            var file = new ZCCH_ST_FILE();
            file.FILECONTENT = Encoding.UTF8.GetBytes(expected);

            var result = new OfferFileXmlModel(file).GetRawXml();

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Returns matched IDATTACH value")]
        [Trait("ZCCH_ST_ATTRIB", "GetIdAttach")]
        public void GetIdAttach_Returns_IdAttach_Value_Based_On_Constants_FileAttributes_TYPE()
        {
            var expected = "XYZ";
            var attrs = new List<ZCCH_ST_ATTRIB>();
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "unknown", ATTRVAL = "hello" });
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = Constants.FileAttributes.TYPE, ATTRVAL = expected });
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = attrs.ToArray();

            var result = new OfferFileXmlModel(file).IdAttach;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Returns null when IDATTACH attribute not found")]
        [Trait("ZCCH_ST_ATTRIB", "GetIdAttach")]
        public void GetIdAttach_Returns_Null_When_Not_Found()
        {
            var attrs = new List<ZCCH_ST_ATTRIB>();
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "unknown", ATTRVAL = "hello" });
            attrs.Add(new ZCCH_ST_ATTRIB() { ATTRID = "my attribute", ATTRVAL = "ELS" });
            var file = new ZCCH_ST_FILE();
            file.ATTRIB = attrs.ToArray();

            var result = new OfferFileXmlModel(file).IdAttach;

            Assert.Null(result);
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("01", 1)]
        [InlineData("001", 1)]
        [InlineData("123", 123)]
        public void GetCounter_Returns_Value_From_Attribute_When_Its_Integer(string value, int expected)
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.COUNTER;
            attr.ATTRVAL = value;

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var result = new OfferFileXmlModel(file).Counter;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("x01")]
        [InlineData("1.2")]
        [InlineData("1,2")]
        [InlineData("abc")]
        public void GetCounter_Returns_Default_When_Value_In_Not_Integer(string value)
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = Constants.OfferAttributes.COUNTER;
            attr.ATTRVAL = value;

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var result = new OfferFileXmlModel(file).Counter;

            Assert.Equal(Constants.FileAttributeDefaults.COUNTER, result);
        }

        [Fact]
        public void GetCounter_Returns_Default_When_Attribute_Not_Found()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = "dummy";
            attr.ATTRVAL = "123";

            var file = new ZCCH_ST_FILE();
            file.ATTRIB = new[] { attr };

            var result = new OfferFileXmlModel(file).Counter;

            Assert.Equal(Constants.FileAttributeDefaults.COUNTER, result);
        }
    }
}
