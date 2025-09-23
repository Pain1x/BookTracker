using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Services;

namespace BlazorWebApp.Configurations
{
	public static class ServiceConfigurationsExtensions
	{
		public static IServiceCollection RegisterAppServices(this IServiceCollection services)
		{
			services.AddScoped<IBooksService, BooksService>();

			return services;
		}
	}
}