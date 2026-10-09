using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Services;
using BookTracker.Jobs.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Configurations
{
	public static class DatabaseConfigurationsExtensions
	{
		public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContextFactory<BooksDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("BooksConnection")));
			services.AddScoped<IBookDbManager, BookDbManager>();
			services.AddScoped<IAuthorDbManager, AuthorDbManager>();
			services.AddScoped<IGenreDbManager, GenreDbManager>();
			services.AddScoped<ITranslationsDbManager, TranslationsDbManager>();
			services.AddScoped<IBookTranslationProcessor, BookTranslationProcessor>();
			services.AddScoped<ITextTranslator, ConfigurableTextTranslator>();
			services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

			return services;
		}
	}
}
