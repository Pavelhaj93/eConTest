// <copyright file="EContractingRichTextTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Content
{
    using System;
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{C7134F38-1CD2-4BF8-A756-C4374A2493B6}", AutoMap = true)]
    public class EContractingRichTextDatasource
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string Text { get; set; }
    }
}
