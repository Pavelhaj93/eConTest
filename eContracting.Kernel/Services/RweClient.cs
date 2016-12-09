using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using eContracting.Kernel.Helpers;
using Sitecore.Diagnostics;
using System.Xml;

namespace eContracting.Kernel.Services
{
    public class RweClient
    {
        private ZCCH_CACHE_API InitApi()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ZCCH_CACHE_API api = new ZCCH_CACHE_API();

            var userName = SystemHelpers.ReadConfig("eContracring.ServiceUser");
            var password = SystemHelpers.ReadConfig("eContracting.ServicePassword");
            api.Url = SystemHelpers.ReadConfig("eContracting.ServiceUrl");

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
            using (var api = InitApi())
            {

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
                catch (Exception ex)
                {
                    Log.Error("Exception occurred when comunicationg with web service", ex, this);
                    return null;
                }
            }
        }

        public List<FileToBeDownloaded> GeneratePDFFiles(string guid)
        {
            var res = GetResponse(guid, "NABIDKA");
            bool IsAccepted = res.ET_ATTRIB != null && res.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT");

            ZCCH_CACHE_GETResponse result = null;
            if(IsAccepted)
            {
                result = GetResponse(guid, "NABIDKA_PRIJ");
            }
            else
            {
                result = GetResponse(guid, "NABIDKA_PDF");
            }

            List<FileToBeDownloaded> fileResults = new List<FileToBeDownloaded>();

            if (result.ThereAreFiles() && result.ET_FILES.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (var f in result.ET_FILES.OrderBy(x => x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL))
                {
                    try
                    {
                        var customisedFileName = f.FILENAME;

                        if (f.ATTRIB.Any(any => any.ATTRID == "Link_label"))
                        {
                            var linkLabel = f.ATTRIB.FirstOrDefault(where => where.ATTRID == "Link_label");
                            customisedFileName = linkLabel.ATTRVAL;

                            var extension = Path.GetExtension(f.FILENAME);
                            customisedFileName = string.Format("{0}{1}", customisedFileName, extension);
                        }

                        FileToBeDownloaded tempItem = new FileToBeDownloaded();
                        tempItem.Index = (++index).ToString();
                        tempItem.FileName = customisedFileName;
                        tempItem.FileNumber = f.FILEINDX;
                        tempItem.FileContent = f.FILECONTENT.ToList();
                        fileResults.Add(tempItem);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception occured when parsing file list", ex, this);
                    }
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
                    XmlSerializer serializer = new XmlSerializer(typeof(Offer), "http://www.sap.com/abapxml");
                    Offer offer = (Offer)serializer.Deserialize(stream);
                    offer.OfferInternal.IsAccepted = result.ET_ATTRIB != null && result.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                        && !String.IsNullOrEmpty(result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
                        && result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => Char.IsDigit(c));

                    if (offer.OfferInternal.IsAccepted)
                    {
                        DateTime parsedAcceptedAt;

                        var acceptedAt = result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Trim();

                        if (DateTime.TryParseExact(acceptedAt, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedAcceptedAt))
                        {
                            var cultureInfo = CultureInfo.CreateSpecificCulture("cs-cz");
                            offer.OfferInternal.AcceptedAt = parsedAcceptedAt.ToString("d.M.yyyy");
                        }
                        else
                        {
                            offer.OfferInternal.AcceptedAt = result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL;
                        }
                    }

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
                using (var api = InitApi())
                {
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
            }
            return false;
        }

        public void ResetOffer(string guid)
        {
            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                using (var api = InitApi())
                {
                    ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                    status.IV_CCHKEY = guid;
                    status.IV_CCHTYPE = "NABIDKA";
                    status.IV_STAT = "1";
                    status.IV_TIMESTAMP = outValue;
                    var response = api.ZCCH_CACHE_STATUS_SET(status);
                }
            }
        }

        public XmlText GetLetterXml(IEnumerable<XmlText> texts)
        {
            return texts.FirstOrDefault();
        }

        public Dictionary<string,string> GetAllAtrributes(XmlText sourceXml)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sourceXml.Text);

            var parameters = doc.DocumentElement.SelectSingleNode("parameters").ChildNodes;

            foreach ( XmlNode param in parameters)
            {
                result.Add(param.Name, param.InnerXml);
            }

            return result;
        }

        public IEnumerable<XmlText> GetTextsXml(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA_XML");

            List<XmlText> fileResults = new List<XmlText>();

            if (result.ThereAreFiles())
            {

                int index = 0;

                foreach (var f in result.ET_FILES)
                {
                    XmlText tempItem = new XmlText();
                    tempItem.Index = (++index).ToString();
                    tempItem.Text = Encoding.UTF8.GetString(f.FILECONTENT);
                    fileResults.Add(tempItem);
                }
            }

            return fileResults;
        }
    }
}
