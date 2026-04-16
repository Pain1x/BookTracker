using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Services;
using BookTracker.DAL.Abstractions;

using BlazorWebApp.Services;

namespace BlazorWebApp.Configurations
{
	public static class ServiceConfigurationsExtensions
	{
		public static IServiceCollection RegisterAppServices(this IServiceCollection services)
		{
			services.AddScoped<IBooksService, BooksService>();
			services.AddScoped<IBookTranslationJobScheduler, HangfireBookTranslationJobScheduler>();

			return services;
		}
	}
}