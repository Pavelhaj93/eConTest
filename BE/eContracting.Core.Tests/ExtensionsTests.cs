using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Glass.Mapper.Sc;
using JSNLog.Infrastructure;
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

        [Theory]
        [InlineData("0,10", 0.1)]
        [InlineData("1 535,00", 0.1)]
        [InlineData("18 573,50", 0.1)]
        public void HasValue_Returns_True(string value, double minValue)
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add(key, value);

            var result = dic.HasValue(key, minValue);
            Assert.True(result);
        }

        [Theory]
        [InlineData("0,00", 0.1)]
        [InlineData("10,49", 10.5)]
        public void HasValue_Returns_False(string value, double minValue)
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add(key, value);

            var result = dic.HasValue(key, minValue);
            Assert.False(result);
        }

        [Fact]
        public void GetDoubleValue_Returns_0_When_Key_Does_Not_Exist()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("A", "value");

            var result = dic.GetDoubleValue("B");

            Assert.Equal(0.0, result);
        }

        [Theory]
        [InlineData("1 234,56", 1234.56)]
        [InlineData("123 456,78", 123456.78)]
        public void GetDoubleValue_Works_With_Spaces_And_Commas(string value, double expected)
        {
            var key = "key";
            var dic = new Dictionary<string, string>();
            dic.Add(key, value);

            var result = dic.GetDoubleValue(key);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetItems_Returns_Collection()
        {
            var path = "/sitecore/content";
            var collection = new List<IBaseSitecoreModel>();
            collection.Add(new Mock<IBaseSitecoreModel>().Object);
            collection.Add(new Mock<IBaseSitecoreModel>().Object);
            collection.Add(new Mock<IBaseSitecoreModel>().Object);

            var mockFolder = new Mock<IFolderItemModel<IBaseSitecoreModel>>();
            mockFolder.Setup(x => x.Children).Returns(collection);

            var mock = new Mock<ISitecoreService>();
            mock.Setup(x => x.GetItem<IFolderItemModel<IBaseSitecoreModel>>(path)).Returns(mockFolder.Object);
            var result = mock.Object.GetItems<IBaseSitecoreModel>(path);

            Assert.Equal(collection, result.ToList());
        }

        [Fact]
        public void GetItems_Returns_Empty_Collection_When_Path_Incorrect()
        {
            var path = "/sitecore/content";
            var collection = new List<IBaseSitecoreModel>();

            var mock = new Mock<ISitecoreService>();
            mock.Setup(x => x.GetItem<IFolderItemModel<IBaseSitecoreModel>>(path)).Returns((IFolderItemModel<IBaseSitecoreModel>)null);
            var result = mock.Object.GetItems<IBaseSitecoreModel>(path);

            Assert.Equal(collection, result.ToList());
        }

        [Fact]
        public void AllowedDocumentTypesList_Returns_All_Items()
        {
            var list = new List<string>();
            list.Add("a");
            list.Add("b");
            list.Add("c");

            var raw = "a, b,c";

            var mock = new Mock<ISiteSettingsModel>();
            mock.Setup(x => x.AllowedDocumentTypes).Returns(raw);

            var result = mock.Object.AllowedDocumentTypesList();

            Assert.Equal(list, result);
        }

        [Fact]
        public void GetFieldValueByName_Returns_Default_When_Extended_Object_Is_Null()
        {
            IBaseSitecoreModel model = null;

            var result = model.GetFieldValueByName<string>("field");

            Assert.Null(result);
        }

        [Fact]
        public void GetFieldValueByName_Returns_Default_When_FieldName_Is_Null()
        {
            var mockModel = new Mock<IBaseSitecoreModel>();

            var result = mockModel.Object.GetFieldValueByName<string>("");

            Assert.Null(result);
        }

        [Fact]
        public void GetFieldValueByName_Returns_Default_When_Property_Doesnt_Exist()
        {
            var mockModel = new Mock<IBaseSitecoreModel>();

            var result = mockModel.Object.GetFieldValueByName<string>("doesnotexist");

            Assert.Null(result);
        }

        [Fact]
        public void GetFieldValueByName_Returns_Value()
        {
            var expected = "eContracting";
            var mockModel = new Mock<IBaseSitecoreModel>();
            mockModel.SetupProperty(x => x.DisplayName, expected);

            var result = mockModel.Object.GetFieldValueByName<string>("DisplayName");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetValueOrDefault_Returns_Value()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("TEXT_PARAMETER", "My value");

            var result = dictionary.GetValueOrDefault("TEXT_PARAMETER");

            Assert.Equal("My value", result);
        }

        [Fact]
        public void GetValueOrDefault_Returns_Default_Value()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("TEXT_PARAMETER", "My value");

            var result = dictionary.GetValueOrDefault("INVALID");

            Assert.Equal(default(string), result);
        }

        [Fact]
        public void IsDifferentDoubleValue_Returns_True()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("LEFT", "10.5");
            dictionary.Add("RIGHT", "5.1");

            var result = dictionary.IsDifferentDoubleValue("LEFT", "RIGHT");

            Assert.True(result);
        }

        [Theory]
        [InlineData("10.5", "10.5")]
        [InlineData("10.5", "10.50")]
        [InlineData("10.50", "10.5")]
        [InlineData("10.5", "10.50000")]
        public void IsDifferentDoubleValue_Returns_False(string left, string right)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("LEFT", left);
            dictionary.Add("RIGHT", right);

            var result = dictionary.IsDifferentDoubleValue("LEFT", "RIGHT");

            Assert.False(result);
        }

        [Fact]
        public void IsDefault_Returns_True_With_Empty_Process()
        {
            var mockDefinition = new Mock<IDefinitionCombinationModel>();
            var definition = mockDefinition.Object;

            var result = definition.IsDefault();

            Assert.True(result);
        }
    }
}
