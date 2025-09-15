using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Models;
using VConnect.Models.Events;
using VConnect.Models.Cases;

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
        public DbSet<ProfileDetails> ProfileDetails { get; set; }

        // Event system
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> EventRoles { get; set; }
        public DbSet<EventApplication> EventApplications { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<EventNotification> EventNotifications { get; set; }

        // Case Study system
        public DbSet<Study> Studies { get; set; }
        public DbSet<Category>Categories { get; set; }
        public DbSet<CaseGalleryImage> CaseGalleryImages { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Impact_Stats> ImpactStats { get; set; }   // using your underscore class

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

            // Role
            modelBuilder.Entity<Role>(r =>
            {
                r.HasKey(ro => ro.EventRoleId);
                r.Property(ro => ro.RoleName).IsRequired().HasMaxLength(100);

                r.HasOne(ro => ro.Event)
                 .WithMany(ev => ev.Roles)
                 .HasForeignKey(ro => ro.EventId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // EventApplication
            modelBuilder.Entity<EventApplication>(a =>
            {
                a.HasKey(ap => ap.EventApplicationId);

                a.HasOne(ap => ap.Event)
                 .WithMany(ev => ev.Applications)
                 .HasForeignKey(ap => ap.EventId);

                a.HasOne(ap => ap.Role)
                 .WithMany(ro => ro.Applications)
                 .HasForeignKey(ap => ap.EventRoleId);

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

                p.HasOne(pa => pa.Role)
                 .WithMany()
                 .HasForeignKey(pa => pa.EventRoleId);

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
                 .HasForeignKey(g => g.StudyId);

                c.HasMany(cs => cs.Milestones)
                 .WithOne(m => m.Study)
                 .HasForeignKey(m => m.StudyId);
            });

            // CaseCategory
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
