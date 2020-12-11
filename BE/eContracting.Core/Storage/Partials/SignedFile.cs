using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public DbSignedFileModel Model { get; set; }

        public SignedFile()
        {
        }

        public SignedFile(string key, string guid, string sessionId, OfferAttachmentModel attachment)
        {
            this.Key = key;
            this.Guid = guid;
            this.SessionId = sessionId;

            //var template = JsonConvert.SerializeObject(attachment.DocumentTemplate);
            //var attributes = attachment.Attributes != null ? JsonConvert.SerializeObject(attachment.Attributes) : null;

            //var file = new File();
            //file.FileName = attachment.OriginalFileName;
            //file.FileExtension = attachment.FileExtension;
            //file.Content = attachment.FileContent;
            //file.MimeType = attachment.MimeType;
            //file.FileAttributes = new Collection<FileAttribute>();
            //file.FileAttributes.Add(new FileAttribute("template", template) { FileId = file.Id });
            //file.FileAttributes.Add(new FileAttribute("attributes", attributes) { FileId = file.Id });
            //this.File = file;
        }

        public SignedFile(DbSignedFileModel model)
        {
            this.Model = model;
            this.Id = model.Id;
            this.Key = model.Key;
            this.Guid = model.Guid;
            this.SessionId = model.SessionId;
            this.FileId = model.File.Id;
        }

        public DbSignedFileModel ToModel()
        {
            var model = new DbSignedFileModel();
            model.Id = this.Id;
            model.Key = this.Key;
            model.Guid = this.Guid;
            model.SessionId = this.SessionId;
            return model;
        }
    }
}
