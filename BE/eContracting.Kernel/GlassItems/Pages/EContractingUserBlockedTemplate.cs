// <copyright file="EContractingUserBlockedTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{59CCBDB6-6AFE-4630-8EAD-04C42F599B14}", AutoMap = true)]
    public class EContractingUserBlockedTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string Header { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
