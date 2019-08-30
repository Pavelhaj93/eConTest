// <copyright file="EContractingAuthenticationTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{FE43834B-E938-4CA5-9ABB-283149FD26D3}", AutoMap = true)]
    public class EContractingWelcomeTemplate : EContractingTemplate
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
        public virtual string DateOfBirthPlaceholder { get; set; }

        [SitecoreField]
        public virtual string ContractDataPlaceholder { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }

        [SitecoreField]
        public virtual string RequiredFields { get; set; }
    }
}
