// <copyright file="AcceptOfferJob.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using System;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{5B8D2056-B177-4288-9B89-5E1DF3221197}", AutoMap = true)]
    public class FooterSettings
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string CopyrightText { get; set; }

        [SitecoreField]
        public virtual string EmailText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual string EmailLink { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberFooter { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberLinkFooter { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link DisclaimerLink { get; set; }
    }
}
