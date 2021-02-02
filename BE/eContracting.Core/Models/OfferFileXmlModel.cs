using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Models
{
    /// <summary>
    /// Wrapper over <see cref="ZCCH_ST_FILE"/> with extra properties.
    /// </summary>
    public class OfferFileXmlModel
    {
        /// <summary>
        /// Real file from SAP. Cannot be null.
        /// </summary>
        public readonly ZCCH_ST_FILE File;

        /// <summary>
        /// Gets or sets related <see cref="OfferAttachmentXmlModel"/> to this file.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        public OfferAttachmentXmlModel Attachment { get; set; }

        /// <summary>
        /// Gets value of IDATTACH from the <see cref="ZCCH_ST_FILE.ATTRIB"/> attributes.
        /// </summary>
        /// <returns>Value or null.</returns>
        public string IdAttach
        {
            get
            {
                return this.File.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TYPE)?.ATTRVAL;
            }
        }

        /// <summary>
        /// Gets value of TEMPLATE from the <see cref="ZCCH_ST_FILE.ATTRIB"/> attributes.
        /// </summary>
        /// <returns>Value or null</returns>
        public string Template
        {
            get
            {
                return this.File.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.FileAttributes.TEMPLATE)?.ATTRVAL;
            }
        }

        /// <summary>
        /// Gets the counter integer value from <see cref="ZCCH_ST_FILE.ATTRIB"/>. If attribute <see cref="Constants.OfferAttributes.COUNTER"/> is not valid, returns 100;
        /// </summary>
        /// <returns>Position from <see cref="Constants.OfferAttributes.COUNTER"/>, otherwise 100</returns>
        public int Counter
        {
            get
            {
                var attr = this.File.ATTRIB.FirstOrDefault(x => x.ATTRID == Constants.OfferAttributes.COUNTER);

                if (attr != null && int.TryParse(attr.ATTRVAL, out int position))
                {
                    return position;
                }

                return Constants.FileAttributeDefaults.COUNTER;
            }
        }

        /// <summary>
        /// Gets the raw XML from <see cref="ZCCH_ST_FILE.FILECONTENT"/>.
        /// </summary>
        /// <returns>String representation of <see cref="ZCCH_ST_FILE.FILECONTENT"/> or null.</returns>
        public string GetRawXml()
        {
            if ((this.File.FILECONTENT?.Length ?? 0) < 1)
            {
                return null;
            }

            return Encoding.UTF8.GetString(this.File.FILECONTENT);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferFileXmlModel"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="ArgumentNullException">file</exception>
        public OfferFileXmlModel(ZCCH_ST_FILE file)
        {
            this.File = file ?? throw new ArgumentNullException(nameof(file));
        }
    }
}
