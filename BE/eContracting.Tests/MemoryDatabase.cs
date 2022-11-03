using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Archiving;
using Sitecore.Data.Clones;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Eventing;
using Sitecore.Data.Items;
using Sitecore.Framework.Data.Blobs;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Workflows;

namespace eContracting.Tests
{
    public class MemoryDatabase : Database
    {
        public override AliasResolver Aliases { get; }
        public override List<string> ArchiveNames { get; }
        public override DataArchives Archives { get; }
        public override DatabaseCaches Caches { get; }
        public override string ConnectionStringName { get; set; }
        public override DataManager DataManager { get; }
        public override DatabaseEngines Engines { get; }
        public override bool HasContentItem { get; }
        public override string Icon { get; set; }
        public override ItemRecords Items { get; }
        public override Language[] Languages { get; }
        public override BranchRecords Branches { get; }
        public override string Name { get; }
        public override DatabaseProperties Properties { get; }
        public override bool Protected { get; set; }
        public override bool PublishVirtualItems { get; set; }
        public override bool ReadOnly { get; set; }
        public override DatabaseRemoteEvents RemoteEvents { get; }
        public override ResourceItems Resources { get; }
        public override bool SecurityEnabled { get; set; }
        public override Item SitecoreItem { get; }
        public override TemplateRecords Templates { get; }
        public override IWorkflowProvider WorkflowProvider { get; set; }
        public override NotificationProvider NotificationProvider { get; set; }
        public override BlobStorage BlobStorage { get; set; }
        protected override DataProviderCollection DataProviders { get; }

        public override bool CleanupDatabase()
        {
            throw new NotImplementedException();
        }

        public override Item CreateItemPath(string path)
        {
            throw new NotImplementedException();
        }

        public override Item CreateItemPath(string path, TemplateItem template)
        {
            throw new NotImplementedException();
        }

        public override Item CreateItemPath(string path, TemplateItem folderTemplate, TemplateItem itemTemplate)
        {
            throw new NotImplementedException();
        }

        public override DataProvider[] GetDataProviders()
        {
            throw new NotImplementedException();
        }

        public override long GetDataSize(int minEntitySize, int maxEntitySize)
        {
            throw new NotImplementedException();
        }

        public override long GetDictionaryEntryCount()
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(ID itemId)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(ID itemId, Language language)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(ID itemId, Language language, Sitecore.Data.Version version)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(string path)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(string path, Language language)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(string path, Language language, Sitecore.Data.Version version)
        {
            throw new NotImplementedException();
        }

        public override Item GetItem(DataUri uri)
        {
            throw new NotImplementedException();
        }

        public override LanguageCollection GetLanguages()
        {
            throw new NotImplementedException();
        }

        public override Item GetRootItem()
        {
            throw new NotImplementedException();
        }

        public override Item GetRootItem(Language language)
        {
            throw new NotImplementedException();
        }

        public override TemplateItem GetTemplate(ID templateId)
        {
            throw new NotImplementedException();
        }

        public override TemplateItem GetTemplate(string fullName)
        {
            throw new NotImplementedException();
        }

        public override Item[] SelectItems(string query)
        {
            throw new NotImplementedException();
        }

        public override ItemList SelectItemsUsingXPath(string query)
        {
            throw new NotImplementedException();
        }

        public override Item SelectSingleItem(string query)
        {
            throw new NotImplementedException();
        }

        public override Item SelectSingleItemUsingXPath(string query)
        {
            throw new NotImplementedException();
        }
    }
}
