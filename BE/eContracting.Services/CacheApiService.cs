using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services.Models;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    /// <summary>
    /// Wrapper over <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class CacheApiService
    {
        public readonly CacheApiServiceOptions Options;

        private readonly ZCCH_CACHE_APIClient Api;

        public static string[] AvailableRequestTypes = new[] { "NABIDKA", "NABIDKA_XML", "NABIDKA_PDF", "NABIDKA_ARCH" };

        public CacheApiService(CacheApiServiceOptions options)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            this.Options = options;

            var binding = new BasicHttpBinding();
            binding.Name = nameof(ZCCH_CACHE_APIClient);
            binding.MaxReceivedMessageSize = 65536 * 100; // this is necessary for "NABIDKA_PDF"
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(this.Options.Url);

            this.Api = new ZCCH_CACHE_APIClient(binding, endpoint);
            this.Api.ClientCredentials.UserName.UserName = this.Options.User;
            this.Api.ClientCredentials.UserName.Password = this.Options.Password;
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Files in array or null</returns>
        public async Task<AttachmentModel[]> GetPdfFilesForOfferAsync(string guid)
        {
            var result = await this.GetResponse(guid, "NABIDKA");

            if (result == null)
            {
                return null;
            }

            var offer = new ResponseCacheGetModel(result);
            bool isAccepted = offer.IsAccepted;

            ZCCH_ST_FILE[] files = await this.GetFiles(guid, isAccepted);

            List<AttachmentModel> fileResults = new List<AttachmentModel>();

            if (files.Length != 0 && files.All(x => x.ATTRIB.Any(y => y.ATTRID == "COUNTER")))
            {
                int index = 0;

                foreach (ZCCH_ST_FILE f in files.OrderBy(x => int.Parse(x.ATTRIB.FirstOrDefault(y => y.ATTRID == "COUNTER").ATTRVAL)))
                {
                    try
                    {
                        var customisedFileName = f.FILENAME;

                        if (f.ATTRIB.Any(any => any.ATTRID == "LINK_LABEL"))
                        {
                            var linkLabel = f.ATTRIB.FirstOrDefault(where => where.ATTRID == "LINK_LABEL");
                            customisedFileName = linkLabel.ATTRVAL;

                            var extension = Path.GetExtension(f.FILENAME);
                            customisedFileName = string.Format("{0}{1}", customisedFileName, extension);
                        }

                        var signRequired = false;
                        var fileType = string.Empty;
                        var templAlcId = string.Empty;

                        if (offer.OfferInternal.Body.OfferType != OfferTypes.Default)
                        {
                            if (offer.OfferInternal.Body.Attachments != null)
                            {
                                var fileTemplate = f.ATTRIB.FirstOrDefault(attribute => attribute.ATTRID == "TEMPLATE");
                                if (fileTemplate != null)
                                {
                                    var correspondingAttachment = offer.OfferInternal.Body.Attachments.FirstOrDefault(attachment => attachment.IdAttach.ToLower() == fileTemplate.ATTRVAL.ToLower());

                                    if (correspondingAttachment != null)
                                    {
                                        if (correspondingAttachment.SignReq.ToLower() == "x")
                                        {
                                            signRequired = true;
                                            templAlcId = correspondingAttachment.TemplAlcId;
                                            fileType = correspondingAttachment.IdAttach;
                                        }
                                    }
                                }
                            }
                        }

                        var tempItem = new FileToBeDownloaded();
                        tempItem.Index = (++index).ToString();
                        tempItem.FileName = customisedFileName;
                        tempItem.FileNumber = f.FILEINDX;
                        tempItem.FileType = fileType;
                        tempItem.TemplAlcId = templAlcId;
                        tempItem.SignRequired = signRequired;
                        tempItem.FileContent = f.FILECONTENT.ToList();
                        tempItem.SignedVersion = false;
                        fileResults.Add(tempItem);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[{guid}] Exception occured when parsing file list", ex, this);
                    }
                }

                this.LogFiles(fileResults, guid, IsAccepted);
                return fileResults;
            }

            this.LogFiles(null, guid, IsAccepted);
            return null;
        }

        public async Task<ZCCH_ST_FILE[]> GetFiles(string guid, bool isAccepted)
        {
            List<ZCCH_ST_FILE> files = new List<ZCCH_ST_FILE>();

            if (isAccepted)
            {
                var result = await this.GetResponse(guid, "NABIDKA_ARCH");
                files.AddRange(result.ET_FILES);
                var filenames = result.ET_FILES.Select(file => file.FILENAME);
                files.RemoveAll(file => filenames.Contains(file.FILENAME));
                files.AddRange(result.ET_FILES);
                //this.LogFiles(files, guid, isAccepted, "NABIDKA_ARCH");
            }
            else
            {
                var result = await GetResponse(guid, "NABIDKA_PDF");

                if (result?.ET_FILES?.Any() ?? false)
                {
                    files.AddRange(result.ET_FILES);
                }

                //this.LogFiles(files, guid, isAccepted, "NABIDKA_PDF");
            }

            return files.ToArray();
        }

        public async Task<ZCCH_CACHE_GETResponse> GetResponse(string guid, string type, string fileType = "B")
        {
            var get = new ZCCH_CACHE_GET();
            get.IV_CCHKEY = guid;
            get.IV_CCHTYPE = type;
            get.IV_GEFILE = fileType;

            var request = new ZCCH_CACHE_GETRequest(get);
            var stop = new Stopwatch();
            stop.Start();
            var response = await this.Api.ZCCH_CACHE_GETAsync(request);
            stop.Stop();
            var time = stop.ElapsedMilliseconds;
            var result = response.ZCCH_CACHE_GETResponse;
            return result;
        }
    }
}
