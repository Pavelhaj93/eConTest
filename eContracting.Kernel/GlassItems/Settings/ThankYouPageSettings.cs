// <copyright file="ThankYouPageSettings.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{35E8B33E-A44D-4E2A-A5D0-7BECB710B8F1}", AutoMap = true)]
    public class ThankYouPageSettings
    {
        [SitecoreField]
        public virtual string IndividualLabel { get; set; }

        [SitecoreField]
        public virtual string CampaignLabel { get; set; }

        [SitecoreField]
        public virtual string ElectricityLabel { get; set; }

        [SitecoreField]
        public virtual string GasLabel { get; set; }

        [SitecoreField]
        public virtual string CatText { get; set; }

        [SitecoreField]
        public virtual string ActText { get; set; }

        [SitecoreField]
        public virtual string LabText { get; set; }
    }
}
