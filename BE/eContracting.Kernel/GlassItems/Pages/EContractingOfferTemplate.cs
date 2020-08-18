// <copyright file="EContractingOfferTemplate.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.GlassItems.Pages
{
    using eContracting.Kernel.Abstract;
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = "{89052C3A-8D1D-427E-9F4C-24FB58DE21CE}", AutoMap = true)]
    public class EContractingOfferTemplate : EContractingTemplate, IMainText, IVoucherText
    {
        [SitecoreField]
        public virtual string MainTextIndividual { get; set; }

        [SitecoreField]
        public virtual string MainTextRetentionIndividual { get; set; }

        [SitecoreField]
        public virtual string MainTextAcquisitionIndividual { get; set; }
        [SitecoreField]
        public virtual string MainTextCampaign { get; set; }

        [SitecoreField]
        public virtual string MainTextRetentionCampaign { get; set; }

        [SitecoreField]
        public virtual string MainTextAcquisitionCampaign { get; set; }

        [SitecoreField]
        public virtual string RetentionVoucherText { get; set; }

        [SitecoreField]
        public virtual string VoucherText { get; set; }

        [SitecoreField]
        public virtual string AcquistionVoucherText { get; set; }

        [SitecoreField]
        public virtual string GDPRUrl { get; set; }

        [SitecoreField]
        public virtual string AesEncryptKey { get; set; }

        [SitecoreField]
        public virtual string AesEncryptVector { get; set; }
    }
}
