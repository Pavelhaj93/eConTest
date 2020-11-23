using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eContracting.Models;
using eContracting.Services;
using Sitecore.Data.Items;
using Sitecore.Pipelines.Save;

namespace eContracting.Services
{
    /// <inheritdoc/>
    /// <seealso cref="IOfferAttachmentParserService" />
    public class OfferAttachmentParserService : IOfferAttachmentParserService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentParserService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">logger</exception>
        public OfferAttachmentParserService(ILogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public OfferAttachmentModel[] Parse(OfferModel offer, ZCCH_ST_FILE[] files)
        {
            if ((offer.Documents?.Length ?? 0) == 0)
            {
                this.Logger.Info(offer.Guid, "No attachments available");
                return new OfferAttachmentModel[] { };
            }

            var list = new List<OfferAttachmentModel>();

            for (int i = 0; i < offer.Documents.Length; i++)
            {
                var template = offer.Documents[i];

                if (string.IsNullOrEmpty(template.IdAttach))
                {
                    this.Logger.Fatal(offer.Guid, $"Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {template.Description})");
                    continue;
                }

                this.MakeCompatible(offer, template);

                var item = this.GetModel(offer, template, files);

                if (item == null)
                {
                    this.Logger.Fatal(offer.Guid, $"File with {Constants.FileAttributes.TYPE} '{template.IdAttach}' not found");
                }
                else
                {
                    list.Add(item);
                }
            }

            return list.ToArray();
        }

        protected internal string GetIdAttach(ZCCH_ST_FILE file)
        {
            return file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE)?.ATTRVAL;
        }

        /// <summary>
        /// Gets readable file name.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>File name to display.</returns>
        protected internal string GetFileName(ZCCH_ST_FILE file)
        {
            var fileName = file.FILENAME;

            if (file.ATTRIB.Any(any => any.ATTRID == Constants.FileAttributes.LABEL))
            {
                var linkLabel = file.ATTRIB.FirstOrDefault(where => where.ATTRID == Constants.FileAttributes.LABEL);
                fileName = linkLabel.ATTRVAL;

                var extension = Path.GetExtension(file.FILENAME);
                fileName = string.Format("{0}{1}", fileName, extension);
            }

            return fileName;
        }

        protected internal OfferAttributeModel[] GetAttributes(ZCCH_ST_FILE file)
        {
            var list = new List<OfferAttributeModel>();

            if (file.ATTRIB?.Length > 0)
            {
                for (int i = 0; i < file.ATTRIB.Length; i++)
                {
                    var attr = file.ATTRIB[i];
                    list.Add(new OfferAttributeModel(attr));
                }
            }

            return list.ToArray();
        }

        protected internal string GetUniqueKey(ZCCH_ST_FILE file)
        {
            var md5 = file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.MD5);

            if (!string.IsNullOrEmpty(md5?.ATTRVAL))
            {
                return md5.ATTRVAL;
            }

            return Utils.GetMd5(file.FILENAME);
        }

        protected internal bool IsNotCompatible(DocumentTemplateModel template)
        {
            return string.IsNullOrEmpty(template.Group);
        }

        protected internal void MakeCompatible(OfferModel offer, DocumentTemplateModel template)
        {
            if (string.IsNullOrEmpty(template.Group))
            {
                template.Group = Constants.OfferDefaults.GROUP;
                this.Logger.Info(offer.Guid, $"Missing value for 'Group' (version {offer.Version}). Set default: '{Constants.OfferDefaults.GROUP}'");
            }

            if (offer.Version == 1)
            {
                if (!template.Printed.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase))
                {
                    template.Printed = Constants.FileAttributeValues.CHECK_VALUE;
                    this.Logger.Info(offer.Guid, $"Missing value for 'Printed' (version {offer.Version}). Set default: '{Constants.FileAttributeValues.CHECK_VALUE}'");
                }
            }
        }

        protected internal OfferAttachmentModel GetModel(OfferModel offer, DocumentTemplateModel template, ZCCH_ST_FILE[] files)
        {
            OfferAttachmentModel item = null;

            // if attachment exists in files
            if (template.Printed == Constants.FileAttributes.CHECK_VALUE)
            {
                for (int y = 0; y < files.Length; y++)
                {
                    var file = files[y];
                    var fileIdAttach = this.GetIdAttach(file);

                    if (template.IdAttach == fileIdAttach)
                    {
                        var uniqueKey = this.GetUniqueKey(file);
                        var attrs = this.GetAttributes(file);
                        item = new OfferAttachmentModel(template, uniqueKey, fileIdAttach, file.MIMETYPE, file.FILENAME, attrs, file.FILECONTENT);
                    }
                }
            }
            // otherwise this file must be uploaded by user
            else
            {
                // this is just a template for file witch is required from a user
                item = new OfferAttachmentModel(template);
            }

            return item;
        }
    }
}
