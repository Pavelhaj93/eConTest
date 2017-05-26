// <copyright file="EContractingAcceptedOfferTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{442E0DAA-1D1B-49D3-B4E8-38B35ABB6AEC}", AutoMap = true)]
    public class EContractingAcceptedOfferTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
