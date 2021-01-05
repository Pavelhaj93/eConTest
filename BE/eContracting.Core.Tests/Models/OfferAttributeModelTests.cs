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
    public class OfferAttributeModelTests
    {
        [Fact]
        public void Constructor_Process_Attribute_Parameter()
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRID = "TEMPLATE";
            attr.ATTRINDX = "07";
            attr.ATTRVAL = "A10";

            var model = new OfferAttributeModel(attr);

            Assert.Equal(attr.ATTRID, model.Key);
            Assert.Equal(attr.ATTRVAL, model.Value);
            Assert.Equal(7, model.Index);
        }

        [Fact]
        public void Constructor_Process_All_Custom_Parameters()
        {
            var model = new OfferAttributeModel(3, "TEMPLATE", "ELH");

            Assert.Equal("TEMPLATE", model.Key);
            Assert.Equal("ELH", model.Value);
            Assert.Equal(3, model.Index);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("01", 1)]
        [InlineData("11", 11)]
        public void Constructor_Convert_Index_Value_To_Integer(string rawValue, int expected)
        {
            var attr = new ZCCH_ST_ATTRIB();
            attr.ATTRINDX = rawValue;

            var model = new OfferAttributeModel(attr);

            Assert.Equal(model.Index, expected);
        }
    }
}
