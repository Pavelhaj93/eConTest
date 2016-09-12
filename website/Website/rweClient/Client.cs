using System;
using System.IO;
using System.Net;

namespace rweClient
{
    public class RweClient
    {
        public void TryCall()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            ZCCH_CACHE_API api = new ZCCH_CACHE_API();

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(api.Url),
                "Basic",
                new NetworkCredential("EEM_SOLMAN", "EEM_SOL2019")
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
                        File.WriteAllBytes(f.FILENAME, f.FILECONTENT);
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
