using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Models;                 // ApplicationUser
using VConnect.Models;       // ProfileDetails

namespace VConnect.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<ApplicationUser> Users { get; set; } = null!;
        public DbSet<ProfileDetails> ProfileDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser
            modelBuilder.Entity<ApplicationUser>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Email).IsRequired();
                // Optional but useful: unique email at DB level
                e.HasIndex(u => u.Email).IsUnique();
            });

            // ProfileDetails
            modelBuilder.Entity<ProfileDetails>(e =>
            {
                e.HasKey(p => p.ProfileDetailsId);

                // One-to-one: ProfileDetails.UserId -> ApplicationUser.Id
                e.HasIndex(p => p.UserId).IsUnique(); // enforce 1:1
                e.Property(p => p.FirstName).HasMaxLength(50);
                e.Property(p => p.LastName).HasMaxLength(50);
                e.Property(p => p.Email).HasMaxLength(100);
                e.Property(p => p.PhoneNumber).HasMaxLength(30);
                e.Property(p => p.City).HasMaxLength(100);
                e.Property(p => p.Address).HasMaxLength(200);
                e.Property(p => p.ProfilePictureUrl).HasMaxLength(500);

                // Relationship config
                e.HasOne<ApplicationUser>()                   // no navs needed on either side
                 .WithOne()
                 .HasForeignKey<ProfileDetails>(p => p.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
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
