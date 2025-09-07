using Microsoft.EntityFrameworkCore;
using VConnect.Models;

namespace VConnect.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<RecurringDonation> RecurringDonations { get; set; }

    }
}