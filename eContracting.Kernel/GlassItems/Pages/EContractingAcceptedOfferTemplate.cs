// <copyright file="EContractingAcceptedOfferTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using eContracting.Kernel.Abstract;
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{442E0DAA-1D1B-49D3-B4E8-38B35ABB6AEC}", AutoMap = true)]
    public class EContractingAcceptedOfferTemplate : EContractingTemplate, IMainText
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
    }
}
