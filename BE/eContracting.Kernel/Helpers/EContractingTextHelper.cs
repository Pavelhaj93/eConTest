using System;
using System.Collections.Generic;
using eContracting.Kernel.Abstract;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;

namespace eContracting.Kernel.Helpers
{
    public class EContractingTextHelper
    {
        public Func<AuthenticationDataItem, IDictionary<string, string>, string, string> CallBackTextCreatorWithParameters { get; private set; }
        public Func<IRweClient, AuthenticationDataItem, string, string, GeneralSettings, string> CallBackTextCreatorNoParameters { get; private set; }
        public IDictionary<string, string> Parameters { get; private set; }

        public EContractingTextHelper(Func<AuthenticationDataItem, IDictionary<string, string>, string, string> callBack, IDictionary<string,string> parameters)
        {
            this.CallBackTextCreatorWithParameters = callBack;
            this.Parameters = parameters;
        }

        public EContractingTextHelper(Func<IRweClient, AuthenticationDataItem, string, string, GeneralSettings, string> callBack)
        {
            this.CallBackTextCreatorNoParameters = callBack;
        }

        public string GetVoucherText(IVoucherText context, AuthenticationDataItem data, GeneralSettings generalSettings)
        {
            if (!data.HasVoucher)
            {
                return null;
            }

            if (data.OfferType == OfferTypes.Default)
            {
                return this.CallBackTextCreatorWithParameters(data, this.Parameters, context.VoucherText);
            }

            if (data.OfferType == OfferTypes.Acquisition)
            {
                return this.CallBackTextCreatorWithParameters(data, this.Parameters, context.AcquistionVoucherText);
            }

            if (data.OfferType == OfferTypes.Retention)
            {
                return this.CallBackTextCreatorWithParameters(data, this.Parameters, context.RetentionVoucherText);
            }

            return null;
        }

        public string GetMainText(IRweClient client, AuthenticationDataItem data, string contentText, GeneralSettings generalSettings)
        {
            if (!string.IsNullOrEmpty(contentText))
            {
                return this.CallBackTextCreatorNoParameters(client, data, contentText, string.Empty, generalSettings);
            }

            return null;
        }

        public string GetMainText(IRweClient client, IMainText context, AuthenticationDataItem data, GeneralSettings generalSettings)
        {
            string text = null;

            if (data.OfferType == OfferTypes.Retention)
            {
                text = data.IsIndi ? context.MainTextRetentionIndividual : context.MainTextRetentionCampaign;
            }

            if (data.OfferType == OfferTypes.Acquisition)
            {
                text = data.IsIndi ? context.MainTextAcquisitionIndividual : context.MainTextAcquisitionCampaign;
            }

            if (data.OfferType == OfferTypes.Default)
            {
                text = data.IsIndi ? context.MainTextIndividual : context.MainTextCampaign;
            }

            return this.Parameters==null ?
                this.CallBackTextCreatorNoParameters(client, data, text, string.Empty, generalSettings) : 
                this.CallBackTextCreatorWithParameters(data, this.Parameters, text);
        }
    }
}
