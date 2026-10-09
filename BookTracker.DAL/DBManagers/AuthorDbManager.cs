using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Authors;
using Microsoft.EntityFrameworkCore;
using BookTracker.Common;
using BookTracker.DAL.Abstractions;

namespace BookTracker.DAL.DBManagers
{
    public class AuthorDbManager(IDbContextFactory<BooksDbContext> factory) : BaseDbManager(factory), IAuthorDbManager
    {
        public async Task<IEnumerable<Author>> GetAuthorsForSearchableDropdown(string? searchTerm)
        {
            await using var context = await BooksDbContextFactory.CreateDbContextAsync();
            var query = context.Authors.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm));
            }

            return await query
                .OrderBy(a => a.Name)
                .Take(DropdownConstants.InitialDisplayLimit)
                .ToListAsync();
        }
    }
}