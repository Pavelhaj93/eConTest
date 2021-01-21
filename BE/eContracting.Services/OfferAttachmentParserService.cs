using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
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
                this.Logger.Warn(offer.Guid, "No attachments available");
                //TODO: return null (fix all related code!)
                return new OfferAttachmentModel[] { };
            }

            this.MakeCompatible(offer, files);
            this.Check(offer, files);

            var list = new List<OfferAttachmentModel>();

            for (int i = 0; i < offer.Documents.Length; i++)
            {
                var template = offer.Documents[i];

                var item = this.GetModel(offer, template, files);

                // should not happen, it should be handled in Check() method
                if (item == null)
                {
                    throw new EcontractingDataException(new ErrorModel("OAPS-MF", $"File template with {Constants.FileAttributes.TYPE} '{template.IdAttach}' doesn't match to any file"));
                }
                else
                {
                    list.Add(item);
                }
            }

            if (offer.Documents.Length != list.Count)
            {
                this.Logger.Error(offer.Guid, $"Count of offer document templates and real files in not equal! (templates: {offer.Documents.Length}, files: {list.Count})");
            }

            return list.ToArray();
        }
        
        /// <inheritdoc/>
        public bool Equals(DocumentTemplateModel template, ZCCH_ST_FILE file)
        {
            // IDATTACH cannot be empty!
            if (string.IsNullOrWhiteSpace(template.IdAttach))
            {
                return false;
            }

            return template.IdAttach == this.GetIdAttach(file);
        }

        /// <inheritdoc/>
        public ZCCH_ST_FILE GetFileByTemplate(DocumentTemplateModel template, ZCCH_ST_FILE[] files)
        {
            for (int y = 0; y < files.Length; y++)
            {
                var file = files[y];

                if (this.Equals(template, file))
                {
                    return file;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public void MakeCompatible(OfferModel offer, ZCCH_ST_FILE[] files)
        {
            for (int i = 0; i < offer.Documents.Length; i++)
            {
                var template = offer.Documents[i];
                this.MakeCompatible(offer, template, i);
            }

            if (offer.Version == 1)
            {
                for (int i = 0; i < offer.Documents.Length; i++)
                {
                    var t = offer.Documents[i];

                    var file = files.Where(x => x.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TYPE && y.ATTRVAL == t.IdAttach) != null).FirstOrDefault();

                    if (file == null)
                    {
                        this.Logger.Info(offer.Guid, $"No file found with {Constants.FileAttributes.TYPE} = '{t.IdAttach}'");

                        // this is because value IDATTACH in template and in a file is not matching
                        if (t.IsSignRequired())
                        {
                            this.Logger.Info(offer.Guid, $"Trying to find file by attribute '{Constants.FileAttributes.TEMPLATE}' with value '{Constants.FileAttributeValues.SIGN_FILE_IDATTACH}'");
                            // find a file by attribute TEMPLATE == "A10"
                            file = files.Where(x => x.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TEMPLATE && y.ATTRVAL == Constants.FileAttributeValues.SIGN_FILE_IDATTACH) != null).FirstOrDefault();                           
                        }
                        else
                        {
                            this.Logger.Info(offer.Guid, $"Trying to find file by attribute '{Constants.FileAttributes.TEMPLATE}' with value '{t.IdAttach}'");
                            file = files.Where(x => x.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TEMPLATE && y.ATTRVAL == t.IdAttach) != null).FirstOrDefault();
                        }

                        if (file == null)
                        {
                            this.Logger.Fatal(offer.Guid, $"No compatible file found");
                        }
                        else
                        {
                            this.Logger.Info(offer.Guid, "Compatible file found");

                            var attrIdAttach = file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE);

                            if (attrIdAttach != null)
                            {
                                var oldValue = t.IdAttach;
                                t.IdAttach = attrIdAttach.ATTRVAL;
                                this.Logger.Debug(offer.Guid, $"Template attribute '{Constants.FileAttributes.TYPE}' value '{oldValue}' replaced with '{attrIdAttach.ATTRVAL}'");
                            }
                            else
                            {
                                var newAttrIdAttach = new ZCCH_ST_ATTRIB();
                                newAttrIdAttach.ATTRID = Constants.FileAttributes.TYPE;
                                newAttrIdAttach.ATTRVAL = t.IdAttach;
                                file.ATTRIB = Utils.GetUpdated(file.ATTRIB, newAttrIdAttach);
                                this.Logger.Warn(offer.Guid, $"Attribute '{Constants.FileAttributes.TYPE}' not found in file. Created new one with value '{t.IdAttach}' and added to file ATTRID collection.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets IDATTACH value from given <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        protected internal string GetIdAttach(ZCCH_ST_FILE file)
        {
            return file.ATTRIB?.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE)?.ATTRVAL;
        }

        /// <summary>
        /// Gets readable file name.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>File name to display.</returns>
        protected internal string GetFileName(ZCCH_ST_FILE file)
        {
            var fileName = file.FILENAME;

            var attr = file.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.LABEL);

            if (attr != null)
            {
                var extension = Path.GetExtension(file.FILENAME);
                fileName = string.Format("{0}{1}", attr.ATTRVAL, extension);
            }

            return fileName;
        }

        /// <summary>
        /// Gets converted attributes from <see cref="ZCCH_ST_FILE.ATTRIB"/> to collection of <see cref="OfferAttributeModel"/>.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
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

        protected internal void MakeCompatible(OfferModel offer, DocumentTemplateModel template, int index)
        {
            if (string.IsNullOrEmpty(template.Group))
            {
                template.Group = Constants.OfferDefaults.GROUP;
                this.Logger.Info(offer.Guid, $"Missing value for 'Group' (version {offer.Version}). Set default: '{Constants.OfferDefaults.GROUP}'");
            }

            if (offer.Version == 1)
            {
                // in version 1 all files must be set as printed because there were no other variant (fix inconsistent data)
                if (!template.IsPrinted())
                {
                    template.Printed = Constants.FileAttributeValues.CHECK_VALUE;
                    this.Logger.Info(offer.Guid, $"Missing value for 'Printed' (version {offer.Version}). Set default: '{Constants.FileAttributeValues.CHECK_VALUE}'");
                }
            }

            if (string.IsNullOrWhiteSpace(template.ConsentType))
            {
                if (offer.Version == 1)
                {
                    if (template.IsSignRequired())
                    {
                        template.ConsentType = Constants.FileAttributeValues.CONSENT_TYPE_S;
                        return;
                    }
                }

                if (index == 0)
                {
                    template.ConsentType = Constants.FileAttributeValues.CONSENT_TYPE_S;
                }
                else
                {
                    template.ConsentType = Constants.FileAttributeValues.CONSENT_TYPE_P;
                }
            }
        }

        /// <summary>
        /// Checks offer templates and files if they are ready and nothing is wrong.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="files">The files.</param>
        /// <exception cref="EcontractingDataException">
        /// new ErrorModel("OAPS-MAT", $"Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {attachment.Description})")
        /// or
        /// new ErrorModel("OAPS-CHECK", $"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files")
        /// or
        /// new ErrorModel("OAPS-EXF", $"File {file.FILENAME} doesn't exist in attachments (IDATTACH = {idattach})")
        /// </exception>
        /// <exception cref="ErrorModel">
        /// OAPS-MAT - Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {attachment.Description})
        /// or
        /// OAPS-CHECK - Attachment {attachment.IdAttach} ({attachment.Description}) not found in files
        /// or
        /// OAPS-EXF - File {file.FILENAME} doesn't exist in attachments (IDATTACH = {idattach})
        /// </exception>
        /// <exception cref="AggregateException">Check of offer templates and offer files failed</exception>
        protected internal void Check(OfferModel offer, ZCCH_ST_FILE[] files)
        {
            var exceptions = new List<Exception>();
            var list = new List<ZCCH_ST_FILE>(files);

            for (int i = 0; i < offer.Documents.Length; i++)
            {
                try
                {
                    var attachment = offer.Documents[i];

                    if (string.IsNullOrEmpty(attachment.IdAttach))
                    {
                        throw new EcontractingDataException(new ErrorModel("OAPS-MAT", $"Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {attachment.Description})"));
                    }

                    if (!attachment.IsPrinted())
                    {
                        this.Logger.Debug(offer.Guid, $"Attachment {attachment.IdAttach} not printed (upload)");
                        continue;
                    }

                    var file = this.GetFileByTemplate(attachment, files);

                    if (file != null)
                    {
                        this.Logger.Debug(offer.Guid, $"Attachment {attachment.IdAttach} found");
                    }
                    else
                    {
                        this.Logger.Fatal(offer.Guid, $"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files");
                        throw new EcontractingDataException(new ErrorModel("OAPS-CHECK", $"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files"));
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    var file = files[i];
                    var idattach = file.GetIdAttach();
                    var template = file.GetTemlate();

                    if (!offer.Documents.Any(x => x.IdAttach == idattach || x.IdAttach == template))
                    {
                        var msg = $"File {file.FILENAME} doesn't exist in attachments ({Constants.FileAttributes.TYPE} = {idattach}) ({Constants.FileAttributes.TEMPLATE} = {template})";
                        this.Logger.Fatal(offer.Guid, msg);
                        throw new EcontractingDataException(new ErrorModel("OAPS-EXF", msg));
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("Check of offer templates and offer files failed", exceptions);
            }
        }

        /// <summary>
        /// Gets <see cref="OfferAttachmentModel"/> created from a <paramref name="template"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="template">The template.</param>
        /// <param name="files">The files.</param>
        /// <returns>Attachment model or null when matching file not found.</returns>
        protected internal OfferAttachmentModel GetModel(OfferModel offer, DocumentTemplateModel template, ZCCH_ST_FILE[] files)
        {
            OfferAttachmentModel item = null;
            // if attachment exists in files
            if (template.Printed == Constants.FileAttributes.CHECK_VALUE)
            {
                var file = this.GetFileByTemplate(template, files);

                if (file != null)
                {
                    var attrs = this.GetAttributes(file);
                    item = new OfferAttachmentModel(template, file.MIMETYPE, file.FILENAME, attrs, file.FILECONTENT);
                }
            }
            // otherwise this file must be uploaded by user
            else
            {
                // this is just a template for file witch is required from a user
                item = new OfferAttachmentModel(template, null, null, new OfferAttributeModel[] { }, null);
            }

            return item;
        }
    }
}
