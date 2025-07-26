using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp.Configurations
{
    public static class DatabaseConfigurationsExtensions
    {
        public static IServiceCollection RegisterDatabase(this IServiceCollection services)
        {
            services.AddDbContext<BooksDbContext>(options => options.UseNpgsql("ConnectionStrings:BooksConnection"));
            services.AddScoped<IBookDBManager, BookDBManager>();

            return services;
        }
    }
}
