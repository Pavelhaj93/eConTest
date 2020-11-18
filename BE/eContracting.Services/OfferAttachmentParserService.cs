using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eContracting.Models;
using eContracting.Services;
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
            if (offer.Attachments.Length == 0)
            {
                this.Logger.Info(offer.Guid, "No attachments available");
                return new OfferAttachmentModel[] { };
            }

            var list = new List<OfferAttachmentModel>();

            for (int i = 0; i < offer.Attachments.Length; i++)
            {
                var attachment = offer.Attachments[i];

                if (string.IsNullOrEmpty(attachment.IdAttach))
                {
                    this.Logger.Fatal(offer.Guid, $"Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {attachment.Description})");
                    continue;
                }

                // if attachment exists in files
                if (attachment.Printed == Constants.FileAttributes.CHECK_VALUE)
                {
                    OfferAttachmentModel item = null;

                    for (int y = 0; y < files.Length; y++)
                    {
                        var file = files[y];
                        var fileIdAttach = this.GetIdAttach(file);

                        if (attachment.IdAttach == fileIdAttach)
                        {
                            var uniqueKey = this.GetUniqueKey(file);
                            var attrs = this.GetAttributes(file);
                            item = new OfferAttachmentModel(attachment, uniqueKey, fileIdAttach, file.MIMETYPE, file.FILENAME, attrs, file.FILECONTENT);
                        }
                    }

                    if (item != null)
                    {
                        list.Add(item);
                    }
                    else
                    {
                        this.Logger.Fatal(offer.Guid, $"File with {Constants.FileAttributes.TYPE} '{attachment.IdAttach}' not found");
                        //throw new ApplicationException($"File with {Constants.FileAttributes.TYPE} '{attachment.IdAttach}' not found");
                        continue;
                    }
                }
                // otherwise this file must be uploaded by user
                else
                {
                    var item = new OfferAttachmentModel(attachment);
                    list.Add(item);
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var idAttach = this.GetIdAttach(file);
                var fileName = this.GetFileName(file);
                var uniqueKey = this.GetUniqueKey(file);
                var attrs = this.GetAttributes(file);

                var fileModel = new OfferAttachmentModel(
                uniqueKey: uniqueKey,
                idAttach: idAttach,
                mimeType: file.MIMETYPE,
                index: i.ToString(),
                fileName: fileName,
                originalFileName: file.FILENAME,
                signedVersion: false,
                attributes: attrs,
                content: file.FILECONTENT);

                list.Add(fileModel);
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

    }
}
