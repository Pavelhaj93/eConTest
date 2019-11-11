// <copyright file="GeneralSettings.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using System;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    [SitecoreType(TemplateId = "{0D513A12-13CC-48F1-BD97-58109010109A}", AutoMap = true)]
    public class GeneralSettings
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SessionExpired { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link UserBlocked { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link AcceptedOffer { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link WrongUrl { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link OfferExpired { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ThankYou { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SystemError { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Welcome { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Login { get; set; }

        [SitecoreField]
        public virtual string IAmInformed { get; set; }

        [SitecoreField]
        public virtual string IAgree { get; set; }

        [SitecoreField]
        public virtual string Accept { get; set; }

        [SitecoreField]
        public virtual string AppNotAvailable { get; set; }

        [SitecoreField]
        public virtual string IdentityCardNumber { get; set; }

        [SitecoreField]
        public virtual string UsedPostalCode { get; set; }

        [SitecoreField]
        public virtual string PermanentResidencePostalCode { get; set; }

        [SitecoreField]
        public virtual string AccountNumber { get; set; }

        [SitecoreField]
        public virtual string DefaultSalutation { get; set; }

        [SitecoreField]
        public virtual string SignFailure { get; set; }

        [SitecoreField]
        public virtual string DocumentToSign { get; set; }

        [SitecoreField]
        public virtual string DocumentToSignAccepted { get; set; }

        [SitecoreField]
        public virtual string Step1Heading { get; set; }

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
