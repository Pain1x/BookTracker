using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Services;

using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Configurations
{
	public static class DatabaseConfigurationsExtensions
	{
		public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContextFactory<BooksDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("BooksConnection")));
			services.AddScoped<IBookDbManager, BookDbManager>();
			services.AddScoped<IBookTranslationProcessor, BookTranslationProcessor>();
			services.AddScoped<ITextTranslator, ConfigurableTextTranslator>();

			return services;
		}
	}
}
