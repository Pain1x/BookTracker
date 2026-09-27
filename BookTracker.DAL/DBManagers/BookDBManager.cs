using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.Entities.Languages;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBManagers
{
    public class BookDbManager(IDbContextFactory<BooksDbContext> contextFactory)
        : BaseDbManager(contextFactory), IBookDbManager
    {
        #region Implementation of IBookDbManager

        ///<inheritdoc/>
        public async Task AddBook(Book book, Languages targetLanguage)
        {
            var author = await BooksDbContext.Authors.FindAsync(book.Author.AuthorPk);

            if (author == null)
            {
                author = new Author
                {
                    AuthorPk = Guid.NewGuid(),
                    Name = book.Author.Name
                };

                await BooksDbContext.Authors.AddAsync(author);
                await BooksDbContext.SaveChangesAsync();
            }

            var genre = await BooksDbContext.Genres.FindAsync(book.Genre.GenrePk);

            if (genre == null)
            {
                genre = new Genre
                {
                    GenrePk = Guid.NewGuid(),
                    Name = book.Genre.Name
                };

                await BooksDbContext.Genres.AddAsync(genre);
                await BooksDbContext.SaveChangesAsync();
            }

            var bookToSave = new Book
            {
                BookPk = Guid.NewGuid(),
                Title = book.Title,
                Author = author,
                Genre = genre,
                DateRead = book.DateRead?.ToUniversalTime(),
                Rating = book.Rating,
                Notes = book.Notes
            };

            await BooksDbContext.Books.AddAsync(bookToSave);
            await BooksDbContext.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task UpdateBook(Book updatedBook, Languages targetLanguage)
        {
            var existing = await BooksDbContext.Books.FindAsync(updatedBook.BookPk);

            if (existing != null)
            {
                BooksDbContext.Entry(existing).CurrentValues.SetValues(updatedBook);
                await BooksDbContext.SaveChangesAsync();
            }
        }

        ///<inheritdoc/>
        public async Task<List<Book>> GetAllBooksLocalized(byte languagePk) => await BooksDbContext.Books
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

        ///<inheritdoc/>
        public async Task<Book?> FindBookByPkLocalized(Guid bookPk, byte languagePk) =>
            await BooksDbContext.Books
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

        ///<inheritdoc/>
        public async Task<Dictionary<int, int>> CountBooksByYears()
        {
            Dictionary<int, int> result;

            await using (BooksDbContext)
            {
                var currentYear = DateTime.UtcNow.Year;
                var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();
                // Get all books with a DateRead in the given years
                var yearSet = years.ToHashSet();

                var query = await BooksDbContext.Books
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