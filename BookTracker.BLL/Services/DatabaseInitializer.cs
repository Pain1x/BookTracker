using BookTracker.BLL.Abstractions;
using BookTracker.DAL.DBContexts; // Necessary for migration logic, but contained within BLL boundary
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookTracker.BLL.Services
{
	/// <summary>
	/// Handles the initialization and migration of the database context.
	/// </summary>
	public class DatabaseInitializer(BooksDbContext dbContext) : IDatabaseInitializer
	{
		public async Task InitializeAsync()
		{
			// This ensures the database schema is up-to-date before the application starts.
			await dbContext.Database.MigrateAsync();
		}
	}
}