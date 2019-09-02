// <copyright file="EContractingAuthenticationTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{0EEAE94D-0018-40C8-A537-5B67349985CD}", AutoMap = true)]
    public class EContractingAuthenticationTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string DateOfBirth { get; set; }

        [SitecoreField]
        public virtual string ContractData { get; set; }

        [SitecoreField]
        public virtual string ButtonText { get; set; }

        [SitecoreField]
        public virtual string AcceptedOfferText { get; set; }

        [SitecoreField]
        public virtual string NotAcceptedOfferText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link NextPageLink { get; set; }

        [SitecoreField]
        public virtual bool WelcomePageEnabled { get; set; }

        [SitecoreField]
        public virtual string DateOfBirthPlaceholder { get; set; }

        [SitecoreField]
        public virtual string ContractDataPlaceholder { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }

        [SitecoreField]
        public virtual string RequiredFields { get; set; }
    }
}
