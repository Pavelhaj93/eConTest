﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Kernel.Models
{
    public class AuthenticationSettingsItemModel
    {
        public string UserFriendlyName { get; set; }
        public string AuthenticationDFieldName { get; set; }
        public string Hint { get; set; }
    }
}