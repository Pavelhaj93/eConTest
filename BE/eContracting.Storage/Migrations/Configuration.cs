using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Storage.Migrations
{
    [ExcludeFromCodeCoverage]
    public sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            // migration proceed in Sitecore initialize pipeline
            this.AutomaticMigrationsEnabled = true;
            // temp data, don't care
            this.AutomaticMigrationDataLossAllowed = true;
            // if we would need to run it manually, it should solve issue with local migratins on diff computers
            this.ContextKey = "eContracting_" + Environment.MachineName;
            this.MigrationsDirectory = "Migrations";
        }

        protected override void Seed(DatabaseContext context)
        {
            //base.Seed(context);
        }
    }
}
