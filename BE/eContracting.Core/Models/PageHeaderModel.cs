using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{B7AA8D70-22CB-47B8-87B4-F070810D168D}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class PageHeaderModel : BaseSitecoreModel
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link ImageLink { get; set; }

        [SitecoreIgnore]
        public string ImageLinkUrl
        {
            get
            {
                return this.ImageLink?.Url;
            }
        }

        [SitecoreField]
        public virtual string PhoneNumber { get; set; }

        [SitecoreField]
        public virtual string PhoneNumberLink { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link LogoutLink { get; set; }

        [SitecoreIgnore]
        public Uri LogoutLinkUrl
        {
            get
            {
                if (Uri.TryCreate(this.LogoutLink?.Url, UriKind.RelativeOrAbsolute, out Uri linkUrl))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(LogoutLink.Query))
                        {
                            if (linkUrl.ToString().Contains("?"))
                                linkUrl = new Uri($"{linkUrl}&{this.LogoutLink.Query}");
                            else
                                linkUrl = new Uri($"{linkUrl}?{this.LogoutLink.Query}");
                        }
                        return linkUrl;
                    }
                    catch (Exception ex)
                    {
                        Sitecore.Diagnostics.Log.Error("PageHeaderModel: Error when processing LogoutLink.", ex, this);
                        return null;
                    }
                }
                else
                { 
                    return null;
                }
            }
        }
    }
}
