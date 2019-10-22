﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.Services;

namespace eContracting.Kernel.Utils
{
    public class AuthenticationRandomChoice : AuthenticationMethod
    {
        private readonly Offer offer;
        public AuthenticationRandomChoice(AuthenticationDataSessionStorage authSessionStorage, Offer offer) : base(authSessionStorage)
        {
            this.offer = offer;
        }

        public override Dictionary<string, string> GetAvailableAuthenticationFields()
        {
            throw new InvalidOperationException("Not available in randome authentication method");
        }

        public override AuthenticationDataItem GetUserData()
        {
            return base.sessionStorage.GetUserData(this.offer, true);
        }
    }
}
