﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ThankYouViewModel
    {
        [JsonIgnore]
        public readonly IPageThankYouModel Datasource;

        [JsonIgnore]
        public readonly StepsViewModel Steps;

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonIgnore]
        public string AbMatrixCombinationPixelUrl { get; set; }

        public GoogleAnalyticsEvendDataModel ScriptParameters { get; set; }

        public ThankYouViewModel(IPageThankYouModel datasource, StepsViewModel steps)
        {
            this.Datasource = datasource;
            this.Steps = steps;
        }
    }
}
