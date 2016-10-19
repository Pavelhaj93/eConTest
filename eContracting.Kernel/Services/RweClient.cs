﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Xml.Serialization;
using eContracting.Kernel.Helpers;
using Sitecore.Diagnostics;

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
                    XmlSerializer serializer = new XmlSerializer(typeof(Offer), "http://www.sap.com/abapxml");
                    Offer offer = (Offer)serializer.Deserialize(stream);
                    offer.OfferInternal.IsAccepted = result.ET_ATTRIB != null && result.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT")
                        && !String.IsNullOrEmpty(result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL)
                        && result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL.Any(c => Char.IsDigit(c));
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

        public List<XmlText> GetTextsXml(string guid)
        {
            ZCCH_CACHE_GETResponse result = GetResponse(guid, "NABIDKA_XML");

            if (result.ThereAreFiles())
            {
                List<XmlText> fileResults = new List<XmlText>();

                int index = 0;

                foreach (var f in result.ET_FILES)
                {
                    XmlText tempItem = new XmlText();
                    tempItem.Index = (++index).ToString();
                    tempItem.Text = Encoding.UTF8.GetString(f.FILECONTENT);
                    fileResults.Add(tempItem);
                }

                return fileResults;
            }

            return null;
        }
    }
}
