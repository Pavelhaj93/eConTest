// <copyright file="EContractingAuthenticationTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using eContracting.Kernel.Abstract;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{0EEAE94D-0018-40C8-A537-5B67349985CD}", AutoMap = true)]
    public class EContractingAuthenticationTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string BirthDateLabel { get; set; }

        [SitecoreField]
        public virtual string BirthDatePlaceholder { get; set; }

        [SitecoreField]
        public virtual string VerificationMethodLabel { get; set; }

        [SitecoreField]
        public virtual string ZipCodeLabel { get; set; }
        
        [SitecoreField]
        public virtual string ZipCodePlaceholder { get; set; }

        [SitecoreField]
        public virtual string ZipCodeHelpText { get; set; }

        [SitecoreField]
        public virtual string CustomerNumberLabel { get; set; }

        [SitecoreField]
        public virtual string CustomerNumberPlaceholder { get; set; }

        [SitecoreField]
        public virtual string CustomerNumberHelpText { get; set; }

        [SitecoreField]
        public virtual string CalendarOpen { get; set; }

        [SitecoreField]
        public virtual string CalendarNextMonth { get; set; }

        [SitecoreField]
        public virtual string CalendarPreviousMonth { get; set; }

        [SitecoreField]
        public virtual string CalendarSelectDay { get; set; }

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
        public virtual bool WelcomePageEnabledRetention { get; set; }

        [SitecoreField]
        public virtual bool WelcomePageEnabledAcquisition { get; set; }

        [SitecoreField]
        public virtual string ContractDataPlaceholder { get; set; }

        [SitecoreField]
        public virtual string ValidationMessage { get; set; }

        [SitecoreField]
        public virtual string DateOfBirthValidationMessage { get; set; }

        [SitecoreField]
        public virtual string RequiredFields { get; set; }

        [SitecoreField]
        public virtual bool UserChoiceAuthenticationEnabled { get; set; }

        [SitecoreField]
        public virtual bool UserChoiceAuthenticationEnabledRetention { get; set; }

        [SitecoreField]
        public virtual bool UserChoiceAuthenticationEnabledAcquisition { get; set; }

        [SitecoreField]
        public virtual string ContractSecondPropertyLabel { get; set; }
    }
}
