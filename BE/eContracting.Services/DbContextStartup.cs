using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using eContracting.Services.Migrations;
using eContracting.Storage;

namespace eContracting.Services
{
    [ExcludeFromCodeCoverage]
    public class DbContextStartup : IStartup
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        public DbContextStartup(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            this.Logger.Info(null, "Database migration starting ...");

            var migration = new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>();
            Database.SetInitializer(migration);

            using (var db = new DatabaseContext(Constants.DatabaseContextConnectionStringName))
            {
                db.Database.Initialize(false);

                this.Logger.Info(null, "Database migration finished.");
                //this.UpdateTriggers(db);
            }
        }

        protected void UpdateTriggers(DatabaseContext context)
        {
            this.Logger.Info(null, "Applying triggers ...");

            this.UploadGroups(context);
            this.UploadGroupWithFiles(context);
            this.SignedFiles(context);
            this.Files(context);

            this.Logger.Info(null, "Triggers are done.");
        }

        protected void UploadGroups(DatabaseContext context)
        {
            string sql = ""
+ "ALTER TRIGGER [dbo].[Trigger_UploadGroups_DELETE]"
+ "    ON[dbo].[UploadGroups]"
+ "    FOR DELETE"
+ "    AS"
+ "    BEGIN"
+ "        SET NOCOUNT ON;"
+ "            DELETE FROM[dbo].[UploadGroupOriginalFiles] WHERE[dbo].[UploadGroupOriginalFiles].[GroupId] IN(SELECT[deleted].[Id] FROM[deleted]);"
+ "            END";
            context.Database.ExecuteSqlCommand(sql);

            this.Logger.Info(null, "Trigger 'Trigger_UploadGroups_DELETE' applied");
        }

        protected void UploadGroupWithFiles(DatabaseContext context)
        {
            string sql = ""
+ "ALTER TRIGGER [dbo].[Trigger_UploadGroupOriginalFiles_DELETE]"
+ "    ON[dbo].[UploadGroupOriginalFiles]"
+ "    FOR DELETE"
+ "    AS"
+ "    BEGIN"
+ "        SET NOCOUNT ON;"
+ "            DELETE FROM[dbo].[Files] WHERE[dbo].[Files].[Id] IN(SELECT[deleted].[FileId] FROM[deleted]); "
+ "            END";
            context.Database.ExecuteSqlCommand(sql);

            this.Logger.Info(null, "Trigger 'Trigger_UploadGroupOriginalFiles_DELETE' applied");
        }

        protected void SignedFiles(DatabaseContext context)
        {
            string sql = ""
+ "ALTER TRIGGER [dbo].[Trigger_SignedFiles_DELETE]"
+ "    ON[dbo].[SignedFiles]"
+ "    FOR DELETE"
+ "    AS"
+ "    BEGIN"
+ "        SET NOCOUNT ON; "
+ "            DELETE FROM[dbo].[Files] WHERE[dbo].[Files].[Id] IN(SELECT[deleted].[Id] FROM[deleted]); "
+ "            END";

            context.Database.ExecuteSqlCommand(sql);

            this.Logger.Info(null, "Trigger 'Trigger_SignedFiles_DELETE' applied");
        }

        protected void Files(DatabaseContext context)
        {
            string sql = ""
+ "ALTER TRIGGER[dbo].[Trigger_Files_DELETE]"
+ "            ON[dbo].[Files]"
+ "    FOR DELETE"
+ "    AS"
+ "    BEGIN"
+ "        SET NOCOUNT ON; "
+ "            DELETE FROM[dbo].[FileAttributes] WHERE[dbo].[FileAttributes].[FileId] IN(SELECT[deleted].[Id] FROM[deleted]); "
+ "            END";

            context.Database.ExecuteSqlCommand(sql);

            this.Logger.Info(null, "Trigger 'Trigger_Files_DELETE' applied");
        }
    }
}
