using System;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems
{
    [SitecoreType(TemplateId = "{5B43AA17-1F2E-4C54-B695-83A1A40A9F1B}", AutoMap = true)]
    public class SiteRootModel
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public string ServiceUrl { get; set; }

        [SitecoreField]
        public string ServiceUser { get; set; }

        [SitecoreField]
        public string ServicePassword { get; set; }

        [SitecoreField]
        public string SigningServiceUrl { get; set; }

        [SitecoreField]
        public string SigningServiceUser { get; set; }

        [SitecoreField]
        public string SigningServicePassword { get; set; }

        [SitecoreField]
        public int MaxFailedAttempts { get; set; }

        [SitecoreField]
        public string DelayAfterFailedAttempts { get; set; }

        /// <summary>
        /// Gets the delay after failed attempts as <see cref="TimeSpan"/>
        /// </summary>
        /// <value>
        /// Parsed value from <see cref="DelayAfterFailedAttempts"/>. If parsing failed, return default value '00:15:00'
        /// </value>
        [SitecoreIgnore]
        public TimeSpan DelayAfterFailedAttemptsTimeSpan
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.DelayAfterFailedAttempts))
                {
                    TimeSpan value;

                    if (TimeSpan.TryParse(this.DelayAfterFailedAttempts, out value))
                    {
                        return value;
                    }
                }

                return new TimeSpan(0, 15, 0);
            }
        }
    }
}
