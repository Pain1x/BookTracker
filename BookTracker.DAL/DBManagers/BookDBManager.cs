using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Genres;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBManagers
{
    public class BookDbManager(IDbContextFactory<BooksDbContext> contextFactory)
        : BaseDbManager(contextFactory), IBookDbManager
    {
        #region Implementation of IBookDbManager

        ///<inheritdoc/>
        public async Task<Book> AddBook(Book book)
        {
            Book bookToSave;
            
            await using (var context = BooksDbContextFactory.CreateDbContext())
            {
                var author = await context.Authors.FindAsync(book.Author.AuthorPk);

                if (author == null)
                {
                    author = new Author
                    {
                        AuthorPk = Guid.NewGuid(),
                        Name = book.Author.Name
                    };

                    await context.Authors.AddAsync(author);
                }

                var genre = await context.Genres.FindAsync(book.Genre.GenrePk);

                if (genre == null)
                {
                    genre = new Genre
                    {
                        GenrePk = Guid.NewGuid(),
                        Name = book.Genre.Name
                    };

                    await context.Genres.AddAsync(genre);
                }

                bookToSave = new Book
                {
                    BookPk = Guid.NewGuid(),
                    Title = book.Title,
                    Author = author,
                    Genre = genre,
                    DateRead = book.DateRead?.ToUniversalTime(),
                    Rating = book.Rating,
                    Notes = book.Notes
                };

                await context.Books.AddAsync(bookToSave);
                await context.SaveChangesAsync();
            }

            return bookToSave;
        }

        ///<inheritdoc/>
        public async Task UpdateBook(Book updatedBook)
        {
            await using (var context = BooksDbContextFactory.CreateDbContext())
            {
                var existing = await context.Books.FindAsync(updatedBook.BookPk);

                if (existing != null)
                {
                    context.Entry(existing).CurrentValues.SetValues(updatedBook);
                    await context.SaveChangesAsync();
                }
            }
        }

        ///<inheritdoc/>
        public async Task<List<Book>> GetAllBooksLocalized(byte languagePk)
        {
            List<Book> books;

            await using (var context = BooksDbContextFactory.CreateDbContext())
            {
                books = await context.Books
                    .Select(b => new Book
                    {
                        BookPk = b.BookPk,
                        Title = b.Translations
                            .Where(t => t.LanguagePk == languagePk)
                            .Select(t => t.Title)
                            .FirstOrDefault() ?? b.Title,
                        Author = new Author
                        {
                            AuthorPk = b.AuthorPk,
                            Name = b.Author.Translations
                                .Where(t => t.LanguagePk == languagePk)
                                .Select(t => t.Name)
                                .FirstOrDefault() ?? b.Author.Name
                        },
                        Genre = new Genre
                        {
                            GenrePk = b.GenrePk,
                            Name = b.Genre.Translations
                                .Where(t => t.LanguagePk == languagePk)
                                .Select(t => t.Name)
                                .FirstOrDefault() ?? b.Genre.Name
                        },
                        DateRead = b.DateRead,
                        Rating = b.Rating,
                        Notes = b.Notes
                    })
                    .OrderBy(b => b.Title).ToListAsync();
            }

            return books;
        }

        ///<inheritdoc/>
        public async Task<Book?> FindBookByPkLocalized(Guid bookPk, byte languagePk)
        {
            Book? book;

            await using (var context = BooksDbContextFactory.CreateDbContext())
            {
                book = await context.Books
                    .Where(b => b.BookPk == bookPk)
                    .Select(b => new Book
                    {
                        BookPk = b.BookPk,
                        Title = b.Translations
                            .Where(t => t.LanguagePk == languagePk)
                            .Select(t => t.Title)
                            .FirstOrDefault() ?? b.Title,
                        Author = new Author
                        {
                            AuthorPk = b.AuthorPk,
                            Name = b.Author.Translations
                                .Where(t => t.LanguagePk == languagePk)
                                .Select(t => t.Name)
                                .FirstOrDefault() ?? b.Author.Name
                        },
                        Genre = new Genre
                        {
                            GenrePk = b.GenrePk,
                            Name = b.Genre.Translations
                                .Where(t => t.LanguagePk == languagePk)
                                .Select(t => t.Name)
                                .FirstOrDefault() ?? b.Genre.Name
                        },
                        DateRead = b.DateRead,
                        Rating = b.Rating,
                        Notes = b.Notes
                    })
                    .FirstOrDefaultAsync();
            }

            return book;
        }

        ///<inheritdoc/>
        public async Task<Dictionary<int, int>> CountBooksByYears()
        {
            Dictionary<int, int> result;

            await using (var context = BooksDbContextFactory.CreateDbContext())
            {
                var currentYear = DateTime.UtcNow.Year;
                var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();
                // Get all books with a DateRead in the given years
                var yearSet = years.ToHashSet();

                var query = await context.Books
                    .Where(b => b.DateRead.HasValue && yearSet.Contains(b.DateRead.Value.Year))
                    .GroupBy(b => b.DateRead!.Value.Year)
                    .Select(g => new { Year = g.Key, Count = g.Count() })
                    .ToListAsync();

                result = years.ToDictionary(y => y, y => 0);

                foreach (var item in query)
                {
                    result[item.Year] = item.Count;
                }
            }

            return result;
        }

        #endregion
    }
}