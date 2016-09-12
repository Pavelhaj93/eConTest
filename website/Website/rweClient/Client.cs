using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Web;

namespace rweClient
{
    public class RweClient
    {
        public void TryCall()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            ZCCH_CACHE_API api = new ZCCH_CACHE_API();

            var userName = rweHelpers.ReadConfig("CacheUserName");
            var password = rweHelpers.ReadConfig("CachePassword");

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new InvalidCredentialException("Wrong credentials for cache!");
            }

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(api.Url),
                "Basic",
                new NetworkCredential(userName, password)
            );

            api.Credentials = credentialCache;

            ZCCH_CACHE_GET inputPar = new ZCCH_CACHE_GET();

            inputPar.IV_CCHKEY = "00145EE9475D1ED59CCDD59CA4BF0EB0";
            inputPar.IV_CCHTYPE = "NABIDKA_PDF";
            inputPar.IV_GEFILE = "X";

            try
            {
                ZCCH_CACHE_GETResponse result = api.ZCCH_CACHE_GET(inputPar);

                if (result.ThereAreFiles())
                {
                    foreach (var f in result.ET_FILES)
                    {
                        File.WriteAllBytes(HttpContext.Current.Request.MapPath("~/PDF/" + f.FILENAME), 
                            f.FILECONTENT);
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response == null)
                {
                    throw;
                }

                var exceptionResponse = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                throw new Exception(exceptionResponse, wex);
            }
        }
    }
}
