// <copyright file="MW01DataSource.cs" company="Actum">
// Copyright © 2018 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Content.Modal_window
{
    using System;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{BB581370-8DB5-4962-AAF3-63EF6A2AE990}", AutoMap = true)]
    public class MW01DataSource
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string Text { get; set; }

        [SitecoreField]
        public virtual string Title { get; set; }

        [SitecoreField]
        public virtual string Tooltip { get; set; }

        [SitecoreField]
        public virtual string Accept_Text { get; set; }

        [SitecoreField]
        public virtual string Cancel_Text { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Accept_Link { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Cancel_Link { get; set; }

        public string ClientId { get; set; }

        public bool IsAccepted { get; set; }
    }
}
