using BookTracker.DAL.DBContexts;

namespace BookTracker.DAL.DBManagers
{
    public class BaseDbManager
    {
        /// <summary>
        /// The books database context
        /// </summary>
        internal BooksDbContext BooksDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbManager"/> class.
        /// </summary>
        /// <param name="booksDbContext">The books database context.</param>
        internal BaseDbManager(BooksDbContext booksDbContext)
        {
            BooksDbContext = booksDbContext;
        }
    }
}