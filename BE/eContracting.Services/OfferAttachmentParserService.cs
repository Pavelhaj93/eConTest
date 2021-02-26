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
        public OfferAttachmentModel[] Parse(OfferModel offer, OfferFileXmlModel[] files)
        {
            if ((offer.Documents?.Length ?? 0) == 0)
            {
                this.Logger.Warn(offer.Guid, "No attachments available");
                //TODO: return null (fix all related code!)
                return new OfferAttachmentModel[] { };
            }

            this.MakeCompatible(offer, files);
            this.ExcludeDocumentsForAcceptedOffer(offer, files);
            this.Check(offer, files);
            var list = this.GetAttachments(offer, files);

            //if (offer.Documents.Length != list.Length)
            //{
            //    this.Logger.Error(offer.Guid, $"Count of offer document templates and real files in not equal! (templates: {offer.Documents.Length}, files: {list.Count})");
            //}

            return list.ToArray();
        }

        protected internal OfferAttachmentModel[] GetAttachments(OfferModel offer, OfferFileXmlModel[] files)
        {
            var exceptions = new List<Exception>();
            var list = new List<OfferAttachmentModel>();

            for (int i = 0; i < offer.Documents.Length; i++)
            {
                try
                {
                    var template = offer.Documents[i];
                    var item = this.GetModel(offer, template, files);
                    list.Add(item);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("Failed to parse and get all attachment models based on offer and files", exceptions);
            }

            return list.OrderBy(x => x.Position).ToArray();
        }

        /// <inheritdoc/>
        public bool Equals(OfferAttachmentXmlModel template, OfferFileXmlModel file)
        {
            // IDATTACH cannot be empty!
            if (string.IsNullOrWhiteSpace(template.IdAttach))
            {
                return false;
            }

            return template.IdAttach == file.IdAttach;
        }

        /// <inheritdoc/>
        public OfferFileXmlModel GetFileByTemplate(OfferModel offer, OfferAttachmentXmlModel template, OfferFileXmlModel[] files)
        {
            this.Logger.Debug(offer.Guid, $"Trying to match file with {Constants.FileAttributes.TYPE} ({template.IdAttach}) ...");

            var idAttach = files.Where(x => x.IdAttach == template.IdAttach).ToArray();

            if (idAttach.Length == 0)
            {
                var msg = $"No file matches to {Constants.FileAttributes.TYPE} ({template.IdAttach})";
                this.Logger.Warn(offer.Guid, msg);
                throw new EcontractingDataException(new ErrorModel("OAPS-GFBT", msg));
            }

            if (idAttach.Length == 1)
            {
                this.Logger.Debug(offer.Guid, $"1 file matches to {Constants.FileAttributes.TYPE} ({template.IdAttach})");
                return idAttach.First();
            }

            this.Logger.Warn(offer.Guid, $"{idAttach.Length} files with the same {Constants.FileAttributes.TYPE} ({template.IdAttach}) found. Trying to resolve ...");

            if (string.IsNullOrEmpty(template.Product))
            {
                var msg = $"Attachment doens't contain value in {Constants.FileAttributes.PRODUCT} and due to this we are not able to determinate differences between {idAttach.Length} the same files with {Constants.FileAttributes.TYPE} {template.IdAttach}";
                this.Logger.Error(offer.Guid, msg);
                throw new EcontractingDataException(new ErrorModel("OAPS-GFBT", msg));
            }

            var product = idAttach.Where(x => x.Product == template.Product).ToArray();

            if (product.Length == 0)
            {
                var msg = $"No file found with {Constants.FileAttributes.TYPE} = {template.IdAttach} AND {Constants.FileAttributes.PRODUCT} = {template.Product} and due to this we are not able to determinate differences";
                this.Logger.Error(offer.Guid, msg);
                throw new EcontractingDataException(new ErrorModel("OAPS-GFBT", msg));
            }

            if (product.Length > 1)
            {
                var msg = $"{product.Length} files with the same {Constants.FileAttributes.TYPE} ({template.IdAttach}) and {Constants.FileAttributes.PRODUCT} ({template.Product}) exists and due to this we are not able to determinate differences";
                this.Logger.Error(offer.Guid, msg);
                throw new EcontractingDataException(new ErrorModel("OAPS-GFBT", msg));
            }

            this.Logger.Info(offer.Guid, $"{idAttach.Length} files with the same {Constants.FileAttributes.TYPE} ({template.IdAttach}) resolved with {Constants.FileAttributes.PRODUCT} ({template.Product})");

            return product.First();
        }

        /// <inheritdoc/>
        public void MakeCompatible(OfferModel offer, OfferFileXmlModel[] files)
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

                    var file = files.Where(x => x.File.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TYPE && y.ATTRVAL == t.IdAttach) != null).FirstOrDefault();

                    if (file == null)
                    {
                        this.Logger.Info(offer.Guid, $"No file found with {Constants.FileAttributes.TYPE} = '{t.IdAttach}'");

                        // this is because value IDATTACH in template and in a file is not matching
                        if (t.IsSignRequired())
                        {
                            this.Logger.Info(offer.Guid, $"Trying to find file by attribute '{Constants.FileAttributes.TEMPLATE}' with value '{Constants.FileAttributeValues.SIGN_FILE_IDATTACH}'");
                            // find a file by attribute TEMPLATE == "A10"
                            file = files.Where(x => x.File.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TEMPLATE && y.ATTRVAL == Constants.FileAttributeValues.SIGN_FILE_IDATTACH) != null).FirstOrDefault();                           
                        }
                        else
                        {
                            this.Logger.Info(offer.Guid, $"Trying to find file by attribute '{Constants.FileAttributes.TEMPLATE}' with value '{t.IdAttach}'");
                            file = files.Where(x => x.File.ATTRIB.FirstOrDefault(y => y.ATTRID == Constants.FileAttributes.TEMPLATE && y.ATTRVAL == t.IdAttach) != null).FirstOrDefault();
                        }

                        if (file == null)
                        {
                            this.Logger.Fatal(offer.Guid, $"No compatible file found");
                        }
                        else
                        {
                            this.Logger.Info(offer.Guid, "Compatible file found");

                            var attrIdAttach = file.File.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE);

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
                                file.File.ATTRIB = Utils.GetUpdated(file.File.ATTRIB, newAttrIdAttach);
                                this.Logger.Warn(offer.Guid, $"Attribute '{Constants.FileAttributes.TYPE}' not found in file. Created new one with value '{t.IdAttach}' and added to file ATTRID collection.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Excludes <see cref="OfferAttachmentXmlModel"/> from <see cref="OfferModel.Documents"/> for accepted offer when real <see cref="OfferFileXmlModel"/> doesn't exist.
        /// </summary>
        /// <remarks>Accepted offer contains only accepted and signed files.</remarks>
        /// <param name="offer">The offer.</param>
        /// <param name="files">The files.</param>
        protected internal void ExcludeDocumentsForAcceptedOffer(OfferModel offer, OfferFileXmlModel[] files)
        {
            if (offer.IsAccepted)
            {
                var updatedDocuments = new List<OfferAttachmentXmlModel>();

                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];

                    var attachments = offer.Documents.Where(x => x.IdAttach == f.IdAttach).ToArray();

                    if (attachments.Length == 1)
                    {
                        updatedDocuments.Add(attachments.First());
                    }
                    else if (attachments.Length > 1)
                    {
                        var attachments2 = attachments.Where(x => x.Product == f.Product).ToArray();

                        if (attachments2.Length == 0)
                        {
                            this.Logger.Error(offer.Guid, $"Cannot find attachment by {Constants.FileAttributes.TYPE} = {f.IdAttach} and {Constants.FileAttributes.PRODUCT} = {f.Product}");
                        }
                        else if (attachments2.Length == 1)
                        {
                            updatedDocuments.Add(attachments.First());
                        }
                        else
                        {
                            this.Logger.Error(offer.Guid, $"Cannot find attachment by {Constants.FileAttributes.TYPE} = {f.IdAttach} and {Constants.FileAttributes.PRODUCT} = {f.Product}");
                        }
                    }
                    else
                    {
                        this.Logger.Error(offer.Guid, $"Attachment {f.IdAttach} not found but real file exists");
                    }
                }

                var excluded = offer.Documents.Except(updatedDocuments).ToArray();

                for (int i = 0; i < excluded.Length; i++)
                {
                    var t = excluded[i];
                    this.Logger.Info(offer.Guid, $"Attachment {t.IdAttach} ({Constants.FileAttributes.PRODUCT} = {t.Product}) excluded from accepted offer because real file doesn't exist");
                }

                offer.Xml.Content.Body.Attachments = updatedDocuments.ToArray();
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
        protected internal OfferAttributeModel[] GetAttributes(OfferFileXmlModel file)
        {
            var list = new List<OfferAttributeModel>();

            if (file.File.ATTRIB?.Length > 0)
            {
                for (int i = 0; i < file.File.ATTRIB.Length; i++)
                {
                    var attr = file.File.ATTRIB[i];
                    list.Add(new OfferAttributeModel(attr));
                }
            }

            return list.ToArray();
        }

        protected internal void MakeCompatible(OfferModel offer, OfferAttachmentXmlModel template, int index)
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
        protected internal void Check(OfferModel offer, OfferFileXmlModel[] files)
        {
            var exceptions = new List<Exception>();
            var list = new List<OfferFileXmlModel>(files);

            //for (int i = 0; i < offer.Documents.Length; i++)
            //{
            //    try
            //    {
            //        var attachment = offer.Documents[i];

            //        if (string.IsNullOrEmpty(attachment.IdAttach))
            //        {
            //            throw new EcontractingDataException(new ErrorModel("OAPS-MAT", $"Missing {Constants.FileAttributes.TYPE} in attachment collection (filename: {attachment.Description})"));
            //        }

            //        if (!attachment.IsPrinted())
            //        {
            //            this.Logger.Debug(offer.Guid, $"Attachment {attachment.IdAttach} not printed (upload)");
            //            continue;
            //        }

            //        var file = this.GetFileByTemplate(offer, attachment, files);

            //        if (file == null)
            //        {
            //            this.Logger.Fatal(offer.Guid, $"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files");
            //            throw new EcontractingDataException(new ErrorModel("OAPS-CHECK", $"Attachment {attachment.IdAttach} ({attachment.Description}) not found in files"));
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        exceptions.Add(ex);
            //    }
            //}

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    var file = files[i];
                    var idattach = file.IdAttach;
                    var template = file.Template;

                    if (!offer.Documents.Any(x => x.IdAttach == idattach || x.IdAttach == template))
                    {
                        var msg = $"File {file.File.FILENAME} doesn't exist in attachments ({Constants.FileAttributes.TYPE} = {idattach}) ({Constants.FileAttributes.TEMPLATE} = {template})";
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
        protected internal OfferAttachmentModel GetModel(OfferModel offer, OfferAttachmentXmlModel template, OfferFileXmlModel[] files)
        {
            OfferAttachmentModel item = null;
            // if attachment exists in files
            if (template.Printed == Constants.FileAttributes.CHECK_VALUE)
            {
                var file = this.GetFileByTemplate(offer, template, files);
                var attrs = this.GetAttributes(file);
                var fileName = this.GetFileName(offer, template, file);
                item = new OfferAttachmentModel(template, file, fileName, attrs);
            }
            // otherwise this file must be uploaded by user
            else
            {
                // this is just a template for file witch is required from a user
                item = new OfferAttachmentModel(template);
            }

            return item;
        }

        protected internal string GetFileName(OfferModel offer, OfferAttachmentXmlModel template, OfferFileXmlModel file)
        {
            if (offer.Version == 1)
            {
                return this.GetFileName(file.File);
            }

            return template.Description;
        }
    }
}
