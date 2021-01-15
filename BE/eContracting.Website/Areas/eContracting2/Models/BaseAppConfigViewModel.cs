﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class BaseAppConfigViewModel
    {
        [JsonIgnore]
        public string PageTitle { get; set; }

        [JsonProperty("view")]
        public readonly string View;

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("errorPageUrl")]
        public readonly string ErrorPage;

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets value with the specified key to <see cref="Labels"/>.
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>Value in <see cref="Labels"/> by <paramref name="key"/> or null.</returns>
        public string this[string key]
        {
            get
            {
                if (this.Labels.ContainsKey(key))
                {
                    return this.Labels[key];
                }

                return null;
            }
            set
            {
                this.Labels[key] = value;
            }
        }

        protected BaseAppConfigViewModel(string view, ISettingsReaderService settingsReader)
        {
            this.View = view;
            this.ErrorPage = settingsReader.GetSiteSettings().SystemError.Url;
        }
    }
}