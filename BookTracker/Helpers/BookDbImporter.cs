using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities;

namespace BookTracker.Helpers
{
    internal class BookDbImporter
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly BooksDbContext _context;

        public BookDbImporter(BooksDbContext context)
        {
            _context = context;
        }

        public void ImportBooks(List<Book> booksToImport)
        {
            foreach (var bookInput in booksToImport)
            {
                // Check or create author
                var author = bookInput.Author;

                if (author != null)
                {
                    author = new Author
                    {
                        AuthorPK = bookInput.Author.AuthorPK,
                        Name = bookInput.Author.Name
                    };

                    if (!_context.Authors.Contains(author))
                    {
                        _context.Authors.Add(author);
                        _context.SaveChanges(); // save to get PK for book insert
                    }
                }

                // Check or create genre
                var genre = bookInput.Genre;
                if (genre != null)
                {
                    genre = new Genre
                    {
                        GenrePK = bookInput.Genre.GenrePK,
                        Name = bookInput.Genre.Name
                    };

                    if (!_context.Genres.Contains(genre))
                    {
                        _context.Genres.Add(genre);
                        _context.SaveChanges(); // save to get PK for book insert
                    }
                }

                // Insert book
                var book = new Book
                {
                    BookPK = bookInput.BookPK,
                    Title = bookInput.Title,
                    Author = bookInput.Author,
                    Genre = bookInput.Genre,
                    DateRead = bookInput.DateRead,
                    Rating = bookInput.Rating,
                    Notes = bookInput.Notes
                };

                _context.Books.Add(book);
            }

            // Final save for all books
            _context.SaveChanges();
        }
    }
}