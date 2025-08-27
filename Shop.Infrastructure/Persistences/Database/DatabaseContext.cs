using Microsoft.EntityFrameworkCore;


namespace Shop.Infrastructure.Persistences.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Shop.Domain.Entitites.Shop> Shops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
