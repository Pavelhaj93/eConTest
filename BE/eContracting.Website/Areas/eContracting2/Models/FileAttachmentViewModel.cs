using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class FileAttachmentViewModel
    {
        public readonly string Id;
        public readonly string Label;
        public readonly bool Sign;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachmentViewModel"/> class.
        /// </summary>
        /// <param name="xmlModel">The XML model.</param>
        /// <exception cref="ArgumentNullException">xmlModel</exception>
        public FileAttachmentViewModel(OfferAttachmentXmlModel xmlModel)
        {
            if (xmlModel == null)
            {
                throw new ArgumentNullException(nameof(xmlModel));
            }

            this.Id = xmlModel.Index;
            this.Label = xmlModel.FileName;
            this.Sign = xmlModel.IsSignReq;
        }
    }
}
