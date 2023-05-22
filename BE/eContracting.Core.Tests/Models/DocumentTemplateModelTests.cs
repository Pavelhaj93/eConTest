using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Tests;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class DocumentTemplateModelTests : BaseTestClass
    {
        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsSignRequired_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new OfferAttachmentXmlModel();
            template.SignReq = value;

            Assert.True(template.IsSignRequired());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsPrinted_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new OfferAttachmentXmlModel();
            template.Printed = value;

            Assert.True(template.IsPrinted());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsObligatory_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new OfferAttachmentXmlModel();
            template.Obligatory = value;

            Assert.True(template.IsObligatory());
        }

        [Theory]
        [InlineData("X")]
        [InlineData("x")]
        public void IsGroupObligatory_Returns_True_When_Value_Equals_X(string value)
        {
            var template = new OfferAttachmentXmlModel();
            template.GroupObligatory = value;

            Assert.True(template.IsGroupObligatory());
        }
        
        [Fact]
        public void AesEncrypt_Encrypts_String()
        {
            string input = "Suspendisse aliquet at justo quis suscipit.";
            string key = "6c2b62bb9f33433ab76cbd7e6d674ede";
            string vector = "dd4f17ae3fe64821";
            var output = Utils.RijndaelEncrypt(input, key, vector);

            Assert.Equal("eezNXFmGYGq94/rkso9+3WZUeXAMWwAyOkQgYb15+msxI+DgxC7zhquaDMd3VVAE", output);
        }

        //[Fact]
        //public void AesDecrypt_decrypts_object()
        //{
        //    var data = this.CreateCognitoData();
        //    string key = "6c2b62bb9f33433ab76cbd7e6d674ede";
        //    string vector = "dd4f17ae3fe64821";
            

        //    //var output = Utils.AesDecrypt<CognitoUserDataModel>();
        //}

        [Fact]
        public void GetUniqueKey()
        {
            var template = new OfferAttachmentXmlModel();
            template.IdAttach = "ABC";
            template.Group = "ACTUM Digital";
            template.Description = "Just to generate MD5 hash";

            var uniqueyKey = Utils.GetMd5(template.IdAttach + template.Group + template.Description);

            Assert.Equal(uniqueyKey, template.UniqueKey);
        }
    }
}
