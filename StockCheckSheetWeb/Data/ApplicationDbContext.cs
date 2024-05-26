using Microsoft.EntityFrameworkCore;
using StockCheckSheetWeb.Models;

namespace StockCheckSheetWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Input> Inputs { get; set; }
        public DbSet<Output> Outputs { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        //public DbSet<StockCheckSheet> StockCheckSheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        //    modelBuilder.Entity<StockCheckSheet>()
        //        .HasMany(u => u.Inputs)
        //        .WithOne(u => u.StockCheckSheet)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<StockCheckSheet>()
        //        .HasMany(u => u.Outputs)
        //        .WithOne(u => u.StockCheckSheet)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<StockCheckSheet>()
        //        .HasMany(u => u.Stocks)
        //        .WithOne(u => u.StockCheckSheet)
        //        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Input>().HasData(
                new Input { Id = 1, Amount = 5, UnitCost = 10.5, TotalCost = 52.5 },
                new Input { Id = 2, Amount = 10, UnitCost = 12.5, TotalCost = 125 },
                new Input { Id = 3, Amount = 3, UnitCost = 7, TotalCost = 21 }
            );
        }


    }
}
