using BookTracker.DAL.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBManagers
{
    public class BaseDbManager
    {
        /// <summary>
        /// The books database context
        /// </summary>
        internal readonly BooksDbContext BooksDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbManager"/> class.
        /// </summary>
        /// <param name="contextFactory">The books database context.</param>
        internal BaseDbManager(IDbContextFactory<BooksDbContext> contextFactory)
        {
            BooksDbContext = contextFactory.CreateDbContext();
        }
    }
}