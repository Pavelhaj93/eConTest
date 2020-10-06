﻿using System;
using System.IO;
using System.Linq;
using eContracting.Services.Models;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    /// <inheritdoc/>
    /// <seealso cref="eContracting.Services.IOfferAttachmentParserService" />
    public class OfferAttachmentParserService : IOfferAttachmentParserService
    {
        /// <inheritdoc/>
        public OfferAttachmentXmlModel Parse(ZCCH_ST_FILE file, int index, OfferModel offer)
        {
            var fileName = this.GetFileName(file);

            var signRequired = false;
            var fileType = string.Empty;
            var templAlcId = string.Empty;

            if (offer.OfferType != OfferTypes.Default)
            {
                if (offer.Attachments != null)
                {
                    var fileTemplate = file.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "TEMPLATE");
                    if (fileTemplate != null)
                    {
                        var correspondingAttachment = offer.Attachments.FirstOrDefault(attachment => attachment.IdAttach.ToLower() == fileTemplate.ATTRVAL.ToLower());

                        if (correspondingAttachment != null)
                        {
                            if (correspondingAttachment.SignReq.ToLower() == "x")
                            {
                                signRequired = true;
                                templAlcId = correspondingAttachment.TemplAlcId;
                                fileType = correspondingAttachment.IdAttach;
                            }
                        }
                    }
                }
            }

            var fileModel = new OfferAttachmentXmlModel(index: index.ToString(), number: file.FILEINDX, type: fileType, name: fileName, signRequired: signRequired, tempAlcId: templAlcId, signedVersion: false, content: file.FILECONTENT);

            if (file.ATTRIB?.Length > 0)
            {
                for (int i = 0; i < file.ATTRIB.Length; i++)
                {
                    var attr = file.ATTRIB[i];
                    fileModel.Attributes.Add(new OfferFileAttribute(Convert.ToInt32(attr.ATTRINDX), attr.ATTRID, attr.ATTRVAL));
                }
            }

            return fileModel;
        }

        /// <summary>
        /// Gets readable file name.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>File name to display.</returns>
        protected internal string GetFileName(ZCCH_ST_FILE file)
        {
            var fileName = file.FILENAME;

            if (file.ATTRIB.Any(any => any.ATTRID == "LINK_LABEL"))
            {
                var linkLabel = file.ATTRIB.FirstOrDefault(where => where.ATTRID == "LINK_LABEL");
                fileName = linkLabel.ATTRVAL;

                var extension = Path.GetExtension(file.FILENAME);
                fileName = string.Format("{0}{1}", fileName, extension);
            }

            return fileName;
        }
    }
}
