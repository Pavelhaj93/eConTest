﻿using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data.Items;

namespace eContracting.Kernel.GlassItems.Settings
{
    [SitecoreType(TemplateId = "{5AF8CBA5-E046-4C7C-8504-C2DD52158E8C}", AutoMap = true)]
    public class AuthenticationSettings
    {
        [SitecoreChildren]
        public virtual IEnumerable<AuthenticationSettingItem> AuthenticationSettingItems { get; set; }
    }

    [SitecoreType(TemplateId = "{B6ADD33B-95B2-4E42-AC88-FACB932D83B2}", AutoMap = true)]
    public class AuthenticationSettingItem
    {
        [SitecoreField]
        public virtual string Label { get; set; }

        [SitecoreField]
        public virtual string Key { get; set; }

        [SitecoreField]
        public virtual string HelpText { get; set; }

        [SitecoreField]
        public virtual string Placeholder { get; set; }

        [SitecoreField]
        public virtual bool EnableForDefault { get; set; }

        [SitecoreField]
        public virtual bool EnableForRetention { get; set; }

        [SitecoreField]
        public virtual bool EnableForAcquisition { get; set; }

        [SitecoreField]
        public virtual string ValidationRegex { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }
    }
}
