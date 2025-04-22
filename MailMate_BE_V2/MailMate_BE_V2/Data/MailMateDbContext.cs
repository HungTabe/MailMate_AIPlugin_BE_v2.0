using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace MailMate_BE_V2.Data
{
    public class MailMateDbContext : DbContext
    {
        public MailMateDbContext(DbContextOptions<MailMateDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailTag> EmailTags { get; set; }
        public DbSet<EmailTagMapping> EmailTagMappings { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignRecipient> CampaignRecipients { get; set; }
        public DbSet<AutoReplyScript> AutoReplyScripts { get; set; }
        public DbSet<EmailSchedule> EmailSchedules { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.SubscriptionPlan)
                .HasConversion<string>();

            // EmailAccounts
            modelBuilder.Entity<EmailAccount>()
                .HasOne(ea => ea.User)
                .WithMany(u => u.EmailAccounts)
                .HasForeignKey(ea => ea.UserId);

            // Emails
            modelBuilder.Entity<Email>()
                .HasOne(e => e.EmailAccount)
                .WithMany(ea => ea.Emails)
                .HasForeignKey(e => e.EmailAccountId);

            // EmailTags
            modelBuilder.Entity<EmailTag>()
                .HasMany(et => et.EmailTagMappings)
                .WithOne(etm => etm.EmailTag)
                .HasForeignKey(etm => etm.EmailTagId);

            // EmailTagMapping
            modelBuilder.Entity<EmailTagMapping>()
                .HasKey(etm => new { etm.EmailId, etm.EmailTagId });

            modelBuilder.Entity<EmailTagMapping>()
                .HasOne(etm => etm.Email)
                .WithMany(e => e.EmailTagMappings)
                .HasForeignKey(etm => etm.EmailId);

            modelBuilder.Entity<EmailTagMapping>()
                .HasOne(etm => etm.EmailTag)
                .WithMany(etm => etm.EmailTagMappings)
                .HasForeignKey(etm => etm.EmailTagId);

            // Campaigns
            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.User)
                .WithMany(u => u.Campaigns)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Campaign>()
                .Property(c => c.Status)
                .HasConversion<string>();

            // CampaignRecipients
            modelBuilder.Entity<CampaignRecipient>()
                .HasOne(cr => cr.Campaign)
                .WithMany(c => c.CampaignRecipients)
                .HasForeignKey(cr => cr.CampaignId);

            modelBuilder.Entity<CampaignRecipient>()
                .Property(cr => cr.Status)
                .HasConversion<string>();

            // AutoReplyScripts
            modelBuilder.Entity<AutoReplyScript>()
                .HasOne(ars => ars.User)
                .WithMany(u => u.AutoReplyScripts)
                .HasForeignKey(ars => ars.UserId);

            // EmailSchedules
            modelBuilder.Entity<EmailSchedule>()
                .HasOne(es => es.EmailAccount)
                .WithMany(ea => ea.EmailSchedules)
                .HasForeignKey(es => es.EmailAccountId);

            modelBuilder.Entity<EmailSchedule>()
                .Property(es => es.Status)
                .HasConversion<string>();

            // Payments
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.PlanType)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            // Logs
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);
        }
    }
}
