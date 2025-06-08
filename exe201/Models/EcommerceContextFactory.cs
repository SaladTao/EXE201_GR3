using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace exe201.Models
{
    public class EcommerceContextFactory : IDesignTimeDbContextFactory<EcommerceContext>
    {
        public EcommerceContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Current Directory: {basePath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
            Console.WriteLine($"Connection String: {connectionString ?? "NULL"}");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'PostgreSqlConnection' not found. Check appsettings.json or current directory.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<EcommerceContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new EcommerceContext(optionsBuilder.Options);
        }
    }
}
