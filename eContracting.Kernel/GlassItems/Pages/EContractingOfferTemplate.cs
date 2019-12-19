// <copyright file="EContractingOfferTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{89052C3A-8D1D-427E-9F4C-24FB58DE21CE}", AutoMap = true)]
    public class EContractingOfferTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string MainText { get; set; }

        [SitecoreField]
        public virtual string MainTextRetention { get; set; }

        [SitecoreField]
        public virtual string MainTextAcquisition { get; set; }

        [SitecoreField]
        public virtual string VoucherText { get; set; }

        [SitecoreField]
        public virtual string GDPRUrl { get; set; }

        [SitecoreField]
        public virtual string AesEncryptKey { get; set; }

        [SitecoreField]
        public virtual string AesEncryptVector { get; set; }
    }
}
