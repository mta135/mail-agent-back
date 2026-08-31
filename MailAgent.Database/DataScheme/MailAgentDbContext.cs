using MailAgent.DataBaseAccess.DataScheme;
using MailAgent.Model.EmailMessage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.DataBaseAccess.Contex
{
    public class MailAgentDbContext : DbContext
    {

        //public MailAgentDbContext()
        //{
            
        //}

        #region DbSet Properties

        public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();

        public DbSet<EmailMessageTo> EmailMessageTos => Set<EmailMessageTo>();

        public DbSet<EmailMessageAttachment> EmailMessageAttachments => Set<EmailMessageAttachment>();

        public DbSet<EmailMessageCopy> EmailMessageCopies => Set<EmailMessageCopy>();


        #endregion


        public MailAgentDbContext(DbContextOptions<MailAgentDbContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MailAgent;Integrated Security=True;Trust Server Certificate=True;");
        //    }
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEmailMessages(modelBuilder);
            ConfigureEmailMessageTo(modelBuilder);

            ConfigureEmailMessageAttachments(modelBuilder);

            ConfigureEmailMessageCopy(modelBuilder);
        }

        #region Private Configuration Methods

        private static void ConfigureEmailMessages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessage>().ToTable("email_message", "dbo");

            modelBuilder.Entity<EmailMessage>().HasKey(m => m.Id);
            modelBuilder.Entity<EmailMessage>().Property(m => m.Id).HasColumnName("id").ValueGeneratedNever();

            modelBuilder.Entity<EmailMessage>().Property(m => m.From).HasColumnName("from").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);
            modelBuilder.Entity<EmailMessage>().Property(m => m.Header).HasColumnName("header").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);

            modelBuilder.Entity<EmailMessage>().Property(m => m.Subject).HasColumnName("subject").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);
            modelBuilder.Entity<EmailMessage>().Property(m => m.Body).HasColumnName("body").HasColumnType("nvarchar(max)").IsRequired(false);

            modelBuilder.Entity<EmailMessage>().Property(m => m.Footer).HasColumnName("footer").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);
            modelBuilder.Entity<EmailMessage>().Property(m => m.CreatedAt).HasColumnName("create_date").HasColumnType("datetime2(7)").IsRequired(false);
            modelBuilder.Entity<EmailMessage>().Property(m => m.Status).HasColumnName("status").HasColumnType("int").IsRequired(false);

        }

        private static void ConfigureEmailMessageTo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessageTo>().ToTable("email_message_to", "dbo");

            modelBuilder.Entity<EmailMessageTo>().HasKey(t => t.Id);
            modelBuilder.Entity<EmailMessageTo>().Property(t => t.Id).HasColumnName("Id").ValueGeneratedOnAdd();

            modelBuilder.Entity<EmailMessageTo>().Property(t => t.EmailMessageId).HasColumnName("email_message_id").HasColumnType("uniqueidentifier").IsRequired(false);
            modelBuilder.Entity<EmailMessageTo>().Property(t => t.To).HasColumnName("to").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);

            modelBuilder.Entity<EmailMessageTo>().HasOne(t => t.EmailMessage).WithMany(m => m.EmailMessageTos).HasForeignKey(t => t.EmailMessageId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);




        }

        private static void ConfigureEmailMessageAttachments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessageAttachment>().ToTable("email_message_attachment", "dbo");

            modelBuilder.Entity<EmailMessageAttachment>().HasKey(a => a.Id);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.Id).HasColumnName("Id").ValueGeneratedOnAdd();

            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.EmailMessageId).HasColumnName("email_message_id").HasColumnType("uniqueidentifier").IsRequired(false);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.FileName).HasColumnName("file_name").HasColumnType("nvarchar(255)").IsRequired().HasMaxLength(255);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.ContentType).HasColumnName("content_type").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.FileSizeBytes).HasColumnName("file_size_bytes").HasColumnType("bigint").IsRequired(false);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.Data).HasColumnName("data").HasColumnType("varbinary(max)").IsRequired(false);
            modelBuilder.Entity<EmailMessageAttachment>().Property(a => a.CreateDate).HasColumnName("create_date").HasColumnType("datetime2(7)").IsRequired(false);

            // Relație circulară
            modelBuilder.Entity<EmailMessageAttachment>().HasOne(a => a.EmailMessage).WithMany(m => m.Attachments).HasForeignKey(a => a.EmailMessageId).OnDelete(DeleteBehavior.Cascade);

        }

        private static void ConfigureEmailMessageCopy(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessageCopy>().ToTable("email_message_copy", "dbo");

            modelBuilder.Entity<EmailMessageCopy>().HasKey(c => c.Id);
            modelBuilder.Entity<EmailMessageCopy>().Property(c => c.Id).HasColumnName("Id").ValueGeneratedOnAdd();

            modelBuilder.Entity<EmailMessageCopy>().Property(c => c.EmailMessageId).HasColumnName("email_message_id").HasColumnType("uniqueidentifier").IsRequired(false);
            modelBuilder.Entity<EmailMessageCopy>().Property(c => c.Copy).HasColumnName("copy").HasColumnType("nvarchar(100)").IsRequired(false).HasMaxLength(100);

            // Relație circulară 
            modelBuilder.Entity<EmailMessageCopy>().HasOne(c => c.EmailMessage).WithMany(m => m.Copies).HasForeignKey(c => c.EmailMessageId).OnDelete(DeleteBehavior.Cascade);

        }

        #endregion
    }
}
