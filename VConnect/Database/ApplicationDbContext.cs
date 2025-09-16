using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Models;
using VConnect.Models.Events;
using VConnect.Models.Cases;
using VConnect.Models.SOS; 

namespace VConnect.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationProvided> DonationProvided { get; set; }

        public DbSet<ProfileDetails> ProfileDetails { get; set; }

        // Event system
        public DbSet<Event> Events { get; set; }
        public DbSet<EventApplication> EventApplications { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<EventNotification> EventNotifications { get; set; }

        // Case Study system
        public DbSet<Study> Studies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CaseGalleryImage> CaseGalleryImages { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Impact_Stats> ImpactStats { get; set; } // underscore class

        // SOS system
        public DbSet<SosPost> SosPosts { get; set; }
        public DbSet<SosComment> SosComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser
            modelBuilder.Entity<ApplicationUser>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Email).IsRequired();
                e.HasIndex(u => u.Email).IsUnique();
            });

            // ProfileDetails
            modelBuilder.Entity<ProfileDetails>(e =>
            {
                e.HasKey(p => p.ProfileDetailsId);

                e.HasIndex(p => p.UserId).IsUnique(); // enforce 1:1

                e.Property(p => p.DateOfBirth).HasColumnType("date");
                e.Property(p => p.CreatedAt).HasColumnType("timestamptz");
                e.Property(p => p.UpdatedAt).HasColumnType("timestamptz");

                e.HasOne<ApplicationUser>()
                 .WithOne()
                 .HasForeignKey<ProfileDetails>(p => p.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Event
            modelBuilder.Entity<Event>(e =>
            {
                e.HasKey(ev => ev.EventId);
                e.Property(ev => ev.Title).IsRequired().HasMaxLength(200);
                e.Property(ev => ev.Location).HasMaxLength(200);
                e.Property(ev => ev.Compensation).HasMaxLength(100);
            });

            

            // EventApplication
            modelBuilder.Entity<EventApplication>(a =>
            {
                a.HasKey(ap => ap.EventApplicationId);

                a.HasOne(ap => ap.Event)
                 .WithMany(ev => ev.Applications)
                 .HasForeignKey(ap => ap.EventId);

                

                a.HasOne(ap => ap.User)
                 .WithMany()
                 .HasForeignKey(ap => ap.UserId);
            });

            // Participation
            modelBuilder.Entity<Participation>(p =>
            {
                p.HasKey(pa => pa.EventParticipationId);

                p.HasOne(pa => pa.Event)
                 .WithMany(ev => ev.Participations)
                 .HasForeignKey(pa => pa.EventId);

                

                p.HasOne(pa => pa.User)
                 .WithMany()
                 .HasForeignKey(pa => pa.UserId);
            });

            // EventNotification
            modelBuilder.Entity<EventNotification>(n =>
            {
                n.HasKey(no => no.EventNotificationId);
                n.Property(no => no.Message).HasMaxLength(500);

                n.HasOne(no => no.Event)
                 .WithMany()
                 .HasForeignKey(no => no.EventId);

                n.HasOne(no => no.User)
                 .WithMany()
                 .HasForeignKey(no => no.UserId);
            });

            // Study
            modelBuilder.Entity<Study>(c =>
            {
                c.HasKey(cs => cs.Id);
                c.Property(cs => cs.Title).IsRequired().HasMaxLength(200);
                c.Property(cs => cs.Category).HasMaxLength(100);

                c.HasMany(cs => cs.GalleryImages)
                 .WithOne(g => g.Study)
                 .HasForeignKey(g => g.StudyId)
                 .OnDelete(DeleteBehavior.Cascade);

                c.HasMany(cs => cs.Milestones)
                 .WithOne(m => m.Study)
                 .HasForeignKey(m => m.StudyId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Category
            modelBuilder.Entity<Category>(cat =>
            {
                cat.HasKey(ca => ca.Id);
                cat.Property(ca => ca.Name).IsRequired().HasMaxLength(100);
            });

            // CaseGalleryImage
            modelBuilder.Entity<CaseGalleryImage>(g =>
            {
                g.HasKey(gi => gi.Id);
                g.Property(gi => gi.ImageUrl).IsRequired();
            });

            // Milestone
            modelBuilder.Entity<Milestone>(m =>
            {
                m.HasKey(ms => ms.Id);
                m.Property(ms => ms.Title).IsRequired().HasMaxLength(200);
            });

            // Impact_Stats
            modelBuilder.Entity<Impact_Stats>(i =>
            {
                i.HasKey(im => im.Id);
            });

            // SOS Post
            modelBuilder.Entity<SosPost>(s =>
            {
                s.HasKey(sp => sp.Id);
                s.Property(sp => sp.Name).HasMaxLength(100);
                s.Property(sp => sp.Contact).HasMaxLength(150);
                s.Property(sp => sp.Location).HasMaxLength(300);
                s.Property(sp => sp.Description).HasMaxLength(2000);
                s.Property(sp => sp.Status).HasMaxLength(50);

                s.HasMany(sp => sp.Comments)
                 .WithOne(c => c.SosPost)
                 .HasForeignKey(c => c.SosPostId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // SOS Comment
            modelBuilder.Entity<SosComment>(c =>
            {
                c.HasKey(co => co.Id);
                c.Property(co => co.Message).IsRequired().HasMaxLength(1000);

                c.HasOne(co => co.SosPost)
                 .WithMany(sp => sp.Comments)
                 .HasForeignKey(co => co.SosPostId);

                c.HasOne(co => co.ParentComment)
                 .WithMany(co => co.Replies)
                 .HasForeignKey(co => co.ParentCommentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // Auto-timestamps for ProfileDetails
        public override int SaveChanges()
        {
            StampProfileTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampProfileTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void StampProfileTimestamps()
        {
            var now = DateTimeOffset.UtcNow;

            foreach (var entry in ChangeTracker.Entries<ProfileDetails>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
        }
    }
}
