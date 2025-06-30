using BookTracker.Entities;
using BookTracker.Repositories;

public class BookManager
{
    /// <summary>
    /// The books
    /// </summary>
    private List<Book> books;

    /// <summary>
    /// The next identifier
    /// </summary>
    private int nextId;

    /// <summary>
    /// The repo
    /// </summary>
    private readonly BookRepository repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookManager"/> class.
    /// </summary>
    public BookManager()
    {
        repo = new BookRepository();
        books = BookRepository.Load();
        nextId = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
    }

    /// <summary>
    /// Adds the book.
    /// </summary>
    /// <param name="book">The book.</param>
    public void AddBook(Book book)
    {
        book.Id = nextId++;
        books.Add(book);
        BookRepository.Save(books);
    }

    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns></returns>
    public List<Book> GetAllBooks() => books;

    /// <summary>
    /// Finds the book by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Book? FindBookById(int id) => books.FirstOrDefault(b => b.Id == id);

    /// <summary>
    /// Deletes the book.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteBook(int id)
    {
        var book = FindBookById(id);
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
    public void EditBook(int id, Action<Book> editAction)
    {
        var book = FindBookById(id);
        if (book != null)
        {
            editAction(book);
            BookRepository.Save(books);
        }
    }
}