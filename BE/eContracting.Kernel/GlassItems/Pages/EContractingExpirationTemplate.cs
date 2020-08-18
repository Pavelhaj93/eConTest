// <copyright file="EContractingExpirationTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using eContracting.Kernel.Abstract;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{22D897AA-70B8-436F-8609-53898C556353}", AutoMap = true)]
    public class EContractingExpirationTemplate : EContractingTemplate, IMainText
    {
        [SitecoreField]
        public virtual string MainTextIndividual { get; set; }

        [SitecoreField]
        public virtual string MainTextRetentionIndividual { get; set; }

        [SitecoreField]
        public virtual string MainTextAcquisitionIndividual { get; set; }

        [SitecoreField]
        public virtual string MainTextCampaign { get; set; }

        [SitecoreField]
        public virtual string MainTextRetentionCampaign { get; set; }

        [SitecoreField]
        public virtual string MainTextAcquisitionCampaign { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link1 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link2 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link3 { get; set; }
    }
}
