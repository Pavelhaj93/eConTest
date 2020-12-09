using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.HtmlControls;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Storage
{
    public partial class SignedFile
    {
        public SignedFile()
        {
        }

        public SignedFile(string key, string guid, string sessionId, OfferAttachmentModel attachment)
        {
            this.Key = key;
            this.Guid = guid;
            this.SessionId = sessionId;

            var template = JsonConvert.SerializeObject(attachment.DocumentTemplate);
            var attributes = attachment.Attributes != null ? JsonConvert.SerializeObject(attachment.Attributes) : null;

            var file = new File();
            file.FileName = attachment.OriginalFileName;
            file.FileExtension = attachment.FileExtension;
            file.Content = attachment.FileContent;
            file.MimeType = attachment.MimeType;
            file.FileAttributes = new Collection<FileAttribute>();
            file.FileAttributes.Add(new FileAttribute("template", template) { FileId = file.Id });
            file.FileAttributes.Add(new FileAttribute("attributes", attributes) { FileId = file.Id });
            this.File = file;
        }

        /// <summary>
        /// Converts current model to attachment model.
        /// </summary>
        /// <returns>The attachment model.</returns>
        public OfferAttachmentModel ToAttachment()
        {
            var templateValue = this.File.FileAttributes.FirstOrDefault(x => x.Name == "template")?.Value;
            var fileAttributesValue = this.File.FileAttributes.FirstOrDefault(x => x.Name == "attributes")?.Value;
            var template = templateValue != null ? JsonConvert.DeserializeObject<DocumentTemplateModel>(templateValue) : null;
            var fileAttributes = fileAttributesValue != null ? JsonConvert.DeserializeObject<OfferAttributeModel[]>(fileAttributesValue) : null;
            var model = new OfferAttachmentModel(template, this.File.MimeType, this.File.FileName, fileAttributes, this.File.Content);
            return model;
        }

        public SignedFile(DbSignedFileModel dbFileModel)
        {
            this.Id = dbFileModel.Id;
            this.File = new File(dbFileModel.File);
            this.Key = dbFileModel.Key;
            this.Guid = dbFileModel.Guid;
            this.SessionId = dbFileModel.SessionId;
            this.File.Content = dbFileModel.File.Content;
        }

        public DbSignedFileModel ToModel()
        {
            var model = new DbSignedFileModel();
            model.Id = this.Id;
            model.Key = this.Key;
            model.Guid = this.Guid;
            model.SessionId = this.SessionId;
            model.File = new DbFileModel();
            model.File.Id = this.File.Id;
            model.File.FileName = this.File.FileName;
            model.File.FileExtension = this.File.FileExtension;
            model.File.MimeType = this.File.MimeType;
            model.File.Size = this.File.Size;
            model.File.Content = this.File.Content;
            model.File.Attributes = this.File.FileAttributes.Select(x => x.ToModel()).ToArray();
            return model;
        }
    }
}
