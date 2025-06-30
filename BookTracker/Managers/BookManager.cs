using BookTracker.Entities;
using BookTracker.Managers;
using BookTracker.Models;
using BookTracker.Repositories;

public class BookManager
{
    /// <summary>
    /// The books
    /// </summary>
    private List<Book> books;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookManager"/> class.
    /// </summary>
    public BookManager()
    {
        books = BookRepository.Load();
    }

    /// <summary>
    /// Adds the book.
    /// </summary>
    /// <param name="book">The book.</param>
    public void AddBook(Book book)
    {
        book.BookPK = Guid.NewGuid();
        books.Add(book);
        BookRepository.Save(books);
    }

    /// <summary>
    /// Gets all books detailed.
    /// </summary>
    /// <param name="authorManager">The author manager.</param>
    /// <param name="genreManager">The genre manager.</param>
    /// <returns></returns>
    public List<BookModel> GetAllBooks(AuthorManager authorManager, GenreManager genreManager)
    {
        return books.Select(book =>
        {
            var author = authorManager.FindAuthorById(book.AuthorPK);
            var genre = genreManager.FindGenreById(book.GenrePK);

            return new BookModel
            {
                BookPK = book.BookPK,
                Title = book.Title,
                AuthorName = author?.Name ?? "Unknown Author",
                GenreName = genre?.Name ?? "Unknown Genre",
                DateRead = book.DateRead,
                Rating = book.Rating
            };
        }).ToList();
    }

    /// <summary>
    /// Finds the book by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Book? FindBookById(Guid bookPK) => books.FirstOrDefault(b => b.BookPK == bookPK);

    /// <summary>
    /// Deletes the book.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteBook(Guid bookPK)
    {
        var book = FindBookById(bookPK);
        if (book != null)
        {
            books.Remove(book);
            BookRepository.Save(books);
        }
    }

    /// <summary>
    /// Edits the book.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="editAction">The edit action.</param>
    public void EditBook(Guid bookPK, Action<Book> editAction)
    {
        var book = FindBookById(bookPK);
        if (book != null)
        {
            editAction(book);
            BookRepository.Save(books);
        }
    }

    /// <summary>
    /// Counts the books by year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    public int CountBooksByYear(int year) => books.Count(b => b.DateRead.HasValue && b.DateRead.Value.Year == year);

    /// <summary>
    /// Counts the books by author.
    /// </summary>
    /// <param name="authorName">Name of the author.</param>
    /// <param name="authorManager">The author manager.</param>
    /// <returns></returns>
    public int CountBooksByAuthor(string authorName, AuthorManager authorManager)
    {
        var author = authorManager.FindAuthorByName(authorName);
        if (author == null) return 0;

        return books.Count(b => b.AuthorPK == author.AuthorPK);
    }

    /// <summary>
    /// Counts the books by genre.
    /// </summary>
    /// <param name="genreName">Name of the genre.</param>
    /// <param name="genreManager">The genre manager.</param>
    /// <returns></returns>
    public int CountBooksByGenre(string genreName, GenreManager genreManager)
    {
        var genre = genreManager.FindGenreByName(genreName);
        if (genre == null) return 0;

        return books.Count(b => b.GenrePK == genre.GenrePK);
    }
}