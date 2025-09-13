using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Models;                 // ApplicationUser, Donation, ProfileDetails
using VConnect.Models.Events;          // Event, Role, EventApplication, Participation, EventNotification

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

        // New Event system
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> EventRoles { get; set; }
        public DbSet<EventApplication> EventApplications { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<EventNotification> EventNotifications { get; set; }

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
                e.Property(p => p.FirstName).HasMaxLength(50);
                e.Property(p => p.LastName).HasMaxLength(50);
                e.Property(p => p.Email).HasMaxLength(100);
                e.Property(p => p.PhoneNumber).HasMaxLength(30);
                e.Property(p => p.City).HasMaxLength(100);
                e.Property(p => p.Address).HasMaxLength(200);
                e.Property(p => p.ProfilePictureUrl).HasMaxLength(500);

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
                 .WithMany()  // no navigation for now
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
            var now = DateTime.UtcNow;

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
