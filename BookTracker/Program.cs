using AutoMapper;
using BookTracker.AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.BLL.Services;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities;
using BookTracker.Helpers;
using BookTracker.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookTracker
{
    internal class Program
    {

        #region Fields

        /// <summary>
        /// The author manager
        /// </summary>
        static AuthorDBManager AuthorsDBManager = new();

        /// <summary>
        /// The genre manager
        /// </summary>
        static GenreDBManager GenresDBManager = new();

        /// <summary>
        /// The mapper configuration
        /// </summary>
        static MapperConfiguration? mapperConfiguration;

        #endregion

        #region Main

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Initialize AutoMapper
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BooksProfile>();
            }, new LoggerFactory());

            //var optionsBuilder = new DbContextOptionsBuilder<BooksDbContext>();
            //optionsBuilder.UseNpgsql("Host=192.168.0.135:5432;Database=Books;Username=postgress;Password=admin1");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("📚 Book Tracker");
                Console.WriteLine("1. List books");
                Console.WriteLine("2. Add book");
                Console.WriteLine("3. Edit book");
                Console.WriteLine("4. Delete book");
                Console.WriteLine("5. Export into database");
                Console.WriteLine("6. Books read in a year");
                Console.WriteLine("7. Count books by author");
                Console.WriteLine("8. Count books by genre");
                Console.WriteLine("9. Add author");
                Console.WriteLine("10. Delete author");
                Console.WriteLine("11. Add Genre");
                Console.WriteLine("12. Delete Genre");
                Console.WriteLine("13. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ListBooks(); break;
                    // case "2": AddBook(); break;
                    //case "3": EditBook(); break;
                    //case "4": DeleteBook(); break;
                    //case "6": ShowBooksReadInYear(); break;
                    //case "7": CountBooksByAuthor(); break;
                    //case "8": CountBooksByGenre(); break;
                    //case "9": AddAuthor(); break;
                    //case "10": DeleteAuthor(); break;
                    //case "11": AddGenre(); break;
                    //case "12": DeleteGenre(); break;
                    case "13": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        #endregion

        #region Books

        /// <summary>
        /// Lists the books.
        /// </summary>
        static void ListBooks()
        {
            var mapper = mapperConfiguration.CreateMapper();

            BooksService BooksService = new(new BookDBManager(new BooksDbContext()), mapper);

            var books = BooksService.GetAllBooks();

            if (books.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} by {book.Author.Name} - Genre: {book.Genre.Name} - Read on: {book.DateRead?.ToString("yyyy-MM-dd") ?? "N/A"} - Rating: {book.Rating}");
            }
        }

        ///// <summary>
        ///// Adds the book.
        ///// </summary>
        //static void AddBook()
        //{
        //    Console.Write("Enter book title: ");
        //    var title = Console.ReadLine() ?? "";

        //    // Select or add author
        //    Console.WriteLine("Choose an author or type a new one:");
        //    AuthorsDBManager.ListAuthors();
        //    Console.Write("Author name: ");
        //    var authorName = Console.ReadLine() ?? "";
        //    var author = AuthorsDBManager.AddAuthor(authorName);

        //    // Select or add genre
        //    Console.WriteLine("Choose a genre or type a new one:");
        //    GenresDBManager.ListGenres();
        //    Console.Write("Genre name: ");
        //    var genreName = Console.ReadLine() ?? "";
        //    var genre = GenresDBManager.AddGenre(genreName);

        //    Console.Write("Enter rating (1-5): ");
        //    int.TryParse(Console.ReadLine(), out var rating);

        //    Console.Write("Enter date read (yyyy-MM-dd) or leave blank: ");
        //    DateTime? dateRead = null;
        //    var dateStr = Console.ReadLine();
        //    if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out var parsedDate))
        //        dateRead = parsedDate;

        //    var book = new Book
        //    {
        //        Title = title,
        //        AuthorPK = author.AuthorPK,
        //        GenrePK = genre.GenrePK,
        //        Rating = rating,
        //        DateRead = dateRead
        //    };


        //    Console.WriteLine("Book added!");
        //}

        ///// <summary>
        ///// Edits the book.
        ///// </summary>
        //static void EditBook()
        //{
        //    Console.Write("Enter book ID to edit: ");
        //    if (!Guid.TryParse(Console.ReadLine(), out var bookPK)) return;

        //    BooksDBManager.EditBook(bookPK, book =>
        //    {
        //        Console.WriteLine($"Editing Book: {book.Title}");

        //        Console.Write($"New title (leave blank to keep '{book.Title}'): ");
        //        var newTitle = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(newTitle))
        //            book.Title = newTitle;

        //        // Edit author
        //        Console.WriteLine("Choose an author or type a new one:");
        //        AuthorsDBManager.ListAuthors();
        //        Console.Write($"New author (leave blank to keep current): ");
        //        var newAuthorName = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(newAuthorName))
        //        {
        //            var newAuthor = AuthorsDBManager.AddAuthor(newAuthorName);
        //            book.AuthorPK = newAuthor.AuthorPK;
        //        }

        //        // Edit genre
        //        Console.WriteLine("Choose a genre or type a new one:");
        //        GenresDBManager.ListGenres();
        //        Console.Write($"New genre (leave blank to keep current): ");
        //        var newGenreName = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(newGenreName))
        //        {
        //            var newGenre = GenresDBManager.AddGenre(newGenreName);
        //            book.GenrePK = newGenre.GenrePK;
        //        }

        //        Console.Write($"New rating (1–5, current {book.Rating}): ");
        //        if (int.TryParse(Console.ReadLine(), out var newRating))
        //            book.Rating = newRating;

        //        Console.Write($"New date read (current {book.DateRead?.ToString("yyyy-MM-dd") ?? "none"}, format yyyy-MM-dd, leave blank to keep): ");
        //        var newDateStr = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(newDateStr) && DateTime.TryParse(newDateStr, out var newDate))
        //            book.DateRead = newDate;
        //    });

        //    Console.WriteLine("Book updated.");
        //}

        ///// <summary>
        ///// Deletes the book.
        ///// </summary>
        //static void DeleteBook()
        //{
        //    Console.Write("Enter book ID to delete: ");
        //    if (Guid.TryParse(Console.ReadLine(), out var bookPK))
        //    {
        //        if (BooksDBManager.FindBookById(bookPK) != null)
        //        {
        //            BooksDBManager.DeleteBook(bookPK);
        //            Console.WriteLine("Book deleted.");
        //        }
        //    }

        //    Console.WriteLine("Book is not present in library.");
        //}

        ///// <summary>
        ///// Shows the books read in year.
        ///// </summary>
        //static void ShowBooksReadInYear()
        //{
        //    Console.Write("Enter year (e.g. 2025): ");
        //    if (int.TryParse(Console.ReadLine(), out var year))
        //    {
        //        int count = BooksDBManager.CountBooksByYear(year);
        //        Console.WriteLine($"You have read {count} books in {year}.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Invalid year.");
        //    }
        //}

        /// <summary>
        /// Counts the books by author.
        /// </summary>
        //static void CountBooksByAuthor()
        //{
        //    Console.Write("Enter author name: ");
        //    var authorName = Console.ReadLine() ?? "";

        //    int count = BooksDBManager.CountBooksByAuthor(authorName, AuthorsDBManager);
        //    Console.WriteLine($"You have read {count} books by {authorName}.");
        //}

        ///// <summary>
        ///// Counts the books by genre.
        ///// </summary>
        //static void CountBooksByGenre()
        //{
        //    Console.Write("Enter genre: ");
        //    var genreName = Console.ReadLine() ?? "";

        //    int count = BooksDBManager.CountBooksByGenre(genreName, GenresDBManager);
        //    Console.WriteLine($"You have read {count} books in the {genreName} genre.");
        //}

        #endregion

        #region Authors

        ///// <summary>
        ///// Adds the author.
        ///// </summary>
        //static void AddAuthor()
        //{
        //    Console.Write("Enter author name: ");
        //    var name = Console.ReadLine() ?? "";

        //    var author = AuthorsDBManager.AddAuthor(name);
        //    Console.WriteLine($"Author '{author.Name}' added with ID {author.AuthorPK}");
        //}

        ///// <summary>
        ///// Deletes the author.
        ///// </summary>
        //static void DeleteAuthor()
        //{
        //    Console.Write("Enter author name to delete: ");
        //    var name = Console.ReadLine() ?? "";

        //    AuthorsDBManager.DeleteAuthorByName(name);
        //}

        #endregion

        #region Genres

        ///// <summary>
        ///// Adds the genre.
        ///// </summary>
        //static void AddGenre()
        //{
        //    Console.Write("Enter genre name: ");
        //    var name = Console.ReadLine() ?? "";
        //    var genre = GenresDBManager.AddGenre(name);
        //    Console.WriteLine($"Genre '{genre.Name}' added with ID {genre.GenrePK}");
        //}

        ///// <summary>
        ///// Deletes the genre.
        ///// </summary>
        //static void DeleteGenre()
        //{
        //    Console.Write("Enter genre name to delete: ");
        //    var name = Console.ReadLine() ?? "";

        //    GenresDBManager.DeleteGenreByName(name);
        //}

        #endregion
    }
}
