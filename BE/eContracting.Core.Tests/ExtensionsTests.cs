using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Moq;
using Xunit;

namespace eContracting.Core.Tests
{
    public class ExtensionsTests
    {
        //[Fact]
        //public void GetItems_Returns_Subitems()
        //{
        //    var mockSitecoreContext = new Mock<ISitecoreContextExtended>();
        //    mockSitecoreContext.Setup(x => x.GetItem<FolderItemModel<ProcessStepModel>>(parentPath, false, false)).Returns(new FolderItemModel<ProcessStepModel>(steps));
        //    mockSitecoreContext.Setup(x => x.GetItems<ProcessStepModel>(parentPath)).Returns(steps);
        //}
        
        [Fact]
        public void Merge_Extends_Current_Dictionary()
        {
            var first = new Dictionary<string, string>();
            first["a"] = "aa";
            var second = new Dictionary<string, string>();
            second["b"] = "bb";

            first.Merge(second);

            Assert.True(first.ContainsKey("b"));
        }

        [Fact]
        public void Merge_Overwrites_Current_Dictionary()
        {
            var first = new Dictionary<string, string>();
            first["a"] = "peter";
            first["b"] = "john";
            var second = new Dictionary<string, string>();
            second["b"] = "lucy";

            first.Merge(second);

            Assert.True(first.ContainsKey("b"));
            Assert.Equal("lucy", first["b"]);
        }

        [Fact]
        public void Merge_Do_Nothing_When_Source_Dictionary_Is_Null()
        {
            Dictionary<string, string> first = null;
            var second = new Dictionary<string, string>();

            first.Merge(second);

            Assert.Null(first);
        }

        [Fact]
        public void Merge_Do_Nothing_When_Additional_Dictionary_Is_Null()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            Dictionary<string, string> second = null;

            first.Merge(second);

            Assert.NotNull(first);
        }

        [Fact]
        public void Merge_Do_Nothing_When_Both_Dictionaryies_Are_Null()
        {
            Dictionary<string, string> first = null;
            Dictionary<string, string> second = null;

            first.Merge(second);

            Assert.Null(first);
        }

        [Fact]
        public void HasValue_Returns_False_When_Dictionary_Is_Null()
        {
            var key = "key";
            Dictionary<string, string> dic = null;

            var result = dic.HasValue(key);

            Assert.False(result);
        }

        [Fact]
        public void HasValue_Returns_False_When_Dictionary_Is_Empty()
        {
            var key = "key";
            var dic = new Dictionary<string, string>();

            var result = dic.HasValue(key);

            Assert.False(result);
        }

        [Fact]
        public void HasValue_Returns_False_When_Dictionary_Doesnt_Contain_Given_Key()
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add("a", "custom");

            var result = dic.HasValue(key);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void HasValue_Returns_False_When_Value_Found_By_Given_Key_Is_Empty(string value)
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add(key, value);

            var result = dic.HasValue(key);

            Assert.False(result);
        }

        [Fact]
        public void HasValue_Returns_True_When_Value_Found_By_Given_Key_Is_Not_Empty()
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add(key, "x");

            var result = dic.HasValue(key);

            Assert.True(result);
        }
    }
}
