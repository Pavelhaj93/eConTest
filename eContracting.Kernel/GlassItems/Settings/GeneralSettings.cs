// <copyright file="GeneralSettings.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [SitecoreChildren]
        public virtual IEnumerable<GeneralTextsSettings> Texts { get; set; }

        public GeneralTextsSettings GetTexts(OfferTypes offerType)
        {
            var name = Enum.GetName(typeof(OfferTypes), offerType);
            return this.Texts.FirstOrDefault(x => x.Name == name);
        }

        public string GetSignInFailure(OfferTypes offerType)
        {
            var value = this.SignFailure;
            var texts = this.GetTexts(offerType);

            if (texts != null)
            {
                value = texts.SignFailure;
            }

            return value;
        }
    }
}
