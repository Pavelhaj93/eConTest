using System;
using eContracting.Kernel.GlassItems.Settings;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Presentation;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Kernel.Models
{
    public class LayoutModel : RenderingModel
    {
        public string ImageLinkUrl { get; set; }

        public string PhoneLinkUrl { get; set; }

        public string PhoneLinkUrlText { get; set; }

        public string CopyrightText { get; set; }

        public string EmailLink { get; set; }

        public string EmailText { get; set; }

        public string PhoneLinkLowerUrl { get; set; }

        public string PhoneLinkLowerUrlText { get; set; }

        public string DisclaimerText { get; set; }

        public string DisclaimerLink { get; set; }

        public override void Initialize(Rendering rendering)
        {
            try
            {
                base.Initialize(rendering);

                using (var sitecoreContext = new SitecoreContext())
                {
                    var headerSettings = sitecoreContext.GetItem<HeaderSettings>(ItemPaths.HeaderSettings);
                    var footerSettings = sitecoreContext.GetItem<FooterSettings>(ItemPaths.FooterSettings);

                    if (headerSettings != null)
                    {
                        if (headerSettings.ImageLink != null)
                        {
                            ImageLinkUrl = headerSettings.ImageLink.Url;
                        }

                        PhoneLinkUrl = headerSettings.PhoneNumberLink;
                        PhoneLinkUrlText = headerSettings.PhoneNumber;
                    }

                    if (footerSettings != null)
                    {
                        CopyrightText = footerSettings.CopyrightText;

                        EmailLink = footerSettings.EmailLink;
                        EmailText = footerSettings.EmailText;
                        PhoneLinkLowerUrl = footerSettings.PhoneNumberLinkFooter;
                        PhoneLinkLowerUrlText = footerSettings.PhoneNumberFooter;

                        if (footerSettings.DisclaimerLink != null)
                        {
                            DisclaimerText = footerSettings.DisclaimerLink.Text;
                            DisclaimerLink = footerSettings.DisclaimerLink.Url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when preparing layout model.", ex, this);
            }
        }
    }
}
