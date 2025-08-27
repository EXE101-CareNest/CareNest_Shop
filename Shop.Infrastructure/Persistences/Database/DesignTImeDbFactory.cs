using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shop.Infrastructure.Persistences.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            // Lấy đường dẫn thư mục chạy hiện tại (thường là thư mục chứa project)
            var basePath = Directory.GetCurrentDirectory();

            // Tạo builder để đọc file appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            // Lấy connection string theo key trong appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
