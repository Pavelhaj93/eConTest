using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Storage.Migrations;

namespace eContracting.Storage
{
    [ExcludeFromCodeCoverage]
    public class DatabaseContextStartup : IStartup
    {
        public void Initialize()
        {
            var migration = new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>();
            Database.SetInitializer(migration);

            using (var db = new DatabaseContext())
            {
                db.Database.Initialize(false);
            }
        }
    }
}
