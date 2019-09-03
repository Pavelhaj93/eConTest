// <copyright file="EContractingAuthenticationTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{FE43834B-E938-4CA5-9ABB-283149FD26D3}", AutoMap = true)]
    public class EContractingWelcomeTemplate : EContractingTemplate
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link NextPageLink { get; set; }
    }
}
