﻿using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using eContracting.Services.Migrations;
using eContracting.Storage;

namespace eContracting.Services
{
    [ExcludeFromCodeCoverage]
    public class DbContextStartup : IStartup
    {
        protected readonly string ConnectionString;

        public DbContextStartup(ISettingsReaderService settingsReader)
        {
            this.ConnectionString = settingsReader.GetFileCacheStorageConnectionString();
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            var migration = new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>();
            System.Data.Entity.Database.SetInitializer(migration);

            using (var db = new DatabaseContext(this.ConnectionString))
            {
                db.Database.Initialize(false);
            }
        }
    }
}
