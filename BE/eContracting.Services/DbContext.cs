using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        /// Gets or sets the files.
        /// </summary>
        public DbSet<DbFileModel> Files { get; set; }

        /// <summary>
        /// Gets or sets the file attributes.
        /// </summary>
        public DbSet<DbFileAttributeModel> FileAttributes { get; set; }

        /// <summary>
        /// Gets or sets collection of signed files.
        /// </summary>
        public DbSet<DbSignedFileModel> SignedFiles { get; set; }

        /// <summary>
        /// Gets or sets upload group data.
        /// </summary>
        public DbSet<DbUploadGroupFileModel> UploadGroupFiles { get; set; }

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

            this.BuildFileModelEntity(modelBuilder);
            this.BuildFileAttributeModel(modelBuilder);
            this.BuildSignedFileModel(modelBuilder);
            this.BuildUploadGroupFileModel(modelBuilder);
        }

        protected void BuildFileModelEntity(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DbFileModel>();

            entity
                .ToTable("Files")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            entity.Property(x => x.FileName).IsRequired();
            entity.Property(x => x.FileExtension).IsRequired();
            entity.Property(x => x.MimeType).IsRequired();
            entity.Property(x => x.Size).IsRequired();
            entity.Property(x => x.Content).IsRequired();

            entity
                .HasOptional(x => x.UploadGroup)
                .WithMany()
                .WillCascadeOnDelete(false);

            entity
                .HasOptional(x => x.SignedFile);

            entity
                .HasMany(x => x.Attributes).WithOptional().HasForeignKey(x => x.FileId).WillCascadeOnDelete(true);
        }

        protected void BuildFileAttributeModel(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DbFileAttributeModel>();

            entity
                .ToTable("FileAttributes")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.FileId).IsRequired();
        }

        //protected void BuildFileHasAttributeModel(DbModelBuilder modelBuilder)
        //{
        //    var entity = modelBuilder.Entity<DbFileHasAttributeModel>();

        //    entity
        //        .ToTable("FileAndAttribute")
        //        .HasKey(x => x.Id)
        //        .Property(x => x.Id)
        //        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        //    entity.HasRequired<DbFileModel>(x => x.File).WithMany().HasForeignKey(x => x.Id);
        //}

        protected void BuildSignedFileModel(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DbSignedFileModel>();

            entity
                .ToTable("SignedFiles")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            entity.Property(x => x.SessionId).IsRequired();
            entity.Property(x => x.Guid).IsRequired();
            entity.Property(x => x.Key).IsRequired();
            entity
                .HasRequired(x => x.File)
                .WithOptional(x => x.SignedFile)
                .WillCascadeOnDelete(true);
        }

        protected void BuildUploadGroupFileModel(DbModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DbUploadGroupFileModel>();
            entity
                .ToTable("UploadGroups")
                .HasKey(x => x.Id)
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            entity.Property(x => x.Key).IsRequired();
            entity.Property(x => x.SessionId).IsRequired();

            //entity
            //    .HasRequired(x => x.OutputFile)
            //    .WithRequiredPrincipal()
            //    .WillCascadeOnDelete(true);
            //entity
            //    .HasMany(x => x.OriginalFiles)
            //    .WithRequired()
            //    .HasForeignKey(x => x.Id)
            //    .WillCascadeOnDelete(true);
        }
    }
}
