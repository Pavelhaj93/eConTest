using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    /// <summary>
    /// Database context for eContracting data.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    [ExcludeFromCodeCoverage]
    public class DbContext : System.Data.Entity.DbContext
    {
        //public DbSet<DbLoginAttemptModel> Logins { get; set; }

        /// <summary>
        /// Gets or sets collection of signed files.
        /// </summary>
        public DbSet<DbSignedFileModel> SignedFiles { get; set; }

        /// <summary>
        /// Gets or sets upload group data.
        /// </summary>
        public DbSet<DbUploadGroupFileModel> UploadGroupFiles { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        public DbSet<DbFileModel> Files { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class.
        /// </summary>
        public DbContext() : base("eContractingContext")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
