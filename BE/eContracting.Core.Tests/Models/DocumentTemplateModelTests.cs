using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class DocumentTemplateModelTests
    {
        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsSignRequired_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new DocumentTemplateModel();
            template.SignReq = value;

            Assert.True(template.IsSignRequired());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsPrinted_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new DocumentTemplateModel();
            template.Printed = value;

            Assert.True(template.IsPrinted());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsObligatory_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new DocumentTemplateModel();
            template.Obligatory = value;

            Assert.True(template.IsObligatory());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsGroupObligatory_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new DocumentTemplateModel();
            template.GroupObligatory = value;

            Assert.True(template.IsGroupObligatory());
        }

        [Fact]
        public void GetUniqueKey()
        {
            var template = new DocumentTemplateModel();
            template.IdAttach = "ABC";
            template.Group = "ACTUM Digital";
            template.Description = "Just to generate MD5 hash";

            var uniqueyKey = Utils.GetMd5(template.IdAttach + template.Group + template.Description);

            Assert.Equal(uniqueyKey, template.UniqueKey);
        }
    }
}
