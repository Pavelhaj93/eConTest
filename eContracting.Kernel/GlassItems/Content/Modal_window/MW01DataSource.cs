// <copyright file="MW01DataSource.cs" company="Actum">
// Copyright © 2018 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Content.Modal_window
{
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using Sitecore.Data.Items;

    [SitecoreType(TemplateId = "{BB581370-8DB5-4962-AAF3-63EF6A2AE990}", AutoMap = true)]
    public class MW01DataSource
    {
        public Item Item { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Accept_Link { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Cancel_Link { get; set; }

        public string ClientId { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsRetention { get; set; }
    }
}
