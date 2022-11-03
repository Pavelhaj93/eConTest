using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Storage;
using Xunit;

namespace eContracting.Core.Tests.Storage
{
    public class FileAttributeTests
    {
        [Fact]
        public void Constructor_Fills_Name_and_Value()
        {
            var name = "My name";
            var value = "My value";

            var model = new FileAttribute(name, value);

            Assert.Equal(name, model.Name);
            Assert.Equal(value, model.Value);
        }

        [Fact]
        public void Constructor_Fills_Data_From_DbFileAttributeModel()
        {
            var dbModel = new DbFileAttributeModel();
            dbModel.Id = 123;
            dbModel.Name = "Db name";
            dbModel.Value = "Db value";

            var model = new FileAttribute(dbModel);

            Assert.Equal(dbModel.Id, model.Id);
            Assert.Equal(dbModel.Name, model.Name);
            Assert.Equal(dbModel.Value, model.Value);
        }

        [Fact]
        public void ToModel_Returns_All_Data()
        {
            var model = new FileAttribute();
            model.Id = 123;
            model.FileId = 456;
            model.Name = "Db name";
            model.Value = "Db value";

            var dbModel = model.ToModel();

            Assert.Equal(model.Id, dbModel.Id);
            Assert.Equal(model.FileId, dbModel.FileId);
            Assert.Equal(model.Name, dbModel.Name);
            Assert.Equal(model.Value, dbModel.Value);
        }
    }
}
