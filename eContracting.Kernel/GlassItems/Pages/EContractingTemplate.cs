// <copyright file="EContractingTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using System;
    using eContracting.Kernel.Abstract;
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{05A8A6B2-D3D7-41E5-B9B7-8CB8CBE39488}", AutoMap = true)]
    public class EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string PageTitle { get; set; }
    }
}
