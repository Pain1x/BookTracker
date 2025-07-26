using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBManagers
{

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDBManager"/> class.
    /// </summary>
    public class BookDBManager(BooksDbContext booksDBContext) : BaseDBManager(booksDBContext), IBookDBManager
    {
        ///<inheritdoc/>
        public async Task AddBook(Book book)
        {
            var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == book.Author.Name);
            if (author == null)
            {
                author = new Author
                {
                    AuthorPK = Guid.NewGuid(),
                    Name = book.Author.Name
                };

                await BooksDbContext.Authors.AddAsync(author);
                await BooksDbContext.SaveChangesAsync();
            }

            var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == book.Genre.Name);
            if (genre == null)
            {
                genre = new Genre
                {
                    GenrePK = Guid.NewGuid(),
                    Name = book.Genre.Name
                };

                await BooksDbContext.Genres.AddAsync(genre);
                await BooksDbContext.SaveChangesAsync();
            }

            var bookToSave = new Book
            {
                BookPK = Guid.NewGuid(),
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
        public async Task EditBook(Book updatedBook)
        {
            var book = await BooksDbContext.Books.FirstOrDefaultAsync(b => b.BookPK == updatedBook.BookPK);
            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == updatedBook.Author.Name);
            if (author == null)
            {
                author = new Author
                {
                    AuthorPK = Guid.NewGuid(),
                    Name = updatedBook.Author.Name
                };
                await BooksDbContext.Authors.AddAsync(author);
                await BooksDbContext.SaveChangesAsync();
            }

            var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == updatedBook.Genre.Name);
            if (genre == null)
            {
                genre = new Genre
                {
                    GenrePK = Guid.NewGuid(),
                    Name = updatedBook.Genre.Name
                };
                await BooksDbContext.Genres.AddAsync(genre);
                await BooksDbContext.SaveChangesAsync();
            }

            book.Title = updatedBook.Title;
            book.Author = author;
            book.Genre = genre;
            book.DateRead = updatedBook.DateRead?.ToUniversalTime();
            book.Rating = updatedBook.Rating;
            book.Notes = updatedBook.Notes;

            await BooksDbContext.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task DeleteBook(Guid bookPK)
        {
            var book = await BooksDbContext.Books.FirstOrDefaultAsync(b => b.BookPK == bookPK);
            if (book != null)
            {
                BooksDbContext.Books.Remove(book);
                await BooksDbContext.SaveChangesAsync();
            }
        }

        ///<inheritdoc/>
        public Task<List<Book>> GetAllBooks() => BooksDbContext.Books
                                                               .Include(b => b.Author)
                                                               .Include(b => b.Genre)
                                                               .OrderBy(b => b.Title)
                                                               .ToListAsync();

        ///<inheritdoc/>
        public Task<Book?> FindBookById(Guid bookPK) => BooksDbContext.Books.FirstOrDefaultAsync(b => b.BookPK == bookPK);

        ///<inheritdoc/>
        public Task<int> CountBooksByYear(int year) => BooksDbContext.Books.CountAsync(b => b.DateRead.HasValue && b.DateRead.Value.Year == year);

        ///<inheritdoc/>
        public async Task<int> CountBooksByAuthor(string authorName)
        {
            var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
            if (author == null)
            {
                return 0;
            }

            return await BooksDbContext.Books.CountAsync(b => b.Author.AuthorPK == author.AuthorPK);
        }

        ///<inheritdoc/>
        public async Task<int> CountBooksByGenre(string genreName)
        {
            var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
            if (genre == null)
            {
                return 0;
            }

            return await BooksDbContext.Books.CountAsync(b => b.Genre.GenrePK == genre.GenrePK);
        }
    }
}