using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Storage
{
    public partial class SignedFile
    {
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
            file.FileAttributes.Add(new FileAttribute("template", template));
            file.FileAttributes.Add(new FileAttribute("attributes", attributes));
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
    }
}
