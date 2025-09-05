using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BookTracker.DAL.DBContexts
{
	public class BooksDbContextFactory : IDesignTimeDbContextFactory<BooksDbContext>
	{
		public BooksDbContext CreateDbContext(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true)
				.AddJsonFile("appsettings.Development.json", optional: true)
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<BooksDbContext>();
			var connectionString = config.GetConnectionString("BooksConnection");
				optionsBuilder.UseNpgsql(connectionString);

			return new BooksDbContext(optionsBuilder.Options);
		}
	}
}