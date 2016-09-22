using rweClient.SerializationClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Xml.Serialization;
using System.Xml;

namespace rweClient
{
    public class FileToBeDownloaded
    {
        public string Index { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public List<Byte> FileContent { get; set; }
    }

    public class RweClient
    {
        private ZCCH_CACHE_API InitApi()
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
            return api;
        }

        private ZCCH_CACHE_GETResponse GetResponse(string guid, string type)
        {
            var api = InitApi();

            ZCCH_CACHE_GET inputPar = new ZCCH_CACHE_GET();

            inputPar.IV_CCHKEY = guid;
            inputPar.IV_CCHTYPE = type;
            inputPar.IV_GEFILE = "X";

            try
            {
                ZCCH_CACHE_GETResponse result = api.ZCCH_CACHE_GET(inputPar);
                return result;
            }
            catch (WebException wex)
            {
                if (wex.Response == null)
                {
                    //throw;
                    return null;
                }

                try
                {
                    var exceptionResponse = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                    //throw new Exception(exceptionResponse, wex);
                    return null;
                }
                catch
                {
                    //throw;
                    return null;
                }
            }
            catch (Exception)
            {
                //throw;
                return null;
            }
        }

        public List<FileToBeDownloaded> GeneratePDFFiles(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA_PDF");

            List<FileToBeDownloaded> fileResults = new List<FileToBeDownloaded>();

            if (result.ThereAreFiles() && result.ET_FILES.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (var f in result.ET_FILES.OrderBy(x => x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL))
                {
                    FileToBeDownloaded tempItem = new FileToBeDownloaded();
                    tempItem.Index = (++index).ToString();
                    tempItem.FileName = f.FILENAME;
                    tempItem.FileNumber = f.FILEINDX;
                    tempItem.FileContent = f.FILECONTENT.ToList();
                    fileResults.Add(tempItem);
                }

                return fileResults;
            }
            return null;
        }

        public Offer GenerateXml(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA");

            if (result.ThereAreFiles())
            {
                using (var stream = new MemoryStream(result.ET_FILES.First().FILECONTENT, false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Offer));
                    Offer offer = (Offer)serializer.Deserialize(stream);
                    offer.IsAccepted = result.ET_ATTRIB != null && result.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                        && !String.IsNullOrEmpty(result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL);
                    return offer;
                }
            }

            return null;
        }

        public bool AcceptOffer(string guid)
        {
            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                var api = InitApi();
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "5";
                status.IV_TIMESTAMP = outValue;
                var response = api.ZCCH_CACHE_STATUS_SET(status);

                if (response != null && response.ET_RETURN != null && response.ET_RETURN.Any())
                {
                    var responseStatus = response.ET_RETURN.First();
                    return !String.IsNullOrEmpty(responseStatus.MESSAGE);
                }

                return false;
            }
            return false;
        }
    }
}
