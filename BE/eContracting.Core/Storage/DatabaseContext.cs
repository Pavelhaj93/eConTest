using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace eContracting.Storage
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("eContractingContext")
        {
        }

        public virtual DbSet<EventLog> EventLogs { get; set; }
        public virtual DbSet<FileAttribute> FileAttributes { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }
        public virtual DbSet<SignedFile> SignedFiles { get; set; }
        public virtual DbSet<UploadGroupOriginalFile> UploadGroupOriginalFiles { get; set; }
        public virtual DbSet<UploadGroup> UploadGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventLog>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<EventLog>()
                .Property(e => e.Guid)
                .IsUnicode(false);

            modelBuilder.Entity<EventLog>()
                .Property(e => e.Event)
                .IsUnicode(false);

            modelBuilder.Entity<EventLog>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<EventLog>()
                .Property(e => e.Error)
                .IsUnicode(false);

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

            modelBuilder.Entity<LoginAttempt>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<LoginAttempt>()
                .Property(e => e.Guid)
                .IsUnicode(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<SignedFile>()
                .Property(e => e.Guid)
                .IsUnicode(false);

            modelBuilder.Entity<UploadGroupOriginalFile>()
                .Property(e => e.FileKey)
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

        }
    }
}
