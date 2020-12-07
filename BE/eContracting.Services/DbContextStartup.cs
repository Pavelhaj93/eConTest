using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using eContracting.Services.Migrations;

namespace eContracting.Services
{
    [ExcludeFromCodeCoverage]
    public class DbContextStartup : IStartup
    {
        /// <inheritdoc/>
        public void Initialize()
        {
            var migration = new MigrateDatabaseToLatestVersion<DbContext, Configuration>();
            Database.SetInitializer(migration);

            using (var db = new DbContext())
            {
                db.Database.Initialize(false);
            }
        }
    }
}
