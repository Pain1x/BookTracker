using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;

using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Configurations
{
	public static class DatabaseConfigurationsExtensions
	{
		public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<BooksDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("BooksConnection")));
			services.AddScoped<IBookDbManager, BookDbManager>();

			return services;
		}
	}
}
