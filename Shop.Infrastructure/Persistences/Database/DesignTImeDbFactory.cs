﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Shop.Infrastructure.Persistences.Configuration;

namespace Shop.Infrastructure.Persistences.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "CareNest_Shop");
            apiProjectPath = Path.GetFullPath(apiProjectPath);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var dbSettings = new DatabaseSettings();
            configuration.GetSection("DatabaseSettings").Bind(dbSettings);

            string connectionString = dbSettings.GetConnectionString();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }

}
