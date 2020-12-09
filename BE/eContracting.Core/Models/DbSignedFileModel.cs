using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Data.Fields;

namespace eContracting.Models
{
    /// <summary>
    /// Represents signed file like 'Plná moc.pdf'
    /// </summary>
    public class DbSignedFileModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the unique offer identifier.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets unique key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public virtual DbFileModel File { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="DbSignedFileModel"/> class from being created.
        /// </summary>
        public DbSignedFileModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSignedFileModel"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="attachment">The attachment.</param>
        public DbSignedFileModel(string key, string guid, string sessionId, OfferAttachmentModel attachment)
        {
            this.Key = key;
            this.Guid = guid;
            this.SessionId = sessionId;

            var template = JsonConvert.SerializeObject(attachment.DocumentTemplate);
            var attributes = attachment.Attributes != null ? JsonConvert.SerializeObject(attachment.Attributes) : null;

            var file = new DbFileModel();
            file.FileName = attachment.OriginalFileName;
            file.FileExtension = attachment.FileExtension;
            file.Content = attachment.FileContent;
            file.MimeType = attachment.MimeType;

            var dbAttributes = new List<DbFileAttributeModel>();
            dbAttributes.Add(new DbFileAttributeModel("template", template));
            dbAttributes.Add(new DbFileAttributeModel("attributes", attributes));

            file.Attributes = dbAttributes.ToArray();

            this.File = file;
        }

        /// <summary>
        /// Converts current model to attachment model.
        /// </summary>
        /// <returns>The attachment model.</returns>
        public OfferAttachmentModel ToAttachment()
        {
            var templateValue = this.File.Attributes.FirstOrDefault(x => x.Name == "template")?.Value;
            var fileAttributesValue = this.File.Attributes.FirstOrDefault(x => x.Name == "attributes")?.Value;
            var template = templateValue != null ? JsonConvert.DeserializeObject<DocumentTemplateModel>(templateValue) : null;
            var fileAttributes = fileAttributesValue != null ? JsonConvert.DeserializeObject<OfferAttributeModel[]>(fileAttributesValue) : null;
            var model = new OfferAttachmentModel(template, this.File.MimeType, this.File.FileName, fileAttributes, this.File.Content);
            return model;
        }
    }
}
