// <copyright file="AcceptOfferJob.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{9FE07BF2-93BA-4FB0-954E-0F08E5E9AF7B}", AutoMap = true)]
    public class EContractingSessionExpiredTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string Header { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
