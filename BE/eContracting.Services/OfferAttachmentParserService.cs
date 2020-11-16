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
    /// <seealso cref="eContracting.Services.IOfferAttachmentParserService" />
    public class OfferAttachmentParserService : IOfferAttachmentParserService
    {
        /// <inheritdoc/>
        public OfferAttachmentXmlModel[] Parse(OfferModel offer, ZCCH_ST_FILE[] files)
        {
            var list = new List<OfferAttachmentXmlModel>();

            for (int i = 0; i < files.Length; i++)
            {
                var fileName = this.GetFileName(files[i]);
                var uniqueKey = this.GetUniqueKey(files[i]);
                var attrs = this.GetAttributes(files[i]);
                var fileModel = new OfferAttachmentXmlModel(
                uniqueKey: uniqueKey,
                mimeType: files[i].MIMETYPE,
                index: i.ToString(),
                name: fileName,
                signedVersion: false,
                attributes: attrs,
                content: files[i].FILECONTENT);
                list.Add(fileModel);
            }

            return list.ToArray();
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
