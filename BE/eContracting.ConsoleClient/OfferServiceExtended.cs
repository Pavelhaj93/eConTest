using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Consinloop;
using Consinloop.Abstractions;
using eContracting.Services;
using Sitecore.Collections;

namespace eContracting.ConsoleClient
{
    class OfferServiceExtended : OfferService
    {
        public OfferServiceExtended(ILogger logger, IUserFileCacheService userFileCache, ISettingsReaderService settingsReaderService, IServiceFactory factory, IOfferDataService offerDataService, IOfferParserService offerParser, IOfferAttachmentParserService offerAttachmentParser, IDataRequestCacheService cacheService, IContextWrapper contextWrapper) : base(logger, userFileCache, settingsReaderService, factory, offerDataService, offerParser, offerAttachmentParser, cacheService, contextWrapper)
        {
        }

        /// <summary>
        /// Download offer data into folder.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public string[] Download(string guid, string path, IConsole console)
        {
            var filesCreated = new List<string>();

            if (Directory.Exists(guid))
            {
                throw new ApplicationException("Directory for this guid already exists");
            }

            var response = this.DataService.GetResponse(guid, OFFER_TYPES.QUOTPRX);

            if (response.Response.EV_RETCODE > 0)
            {
                throw new ApplicationException(response.Response.ET_RETURN?.FirstOrDefault()?.MESSAGE ?? "Cannot get offer. Return code: " + response.Response.EV_RETCODE);
            }

            var version = this.OfferParser.GetVersion(response.Response);

            path = path.Replace("{version}", $"{version}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                console.WriteLine($"Root director '{path}' for documents created");
            }
            else
            {
                console.WriteLine($"Root path for documents: {path}");
            }

            var dirPath = path + "\\" + guid;

            var dir = new DirectoryInfo(dirPath);

            if (!dir.Exists)
            {
                dir.Create();
                console.WriteLineSuccess($"Directory '{dir.Name}' created");
            }
            else
            {
                console.WriteLineWarning($"Directory '{dir.Name}' already exist");
            }

            var attributes = response.Response.ET_ATTRIB;
            var header = response.Response.ES_HEADER;

            var fileHeader = this.SaveHeader(dirPath, header, console);

            if (fileHeader != null)
            {
                filesCreated.Add(fileHeader);
            }

            var fileAttributes = this.SaveAttributes(dirPath, attributes, console);

            if (fileAttributes != null)
            {
                filesCreated.Add(fileAttributes);
            }

            for (int i = 0; i < response.Response.ET_FILES.Length; i++)
            {
                var f = response.Response.ET_FILES[i];
                var filePath = dirPath + "\\" + f.FILENAME;
                var file = new FileInfo(filePath);

                if (!file.Exists)
                {
                    using (var stream = file.OpenWrite())
                    {
                        stream.Write(f.FILECONTENT, 0, f.FILECONTENT.Length);
                    }

                    filesCreated.Add(filePath);
                    console.WriteLineSuccess($"File '{file.Name}' created");
                }
                else
                {
                    console.WriteLineWarning($"File '{file.Name}' already exists");
                }
                
                this.SaveFileAttributes(dirPath, f, console);
            }

            var files = this.GetFiles(guid, false);

            for (int i = 0; i < files.Length; i++)
            {
                var f = files[i];
                var filePath = dirPath + "\\" + f.File.FILENAME;
                var file = new FileInfo(filePath);

                if (!file.Exists)
                {
                    using (var stream = file.OpenWrite())
                    {
                        stream.Write(f.File.FILECONTENT, 0, f.File.FILECONTENT.Length);
                    }

                    console.WriteLineSuccess($"File '{file.Name}' created");
                    filesCreated.Add(filePath);
                }
                else
                {
                    console.WriteLineWarning($"File '{file.Name}' already exists");
                }

                this.SaveFileAttributes(dirPath, f.File, console);
            }

            return filesCreated.ToArray();
        }

        protected string SaveAttributes(string dirPath, ZCCH_ST_ATTRIB[] attributes, IConsole console)
        {
            var file = new FileInfo(dirPath + "\\" + nameof(ZCCH_ST_ATTRIB) + ".xml");

            if (file.Exists)
            {
                console.WriteLineWarning($"File '{file.Name}' already exists");
                return null;
            }

            var xml = new XmlDocument();
            var xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xml.DocumentElement;
            xml.InsertBefore(xmlDeclaration, root);
            var body = xml.CreateElement(string.Empty, "body", string.Empty);
            //xml.AppendChild(body);
            xml.AppendChild(body);

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];

                var attributeElement = xml.CreateElement(nameof(ZCCH_ST_ATTRIB));
                body.AppendChild(attributeElement);

                var attrid = xml.CreateElement(nameof(attribute.ATTRID));
                attrid.AppendChild(xml.CreateTextNode(attribute.ATTRID));
                attributeElement.AppendChild(attrid);

                var attridx = xml.CreateElement(nameof(attribute.ATTRINDX));
                attridx.AppendChild(xml.CreateTextNode(attribute.ATTRINDX));
                attributeElement.AppendChild(attridx);

                var attrval = xml.CreateElement(nameof(attribute.ATTRVAL));
                attrval.AppendChild(xml.CreateTextNode(attribute.ATTRVAL));
                attributeElement.AppendChild(attrval);
            }

            xml.Save(file.FullName);
            console.WriteLineSuccess($"File '{file.Name}' created");
            return file.FullName;
        }

        protected string SaveFileAttributes(string dirPath, ZCCH_ST_FILE stFile, IConsole console)
        {
            var file = new FileInfo(dirPath + "\\" + stFile.FILENAME + ".METADATA.xml");

            if (file.Exists)
            {
                console.WriteLineWarning($"File '{file.Name}' already exists");
                return null;
            }

            var attributes = stFile.ATTRIB;
            var xml = new XmlDocument();
            var xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xml.DocumentElement;
            xml.InsertBefore(xmlDeclaration, root);
            var body = xml.CreateElement(string.Empty, "body", string.Empty);
            xml.AppendChild(body);

            var mimetype = xml.CreateElement(string.Empty, nameof(stFile.MIMETYPE), string.Empty);
            mimetype.AppendChild(xml.CreateTextNode(stFile.MIMETYPE));
            body.AppendChild(mimetype);

            var fileindex = xml.CreateElement(string.Empty, nameof(stFile.FILEINDX), string.Empty);
            fileindex.AppendChild(xml.CreateTextNode(stFile.FILEINDX));
            body.AppendChild(fileindex);

            var filename = xml.CreateElement(string.Empty, nameof(stFile.FILENAME), string.Empty);
            filename.AppendChild(xml.CreateTextNode(stFile.FILENAME));
            body.AppendChild(filename);

            var attribs = xml.CreateElement(string.Empty, nameof(ZCCH_ST_ATTRIB) + "S", string.Empty);
            body.AppendChild(attribs);

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];

                var attributeElement = xml.CreateElement(nameof(ZCCH_ST_ATTRIB));
                attribs.AppendChild(attributeElement);

                var attrid = xml.CreateElement(nameof(attribute.ATTRID));
                attrid.AppendChild(xml.CreateTextNode(attribute.ATTRID));
                attributeElement.AppendChild(attrid);

                var attridx = xml.CreateElement(nameof(attribute.ATTRINDX));
                attridx.AppendChild(xml.CreateTextNode(attribute.ATTRINDX));
                attributeElement.AppendChild(attridx);

                var attrval = xml.CreateElement(nameof(attribute.ATTRVAL));
                attrval.AppendChild(xml.CreateTextNode(attribute.ATTRVAL));
                attributeElement.AppendChild(attrval);
            }

            xml.Save(file.FullName);
            console.WriteLineSuccess($"File '{file.Name}' created");
            return file.FullName;
        }

        protected string SaveHeader(string dirPath, ZCCH_ST_HEADER header, IConsole console)
        {
            var file = new FileInfo(dirPath + "/" + nameof(ZCCH_ST_HEADER) + ".xml");

            if (file.Exists)
            {
                console.WriteLineWarning($"File '{file.Name}' already exists");
                return null;
            }

            var xml = new XmlDocument();
            var xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xml.DocumentElement;
            xml.InsertBefore(xmlDeclaration, root);
            var body = xml.CreateElement(string.Empty, "body", string.Empty);
            xml.AppendChild(body);

            var headerElement = xml.CreateElement(nameof(ZCCH_ST_HEADER));

            var cchtype = xml.CreateElement(nameof(header.CCHTYPE));
            cchtype.AppendChild(xml.CreateTextNode(header.CCHTYPE));
            headerElement.AppendChild(cchtype);

            var cchstat = xml.CreateElement(nameof(header.CCHSTAT));
            cchstat.AppendChild(xml.CreateTextNode(header.CCHSTAT));
            headerElement.AppendChild(cchstat);

            var cchkey = xml.CreateElement(nameof(header.CCHKEY));
            cchkey.AppendChild(xml.CreateTextNode(header.CCHKEY));
            headerElement.AppendChild(cchkey);

            var cchvalto = xml.CreateElement(nameof(header.CCHVALTO));
            cchvalto.AppendChild(xml.CreateTextNode(header.CCHVALTO));
            headerElement.AppendChild(cchvalto);

            body.AppendChild(headerElement);

            xml.Save(file.FullName);
            console.WriteLineSuccess($"File '{file.Name}' created");
            return file.FullName;
        }
    }
}
