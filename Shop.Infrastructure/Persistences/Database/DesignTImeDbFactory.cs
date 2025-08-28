using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shop.Infrastructure.Persistences.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            // Đường dẫn đến thư mục chứa project API, giả sử cách Infrastructure 1 cấp thư mục
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "CareNest_Shop");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath) // trỏ đúng đến thư mục chứa appsettings.Development.json
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }

}
