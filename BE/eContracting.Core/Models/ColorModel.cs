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
    /// A color definition used from RWE collection <c>/sitecore/system/Modules/innogy/components/code-lists/color-themes</c>.
    /// </summary>
    /// <seealso cref="eContracting.Models.BaseSitecoreModel" />
    [SitecoreType(TemplateId = "{17F5F5DD-9FDB-43BE-BAD9-815BC9BA5496}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ColorModel : BaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets hexadecimal value.
        /// </summary>
        [SitecoreField("Color Code")]
        public virtual string Value { get; set; }
    }
}
