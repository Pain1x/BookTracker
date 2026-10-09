using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Genres;
using Microsoft.EntityFrameworkCore;
using BookTracker.Common;
using BookTracker.DAL.Abstractions;

namespace BookTracker.DAL.DBManagers
{
    public class GenreDbManager(IDbContextFactory<BooksDbContext> factory) : BaseDbManager(factory), IGenreDbManager
    {
        public async Task<IEnumerable<Genre>> GetGenresForSearchableDropdown(string? searchTerm)
        {
            await using var context = await BooksDbContextFactory.CreateDbContextAsync();
            var query = context.Genres.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(g => g.Name.Contains(searchTerm));
            }

            return await query
                .OrderBy(g => g.Name)
                .Take(DropdownConstants.InitialDisplayLimit)
                .ToListAsync();
        }
    }
}