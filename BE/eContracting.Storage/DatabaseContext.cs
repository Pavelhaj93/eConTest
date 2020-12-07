using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Storage.Models;

namespace eContracting.Storage
{
    public class DatabaseContext : DbContext
    {
        public DbSet<LoginAttemptModel> Logins { get; set; }

        public DatabaseContext() : base("eContracting")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
