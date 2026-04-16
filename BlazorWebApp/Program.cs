using BlazorWebApp.AutoMapper;
using BlazorWebApp.Components;
using BlazorWebApp.Configurations;
using BlazorWebApp.Services;

using BookTracker.DAL.DBContexts;

using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

			builder.Services.RegisterDatabase(builder.Configuration);
			builder.Services.RegisterAppServices();
			builder.Services.AddHttpClient();
			builder.Services.AddHangfire(config =>
				config.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UsePostgreSqlStorage(options =>
						options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("BooksConnection"))));
			builder.Services.AddHangfireServer();
			builder.Services.AddAutoMapper(cfg =>
			{
				cfg.AddProfile<BooksProfile>();
				cfg.AddProfile<AuthorsProfile>();
				cfg.AddProfile<GenresProfile>();
			});

			builder.Services.AddLocalization();
			builder.Services.AddControllers();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
				dbContext.Database.Migrate();
			}

			var supportedCultures = new[] { "en", "uk-UA" };
			var localizationOptions = new RequestLocalizationOptions()
				.AddSupportedCultures(supportedCultures)
				.AddSupportedUICultures(supportedCultures)
				.SetDefaultCulture(supportedCultures[1])
				.AddInitialRequestCultureProvider(new CookieRequestCultureProvider());

			app.UseRequestLocalization(localizationOptions);

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseAntiforgery();

			var dashboardUsername = builder.Configuration["Hangfire:Dashboard:Username"] ?? string.Empty;
			var dashboardPassword = builder.Configuration["Hangfire:Dashboard:Password"] ?? string.Empty;
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization =
				[
					new HangfireDashboardAuthorizationFilter(dashboardUsername, dashboardPassword)
				]
			});

			app.MapControllers();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			app.Run();
		}
	}
}