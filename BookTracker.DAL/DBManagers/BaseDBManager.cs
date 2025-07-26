using BookTracker.DAL.DBContexts;

namespace BookTracker.DAL.DBManagers
{
    public class BaseDBManager
    {
        /// <summary>
        /// The books database context
        /// </summary>
        internal BooksDbContext BooksDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDBManager"/> class.
        /// </summary>
        /// <param name="booksDBContext">The books database context.</param>
        internal BaseDBManager(BooksDbContext booksDBContext)
        {
            BooksDbContext = booksDBContext;
        }
    }
}