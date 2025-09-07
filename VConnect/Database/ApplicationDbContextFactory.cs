using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VConnect.Database
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Load connection string from appsettings (Development overrides base)
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = config.GetConnectionString("DefaultConnection")
                     // fallback to your working endpoint if not found
                     ?? "Server=localhost,14330;Database=vconnect;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(cs)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
