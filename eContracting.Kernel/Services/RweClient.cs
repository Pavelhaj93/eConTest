using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Sitecore.Analytics.Data.DataAccess.MongoDb;
using Sitecore.Diagnostics;
using MongoDB.Driver;
using eContracting.Kernel.Utils;

namespace eContracting.Kernel.Services
{
    delegate T CallServiceMethod<T, P>(P inputParams);
    public class RweClient
    {
        #region Public methods
        public List<FileToBeDownloaded> GeneratePDFFiles(string guid)
        {
            var res = GetResponse(guid, "NABIDKA");
            bool IsAccepted = res.ET_ATTRIB != null && res.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT");

            ZCCH_CACHE_GETResponse result = null;
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (IsAccepted)
            {
                result = GetResponse(guid, "NABIDKA_PDF");
                files.AddRange(result.ET_FILES);
                result = GetResponse(guid, "NABIDKA_PRIJ");
                var filenames = result.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.ET_FILES);
            }
            else
            {
                result = GetResponse(guid, "NABIDKA_PDF");
                files.AddRange(result.ET_FILES);
            }

            List<FileToBeDownloaded> fileResults = new List<FileToBeDownloaded>();

            if (files.Count != 0 && files.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (var f in files.OrderBy(x => int.Parse(x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL)))
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

                        FileToBeDownloaded tempItem = new FileToBeDownloaded()
                        {
                            Index = (++index).ToString(),
                            FileName = customisedFileName,
                            FileNumber = f.FILEINDX,
                            FileContent = f.FILECONTENT.ToList(),
                            Guid = guid
                        };
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

        public void RemoveFllesFromMongo()
        {
            AuthenticationDataSessionStorage storage = new AuthenticationDataSessionStorage();
            FilesInSessionCollection.Remove(Query.And(Query.EQ("Guid", (BsonValue)storage.GetUserData().Identifier)));
        }

        public void StoreFilesInDb(List<FileToBeDownloaded> files)
        {
            AuthenticationDataSessionStorage storage = new AuthenticationDataSessionStorage();
            FilesInSessionCollection.Remove(Query.And(Query.EQ("Guid", (BsonValue)storage.GetUserData().Identifier)));
            foreach (var file in files)
            {
                FilesInSessionCollection.Save(file);
            }
        }

        public List<FileToBeDownloaded> GetFilesFromDb()
        {
            AuthenticationDataSessionStorage storage = new AuthenticationDataSessionStorage();
            var ret = FilesInSessionCollection.FindAs<FileToBeDownloaded>(Query.And(Query.EQ("Guid", (BsonValue)storage.GetUserData().Identifier)));
            return ret.ToList();
        }



        private MongoDbCollection AcceptedOfferCollection
        {
            get
            {
                return  MongoDbDriver.FromConnectionString("OfferDB")["AcceptedOffer"];
            }
        }

        private MongoDbCollection FilesInSessionCollection
        {
            get
            {
                return MongoDbDriver.FromConnectionString("OfferDB")["FilesInSession"];
            }
        }


        public MongoCursor<AcceptedOffer> GetNotSentOffers()
        {
            var offersNotsent = AcceptedOfferCollection.FindAs<AcceptedOffer>(Query.And(Query.EQ("SentToService", (BsonValue)false)));
            return offersNotsent;

        }

        public bool GuidExistInMongo(string guid)
        {
            var ao = AcceptedOfferCollection.FindOneAs<AcceptedOffer>(Query.And(Query.EQ("Guid", (BsonValue)guid)));
            return ao != null;
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
                            offer.OfferInternal.AcceptedAt = parsedAcceptedAt.ToString("d.M.yyyy");
                        }
                        else
                        {
                            offer.OfferInternal.AcceptedAt = result.ET_ATTRIB.First(x => x.ATTRID == "ACCEPTED_AT").ATTRVAL;
                        }
                    }
                    

                    offer.OfferInternal.IsAccepted = GuidExistInMongo(guid) ? true: offer.OfferInternal.IsAccepted;
                    return offer;
                }
            }

            return null;
        }

        public bool AcceptOffer(string guid)
        {
            AcceptedOffer offer = new AcceptedOffer()
            {
                Guid = guid,
                SentToService = false
            };

            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "5";
                status.IV_TIMESTAMP = outValue;

                CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);
                var  response = CallService(status, del);
                if (response != null)
                {
                    var responseStatus = response.ET_RETURN.First();
                    if (response != null && response.ET_RETURN != null && response.ET_RETURN.Any())
                    {
                        offer.SentToService = true;
                    }
                }
            }
            AcceptedOfferCollection.Save(offer);
            return offer.SentToService;
        }

        public void ResetOffer(string guid)
        {
            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            Decimal outValue = 1M;

            if (Decimal.TryParse(timestampString, out outValue))
            {
                ZCCH_CACHE_STATUS_SET status = new ZCCH_CACHE_STATUS_SET();
                status.IV_CCHKEY = guid;
                status.IV_CCHTYPE = "NABIDKA";
                status.IV_STAT = "1";
                status.IV_TIMESTAMP = outValue;

                CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET> del = new CallServiceMethod<ZCCH_CACHE_STATUS_SETResponse, ZCCH_CACHE_STATUS_SET>(AcceptOfferDel);
                var response = CallService(status, del);
            }
        }

        public XmlText GetLetterXml(IEnumerable<XmlText> texts)
        {
            return texts.FirstOrDefault();
        }

        public Dictionary<string, string> GetAllAtrributes(XmlText sourceXml)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sourceXml.Text);

            var parameters = doc.DocumentElement.SelectSingleNode("parameters").ChildNodes;
            if (parameters == null)
                return result;
            foreach (XmlNode param in parameters)
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

        #endregion

        #region Private method
        private ZCCH_CACHE_API InitApi()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ZCCH_CACHE_API api = new ZCCH_CACHE_API();

            var userName = Encoding.UTF8.GetString(Convert.FromBase64String(SystemHelpers.ReadConfig("eContracring.ServiceUser")));
            var password = Encoding.UTF8.GetString(Convert.FromBase64String(SystemHelpers.ReadConfig("eContracting.ServicePassword")));
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

        private ZCCH_CACHE_GETResponse GetResponseDel(ZCCH_CACHE_GET inputParam)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.AppendLine("Calling web service with parameters ");
            parameters.AppendFormat("IV_CCHKEY = {0}", inputParam.IV_CCHKEY);
            parameters.AppendLine();
            parameters.AppendFormat("IV_CCHTYPE = {0}", inputParam.IV_CCHTYPE);
            parameters.AppendLine();
            parameters.AppendFormat("IV_GEFILE = ", inputParam.IV_CCHTYPE);
            Log.Info(parameters.ToString(), this);

            using (var api = InitApi())
            {
                ZCCH_CACHE_GETResponse result = api.ZCCH_CACHE_GET(inputParam);
                return result;
            }

        }
        private ZCCH_CACHE_GETResponse GetResponse(string guid, string type)
        {
            ZCCH_CACHE_GET inputPar = new ZCCH_CACHE_GET();
            inputPar.IV_CCHKEY = guid;
            inputPar.IV_CCHTYPE = type;
            inputPar.IV_GEFILE = "X";
            CallServiceMethod<ZCCH_CACHE_GETResponse, ZCCH_CACHE_GET> del = new CallServiceMethod<ZCCH_CACHE_GETResponse, ZCCH_CACHE_GET>(GetResponseDel);
            return CallService(inputPar, del);
        }

        private ZCCH_CACHE_STATUS_SETResponse AcceptOfferDel(ZCCH_CACHE_STATUS_SET inputParam)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.AppendLine("Calling web service with parameters ");
            parameters.AppendFormat("IV_CCHKEY = {0}", inputParam.IV_CCHKEY);
            parameters.AppendLine();
            parameters.AppendFormat("IV_CCHTYPE = {0}", inputParam.IV_CCHTYPE);
            parameters.AppendLine();
            parameters.AppendFormat("IV_STAT = ", inputParam.IV_STAT);
            parameters.AppendLine();
            parameters.AppendFormat("IV_TIMESTAMP = ", inputParam.IV_TIMESTAMP);
            Log.Info(parameters.ToString(), this);

            using (var api = InitApi())
            {
                var response = api.ZCCH_CACHE_STATUS_SET(inputParam);
                return response;
            }
        }

        private T CallService<T, P>(P param, CallServiceMethod<T,P> del)
        {
            T result;
            for (int callCount = 1; callCount <= 15; callCount++)
            {
                try
                {
                    result =del(param);
                    Log.Info("Call of web service was seccessfull", this);
                    return result;
                }
                catch (Exception ex)
                {
                    Log.Error("An exception occurred during calling web service", ex, this);
                    Thread.Sleep(500);
                }
            }
            return default(T);
        }
        #endregion


    }
}
