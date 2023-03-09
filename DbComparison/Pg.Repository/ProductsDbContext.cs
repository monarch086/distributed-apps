using DataModel;
using Microsoft.EntityFrameworkCore;

namespace Pg.Repository
{
    internal class ProductsDbContext : DbContext
    {
        private const string HOST = "127.0.0.1:5432";
        private const string DATABASE = "postgres";
        private const string USERNAME = "postgres";

        public DbSet<Record> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql($"Host={HOST};Database={DATABASE};Username={USERNAME};");
    }
}
