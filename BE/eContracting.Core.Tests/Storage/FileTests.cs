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
    public class FileTests
    {
        [Fact]
        public void Constructor_Fills_Data_From_DbFileModel()
        {
            var dbModel = new DbFileModel();
            dbModel.Id = 123;
            dbModel.FileName = "customfile";
            dbModel.FileExtension = "pdf";
            dbModel.MimeType = "text/plain";
            dbModel.Content = new byte[1000];
            dbModel.CreateDate = DateTime.UtcNow;

            var model = new File(dbModel);

            Assert.Equal(dbModel.Id, model.Id);
            Assert.Equal(dbModel.FileName, model.FileName);
            Assert.Equal(dbModel.FileExtension, model.FileExtension);
            Assert.Equal(dbModel.MimeType, model.MimeType);
            Assert.Equal(dbModel.Content, model.Content);
            Assert.Equal(dbModel.Content.Length, model.Size);
            Assert.Equal(dbModel.CreateDate, model.CreateDate);
        }

        [Fact]
        public void ToModel_Returns_All_Data_From_File()
        {
            var model = new File();
            model.Id = 123;
            model.FileName = "customfile";
            model.FileExtension = "pdf";
            model.MimeType = "text/plain";
            model.Content = new byte[1000];
            model.CreateDate = DateTime.UtcNow;

            var attributes = new List<FileAttribute>();
            attributes.Add(new FileAttribute() { Id = 1, FileId = 2, Name = "AtName", Value = "AtValue" });
            
            var dbModel = model.ToModel(attributes);

            Assert.Equal(model.Id, dbModel.Id);
            Assert.Equal(model.FileName, dbModel.FileName);
            Assert.Equal(model.FileExtension, dbModel.FileExtension);
            Assert.Equal(model.MimeType, dbModel.MimeType);
            Assert.Equal(model.Content, dbModel.Content);
            Assert.Equal(model.Content.Length, dbModel.Size);
            Assert.Equal(model.CreateDate, dbModel.CreateDate);
            Assert.Equal(attributes.Count, dbModel.Attributes.Count);
        }
    }
}
