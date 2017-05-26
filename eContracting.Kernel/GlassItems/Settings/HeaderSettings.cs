// <copyright file="HeaderSettings.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using System;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{1913027B-30FC-4380-B717-119E3007E027}", AutoMap = true)]
    public class HeaderSettings
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ImageLink { get; set; }

        [SitecoreField]
        public virtual string PhoneNumber { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberLink { get; set; }
    }
}
