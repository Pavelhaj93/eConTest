﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{978F7ACC-208E-4926-8313-FB8218500B22}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class PromoBoxModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Text { get; set; }

        [SitecoreField]
        public virtual ColorModel Color { get; set; }

        [SitecoreField]
        public virtual Link Link { get; set; }

        /// <summary>
        /// Gets the color in hexadecimal format with leading <c>#</c>.
        /// </summary>
        public string GetColor()
        {
            var value = this.Color?.Value;

            if (string.IsNullOrEmpty(value))
            {
                value = "64b42d";
            }

            return "#" + value;
        }

        public string GetLinkUrl()
        {
            return this.Link?.Url;
        }

        public string GetLinkText()
        {
            return this.Link?.Text ?? string.Empty;
        }
        public string GetLinkTitle()
        {
            return this.Link?.Title ?? string.Empty;
        }

    }
}
