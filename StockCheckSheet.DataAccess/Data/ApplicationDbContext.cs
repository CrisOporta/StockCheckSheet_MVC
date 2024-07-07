using StockCheckSheetWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace StockCheckSheet.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Input> Inputs { get; set; }
        public DbSet<Output> Outputs { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always require with Entity Framework Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Inputs)
                .WithOne(u => u.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Outputs)
                .WithOne(u => u.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Stocks)
                .WithOne(u => u.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade);


            // Crear instancia de StockCheckSheet con el ID 1
            //var report = new Report { Id = 1 };

            //modelBuilder.Entity<Report>().HasData(report);

            //modelBuilder.Entity<Input>().HasData(
            //    new Input { Id = 1, Amount = 5, UnitCost = 10.5, TotalCost = 52.5, ReportId = report.Id },
            //    new Input { Id = 2, Amount = 10, UnitCost = 12.5, TotalCost = 125, ReportId = report.Id },
            //    new Input { Id = 3, Amount = 3, UnitCost = 7, TotalCost = 21, ReportId = report.Id }
            //);
        }
    }
}
