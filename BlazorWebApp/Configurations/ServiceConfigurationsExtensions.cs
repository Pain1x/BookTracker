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
			services.AddScoped<ITranslationService, LocalLlmTranslationService>(); // Register the local LLM service
			services.AddScoped<IBookTranslationJobScheduler, HangfireBookTranslationJobScheduler>();

			return services;
		}
	}
}