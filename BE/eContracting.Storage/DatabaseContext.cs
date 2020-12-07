using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Storage.Models;

namespace eContracting.Storage
{
    [ExcludeFromCodeCoverage]
    public class DatabaseContext : DbContext
    {
        //public DbSet<LoginAttemptModel> Logins { get; set; }

        /// <summary>
        /// Gets or sets collection of signed files.
        /// </summary>
        public DbSet<SignedFileModel> SignedFiles { get; set; }

        /// <summary>
        /// Gets or sets upload group data.
        /// </summary>
        public DbSet<UploadGroupFileModel> UploadGroupFiles { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        public DbSet<FileModel> Files { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        public DatabaseContext() : base("eContractingContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
