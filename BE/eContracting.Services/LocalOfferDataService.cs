using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using eContracting.Models;
using eContracting.Services;
using Sitecore.Collections;

namespace eContracting.Services
{
    /// <summary>
    ///   <para>Local / offline provider for offers. It only reads data, it's not updating them.</para>
    ///   <para>How to run it:</para>
    ///   <para>Create local folder <c>[site root]\App_Data\eContracting\ZCCH_CACHE_API</c> and copy there folders from [git root]/docs/Examples/Versions/1|2|3.</para>
    ///   <para>Create patch file in your running site to use this service (is must be loaded as last!) - see examples. NEVER commit this file to repository!</para>
    ///   <para>If you want to download new offer, run project <c>eContracting.ConsoleClient</c>, write <c>download</c>, enter and put there a guid. If will download it to <c>[git root]/docs/Examples/Versions/1|2|3</c>.</para>
    /// </summary>
    /// <remarks>
    ///   <para>If an offer is expired, you need to overwrite valid date. Go to <c>ZCCH_ST_ATTRIB.xml</c>, find attribute <c>VALID_TO</c> and overwrite <c>ATTRVAL</c> to future date.</para>
    ///   <para>If an offer is accepted, you can invalidate it. Go to <c>ZCCH_ST_ATTRIB.xml</c>, find <c>ATTRID</c> with <c>ACCEPTED_AT</c> and delete parent element <c>ZCCH_ST_ATTRIB</c> from the file.</para>
    /// </remarks>
    /// <example>
    /// Patch file:
    /// <code>
    ///   <configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
    ///     <sitecore>
    ///       <services>
    ///         <register serviceType="eContracting.IOfferDataService, eContracting.Core">
    ///           <patch:attribute name="implementationType">eContracting.Services.LocalOfferDataService, eContracting.Services</patch:attribute>
    ///         </register>
    ///       </services>
    ///     </sitecore>
    ///   </configuration>
    /// </code>
    /// Change valid date for expired offer in <c>ZCCH_ST_ATTRIB.xml</c>:
    /// <code>
    ///   <ZCCH_ST_ATTRIB>
    ///     <ATTRID>VALID_TO</ATTRID>
    ///     <ATTRINDX>000</ATTRINDX>
    ///     <ATTRVAL>20300101</ATTRVAL>
    ///   </ZCCH_ST_ATTRIB>
    /// </code>
    /// Remove accepted date in <c>ZCCH_ST_ATTRIB.xml</c> - delete element <c>ZCCH_ST_ATTRIB</c> where <c>ATTRID</c> has value <c>ACCEPTED_AT</c>.
    /// <code>
    ///   <ZCCH_ST_ATTRIB>
    ///     <ATTRID>ACCEPTED_AT</ATTRID>
    ///     <ATTRINDX>000</ATTRINDX>
    ///     <ATTRVAL>20220816125421</ATTRVAL>
    ///   </ZCCH_ST_ATTRIB>
    /// </code>
    /// </example>
    [ExcludeFromCodeCoverage]
    public class LocalOfferDataService : IOfferDataService
    {
        public string LocalFolder
        {
            get { return this._rootPath; }
            set
            {
                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException($"Directory '{value}' doesn't exist");
                }

                this._rootPath = value;
            }
        }

        private string _rootPath;

        private Regex RegexCoreFile = new Regex("^(BN|OPPT_ZPRA)_[0-9]*\\.xml");
        private Regex RegexAd1File = new Regex("^(BN|OPPT_ZPRA)_[0-9]*_AD1\\.xml");
        private Regex RegexCbFile = new Regex("^(BN|OPPT_ZPRA)_[0-9]*_CB[PE]");

        public LocalOfferDataService()
        {
            this.LocalFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\eContracting\\ZCCH_CACHE_API");
        }

        public ResponseCacheGetModel GetResponse(string guid, OFFER_TYPES type, string fileType = "B")
        {
            if (!Directory.Exists(Path.Combine(this.LocalFolder, guid)))
            {
                return new ResponseCacheGetModel(new ZCCH_CACHE_GETResponse() { EV_RETCODE = 4, ET_RETURN = new BAPIRET2[] { new BAPIRET2() { MESSAGE = "Folder for local offer doesn't exist" } } });
            }

            if (type == OFFER_TYPES.QUOTPRX)
            {
                var data = this.GetOffer(guid);
                return new ResponseCacheGetModel(data);
            }
            else if (type == OFFER_TYPES.QUOTPRX_XML)
            {
                var data = this.GetXmlFiles(guid);
                return new ResponseCacheGetModel(new ZCCH_CACHE_GETResponse() { ET_FILES = data.ToArray() });
            }
            else if (type == OFFER_TYPES.QUOTPRX_ARCH || type == OFFER_TYPES.QUOTPRX_PDF)
            {
                var data = this.GetAttachmentsFiles(guid);
                return new ResponseCacheGetModel(data);
            }
            else
            {
                throw new InvalidOperationException("Cannot get response for " + Enum.GetName(typeof(OFFER_TYPES), type));
            }
        }

        public ResponsePutModel Put(string guid, ZCCH_ST_ATTRIB[] attributes, OfferFileXmlModel[] files)
        {
            return new ResponsePutModel(new ZCCH_CACHE_PUTResponse1() { ZCCH_CACHE_PUTResponse = new ZCCH_CACHE_PUTResponse() });
        }

        public ResponseStatusSetModel SetStatus(string guid, OFFER_TYPES type, decimal timestamp, string status)
        {
            return new ResponseStatusSetModel(new ZCCH_CACHE_STATUS_SETResponse1() { ZCCH_CACHE_STATUS_SETResponse = new ZCCH_CACHE_STATUS_SETResponse() });
        }

        public ResponseAccessCheckModel UserAccessCheck(string guid, string userId, OFFER_TYPES type)
        {
            return new ResponseAccessCheckModel(new ZCCH_CACHE_ACCESS_CHECKResponse());
        }

        protected ZCCH_CACHE_GETResponse GetOffer(string guid)
        {
            var files = this.GetXmlFiles(guid);

            if (files.Length == 0)
            {
                return new ZCCH_CACHE_GETResponse() { EV_RETCODE = 4, ET_RETURN = new BAPIRET2[] { new BAPIRET2() { MESSAGE = "Local offer not found" } } };
            }

            var response = new ZCCH_CACHE_GETResponse();
            response.ET_ATTRIB = this.GetAttributes(guid);
            response.ES_HEADER = this.GetHeader(guid);
            response.ET_FILES = files;
            return response;
        }

        protected ZCCH_CACHE_GETResponse GetAttachmentsFiles(string guid)
        {
            var files = new List<ZCCH_ST_FILE>();
            var folder = new DirectoryInfo(Path.Combine(this.LocalFolder, guid));
            var xmlFiles = folder.GetFiles("*.pdf");

            foreach (var xmlFile in xmlFiles)
            {
                using (var stream = xmlFile.OpenRead())
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    var file = new ZCCH_ST_FILE();
                    file.FILECONTENT = bytes;
                    file.FILENAME = xmlFile.Name;

                    this.LoadMetadata(xmlFile.FullName, file);

                    files.Add(file);
                }
            }

            var response = new ZCCH_CACHE_GETResponse();
            response.ET_FILES = files.ToArray();
            return response;
        }

        protected ZCCH_ST_ATTRIB[] GetAttributes(string guid)
        {
            var attributes = new List<ZCCH_ST_ATTRIB>();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(this.LocalFolder, guid, "ZCCH_ST_ATTRIB.xml"));

            foreach (XmlElement child in xml["body"].ChildNodes)
            {
                var id = child[nameof(ZCCH_ST_ATTRIB.ATTRID)].InnerText;
                var index = child[nameof(ZCCH_ST_ATTRIB.ATTRINDX)].InnerText;
                var value = child[nameof(ZCCH_ST_ATTRIB.ATTRVAL)].InnerText;
                attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = id, ATTRINDX = index, ATTRVAL = value });
            }

            return attributes.ToArray();
        }

        protected ZCCH_ST_HEADER GetHeader(string guid)
        {
            var xml = new XmlDocument();
            xml.Load(Path.Combine(this.LocalFolder, guid, "ZCCH_ST_HEADER.xml"));
            XmlElement body = xml["body"];
            XmlElement header = body["ZCCH_ST_HEADER"];

            var model = new ZCCH_ST_HEADER();
            model.CCHTYPE  = header[nameof(ZCCH_ST_HEADER.CCHTYPE)].InnerText;
            model.CCHSTAT  = header[nameof(ZCCH_ST_HEADER.CCHSTAT)].InnerText;
            model.CCHKEY   = header[nameof(ZCCH_ST_HEADER.CCHKEY)].InnerText;
            model.CCHVALTO = header[nameof(ZCCH_ST_HEADER.CCHVALTO)].InnerText;
            return model;
        }

        protected ZCCH_ST_FILE[] GetXmlFiles(string guid)
        {
            var files = new List<ZCCH_ST_FILE>();
            var folder = new DirectoryInfo(Path.Combine(this.LocalFolder, guid));
            var xmlFiles = folder.GetFiles("*.xml");

            foreach (var xmlFile in xmlFiles)
            {
                if (this.RegexCoreFile.IsMatch(xmlFile.Name) || this.RegexAd1File.IsMatch(xmlFile.Name) || this.RegexCbFile.IsMatch(xmlFile.Name))
                {
                    if (!xmlFile.Name.EndsWith(".METADATA.xml"))
                    {
                        using (var stream = xmlFile.OpenRead())
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);

                            var file = new ZCCH_ST_FILE();
                            file.FILECONTENT = bytes;
                            file.FILENAME = xmlFile.Name;

                            this.LoadMetadata(xmlFile.FullName, file);

                            files.Add(file);
                        }
                    }
                }
            }

            return files.ToArray();
        }

        protected ZCCH_ST_FILE[] GetXmlTextFiles(string guid)
        {
            var files = new List<ZCCH_ST_FILE>();
            var folder = new DirectoryInfo(Path.Combine(this.LocalFolder, guid));
            var xmlFiles = folder.GetFiles("*.xml");

            foreach (var xmlFile in xmlFiles)
            {
                if (this.RegexAd1File.IsMatch(xmlFile.Name) || this.RegexCbFile.IsMatch(xmlFile.Name))
                {
                    if (!xmlFile.Name.EndsWith(".METADATA.xml"))
                    {
                        using (var stream = xmlFile.OpenRead())
                        {
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);

                            var file = new ZCCH_ST_FILE();
                            file.FILECONTENT = bytes;
                            file.FILENAME = xmlFile.Name;

                            this.LoadMetadata(xmlFile.FullName, file);

                            files.Add(file);
                        }
                    }
                }
            }

            return files.ToArray();
        }

        protected void LoadMetadata(string fullFileName, ZCCH_ST_FILE stFile)
        {
            var metadataFile = new FileInfo(fullFileName + ".METADATA.xml");

            if (!metadataFile.Exists)
            {
                return;
            }

            var xml = new XmlDocument();
            xml.Load(metadataFile.FullName);

            XmlElement body = xml["body"];
            stFile.MIMETYPE = body[nameof(ZCCH_ST_FILE.MIMETYPE)].InnerText;
            stFile.FILEINDX = body[nameof(ZCCH_ST_FILE.FILEINDX)].InnerText;
            stFile.FILENAME = body[nameof(ZCCH_ST_FILE.FILENAME)].InnerText;

            var attributes = new List<ZCCH_ST_ATTRIB>();

            foreach (XmlElement child in body["ZCCH_ST_ATTRIBS"].ChildNodes)
            {
                var id = child[nameof(ZCCH_ST_ATTRIB.ATTRID)].InnerText;
                var index = child[nameof(ZCCH_ST_ATTRIB.ATTRINDX)].InnerText;
                var value = child[nameof(ZCCH_ST_ATTRIB.ATTRVAL)].InnerText;
                attributes.Add(new ZCCH_ST_ATTRIB() { ATTRID = id, ATTRINDX = index, ATTRVAL = value });
            }

            stFile.ATTRIB = attributes.ToArray();
        }
    }
}
