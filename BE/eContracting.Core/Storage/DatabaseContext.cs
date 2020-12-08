using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace eContracting.Storage
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }

        public virtual DbSet<FileAttribute> FileAttributes { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<SignedFile> SignedFiles { get; set; }
        public virtual DbSet<UploadGroupOriginalFile> UploadGroupOriginalFiles { get; set; }
        public virtual DbSet<UploadGroup> UploadGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileAttribute>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FileAttribute>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.FileName)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.FileExtension)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.MimeType)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.FileAttributes)
                .WithRequired(e => e.File)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.SignedFiles)
                .WithRequired(e => e.File)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.UploadGroupOriginalFiles)
                .WithRequired(e => e.File)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.UploadGroups)
                .WithRequired(e => e.File)
                .HasForeignKey(e => e.OutputFileId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.Guid)
                .IsUnicode(false);

            modelBuilder.Entity<UploadGroup>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<UploadGroup>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<UploadGroup>()
                .Property(e => e.Guid)
                .IsUnicode(false);

            modelBuilder.Entity<UploadGroup>()
                .HasMany(e => e.UploadGroupOriginalFiles)
                .WithRequired(e => e.UploadGroup)
                .HasForeignKey(e => e.GroupId)
                .WillCascadeOnDelete(false);
        }
    }
}
