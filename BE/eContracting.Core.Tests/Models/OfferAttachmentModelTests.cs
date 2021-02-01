using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class OfferAttachmentModelTests
    {
        [Fact]
        public void Constructor_Throws_ArgumentNullException_When_Template_Is_Null()
        {
            DocumentTemplateModel template = null;

            Assert.Throws<ArgumentNullException>(() => new OfferAttachmentModel(template, null, null, null, null));
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Name_Of_File_Cannot_Be_Determinated()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = string.Empty;

            Assert.Throws<ArgumentException>(() => new OfferAttachmentModel(template, null, null, null, null));
        }

        [Fact]
        public void Constructor_Sets_Empty_Array_When_Attributes_Are_Null()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();

            var model = new OfferAttachmentModel(template, null, "file.pdf", null, null);

            Assert.True(model.Attributes.Length == 0);
        }

        [Fact]
        public void Constructor_Sets_Empty_Array_When_Content_Is_Null()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();

            var model = new OfferAttachmentModel(template, null, "file.pdf", null, null);

            Assert.True(model.FileContent.Length == 0);
        }

        [Fact]
        public void Constructor_Takes_OriginalFileName_From_Template_Description_When_Missing()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "file";

            var model = new OfferAttachmentModel(template, null, null, null, null);

            Assert.Equal(template.Description, model.OriginalFileName);
        }

        [Fact]
        public void Constructor_Gets_File_Extension_From_Original_Name()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "file";

            var model = new OfferAttachmentModel(template, null, "original.wtf", null, null);

            Assert.Equal("wtf", model.FileExtension);
        }

        [Fact]
        public void Constructor_Compose_FileNameExtension_From_Template_Description_And_OriginalFileName()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "Plná moc";

            var model = new OfferAttachmentModel(template, null, "original.pdf", null, null);

            Assert.Equal("Plná moc.pdf", model.FileNameExtension);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_Group()
        {
            var expected = "1693B279EC6D4045AD8E14A851C93559";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "Plná moc";
            template.Group = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.Group);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_GroupGuid()
        {
            var expected = "1693B279EC6D4045AD8E14A851C93559";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "Plná moc";
            template.ItemGuid = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.GroupGuid);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_FileName()
        {
            var expected = "Plná moc";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.FileName);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_ConsentType()
        {
            var expected = "P";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.ConsentType = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.ConsentType);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_TempAlcId()
        {
            var expected = "CRM037";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.TemplAlcId = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.TemplAlcId);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_IsObligatory()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Obligatory = Constants.FileAttributeValues.CHECK_VALUE;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.True(model.IsObligatory);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_IsGroupObligatory()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.GroupObligatory = Constants.FileAttributeValues.CHECK_VALUE;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.True(model.IsGroupObligatory);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_IsPrinted()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Printed = Constants.FileAttributeValues.CHECK_VALUE;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.True(model.IsPrinted);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_IsSignReq()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.SignReq = Constants.FileAttributeValues.CHECK_VALUE;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.True(model.IsSignReq);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_IdAttach()
        {
            var expected = "A10";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.IdAttach = expected;

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.IdAttach);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_UniqueKey()
        {
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.IdAttach = "A10";
            template.Group = "174169341E844F6EB632A847DBDC7766";
            template.Description = "Plná moc";

            var model = new OfferAttachmentModel(template, null, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(template.UniqueKey, model.UniqueKey);
        }

        [Fact]
        public void Constructor_Correctly_Sets_Property_MimeType()
        {
            var expected = "application/pdf-custom";
            DocumentTemplateModel template = new DocumentTemplateModel();
            template.Description = "Plná moc";

            var model = new OfferAttachmentModel(template, expected, "BN_512312414_A10.pdf", null, null);

            Assert.Equal(expected, model.MimeType);
        }
    }
}
