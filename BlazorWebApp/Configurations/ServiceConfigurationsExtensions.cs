using Microsoft.Extensions.DependencyInjection;
using BookTracker.BLL.Services;
using BookTracker.BLL.Abstractions;
using BookTracker.Jobs.Abstractions;
using BookTracker.Jobs.JobSchedulers;

namespace BlazorWebApp.Configurations
{
	public static class ServiceConfigurationsExtensions
	{
		public static IServiceCollection RegisterAppServices(this IServiceCollection services)
		{
			services.AddScoped<IBooksService, BooksService>();
			services.AddScoped<IGenreService, GenreService>();
			services.AddScoped<IAuthorService, AuthorService>();
			services.AddScoped<IBookTranslationJobScheduler, HangfireBookTranslationJobScheduler>();

			return services;
		}
	}
}