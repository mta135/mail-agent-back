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

        public MailAgentDbContext()
        {
            
        }

        public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();


        public MailAgentDbContext(DbContextOptions<MailAgentDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MailAgent;Integrated Security=True;Trust Server Certificate=True;");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEmailMessages(modelBuilder);

        }


        private static void ConfigureEmailMessages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessage>().ToTable("email_message", "dbo");

            modelBuilder.Entity<EmailMessage>().HasKey(m => m.Id);
            modelBuilder.Entity<EmailMessage>().Property(m => m.Id).ValueGeneratedNever();

            modelBuilder.Entity<EmailMessage>().Property(m => m.From).HasMaxLength(100);
            modelBuilder.Entity<EmailMessage>().Property(e => e.Subject).HasMaxLength(500);

        }
    }
}
