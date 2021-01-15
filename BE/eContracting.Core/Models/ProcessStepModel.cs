﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Model for step model.
    /// </summary>
    /// <seealso cref="eContracting.Models.BaseSitecoreModel" />
    [SitecoreType(TemplateId = "{7691585A-BB36-4EE7-937F-DA4DE7A3EDEC}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ProcessStepModel : BaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [SitecoreField]
        public virtual string Label { get; set; }

        [SitecoreIgnore]
        public bool IsSelected { get; set; }
    }
}