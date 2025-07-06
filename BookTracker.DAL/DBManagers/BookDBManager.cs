using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities;
using BookTracker.Managers;
using Microsoft.EntityFrameworkCore;

public class BookDBManager
{
    /// <summary>
    /// The books
    /// </summary>
    private List<Book> books;

    private BooksDbContext BooksDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDBManager"/> class.
    /// </summary>
    public BookDBManager(BooksDbContext booksDBContext)
    {
        BooksDbContext = booksDBContext;
    }

    /// <summary>
    /// Adds the book.
    /// </summary>
    /// <param name="book">The book.</param>
    public void AddBook(Book book)
    {
        book.BookPK = Guid.NewGuid();
        BooksDbContext.Add(book);
        BooksDbContext.SaveChanges();
    }

    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns></returns>
    public List<Book> GetAllBooks() => BooksDbContext.Books.Include(b => b.Author).Include(b => b.Genre).OrderBy(b => b.Title).ToList();

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

            BooksDbContext.Remove(book);
            BooksDbContext.SaveChanges();
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

            BooksDbContext.Update(book);
            BooksDbContext.SaveChanges();
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
    public int CountBooksByAuthor(string authorName, AuthorDBManager authorManager)
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
    public int CountBooksByGenre(string genreName, GenreDBManager genreManager)
    {
        var genre = genreManager.FindGenreByName(genreName);
        if (genre == null) return 0;

        return books.Count(b => b.GenrePK == genre.GenrePK);
    }
}