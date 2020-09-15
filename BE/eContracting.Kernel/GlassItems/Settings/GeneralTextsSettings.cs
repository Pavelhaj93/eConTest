// <copyright file="GeneralSettings.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using System;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{19121365-FDB3-4CD1-B1E0-ADA958669D0D}", AutoMap = true)]
    public class GeneralTextsSettings
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreInfo(Type = SitecoreInfoType.Name)]
        public virtual string Name { get; set; }

        [SitecoreField]
        public virtual string SignFailure { get; set; }

        [SitecoreField]
        public virtual string DocumentToSign { get; set; }

        [SitecoreField]
        public virtual string DocumentToSignAccepted { get; set; }

        [SitecoreField]
        public virtual string Step1Heading { get; set; }

        [SitecoreField]
        public virtual string Step1SubHeading { get; set; }

        [SitecoreField]
        public virtual string Step2Heading { get; set; }

        [SitecoreField]
        public virtual string WhySignIsRequired { get; set; }

        [SitecoreField]
        public virtual string SignButton { get; set; }

        [SitecoreField]
        public virtual string HowToSign { get; set; }

        [SitecoreField]
        public virtual string HowToAccept { get; set; }

        [SitecoreField]
        public virtual string SignDocument { get; set; }

        [SitecoreField]
        public virtual string SignRequest { get; set; }

        [SitecoreField]
        public virtual string SignConfirm { get; set; }

        [SitecoreField]
        public virtual string SignDelete { get; set; }
    }
}
