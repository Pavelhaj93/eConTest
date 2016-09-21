using rweClient.SerializationClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Xml.Serialization;

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
        private ZCCH_CACHE_GETResponse GetResponse(string guid, string type)
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
                    throw;
                }

                try
                {
                    var exceptionResponse = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                    throw new Exception(exceptionResponse, wex);
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
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
                    return offer;
                }
            }

            return null;
        }
    }
}
