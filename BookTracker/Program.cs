using AutoMapper;
using BookTracker.AutoMapper;
using BookTracker.Entities;
using BookTracker.Managers;

namespace BookTracker
{
    internal class Program
    {
        /// <summary>
        /// The manager
        /// </summary>
        static BookManager BookManager = new();

        static AuthorManager authorManager = new AuthorManager();
        static GenreManager genreManager = new GenreManager();

        static MapperConfiguration? mapperConfiguration;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Initialize AutoMapper
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BooksProfile>();
            });

            while (true)
            {
                Console.Clear();
                Console.WriteLine("📚 Book Tracker");
                Console.WriteLine("1. List books");
                Console.WriteLine("2. Add book");
                Console.WriteLine("3. Edit book");
                Console.WriteLine("4. Delete book");
                //Console.WriteLine("5. Export into database");
                Console.WriteLine("6. Books read in a year");
                Console.WriteLine("7. Count books by author");
                Console.WriteLine("8. Count books by genre");
                Console.WriteLine("9. Add author");
                Console.WriteLine("10. Add Genre");
                Console.WriteLine("11. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ListBooks(); break;
                    case "2": AddBook(); break;
                    case "3": EditBook(); break;
                    case "4": DeleteBook(); break;
                    //case "5": ExportToDatabase(); break;
                    case "6": ShowBooksReadInYear(); break;
                    case "7": CountBooksByAuthor(); break;
                    case "8": CountBooksByGenre(); break;
                    case "9": AddAuthor(); break;
                    case "10": AddGenre(); break;
                    case "11": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Lists the books.
        /// </summary>
        static void ListBooks()
        {
            var bookModels = BookManager.GetAllBooks(authorManager, genreManager);
            if (bookModels.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            foreach (var book in bookModels)
            {
                Console.WriteLine($"{book.Title} by {book.AuthorName} - Genre: {book.GenreName} - Read on: {book.DateRead?.ToString("yyyy-MM-dd") ?? "N/A"} - Rating: {book.Rating}");
            }
        }

        /// <summary>
        /// Adds the book.
        /// </summary>
        static void AddBook()
        {
            Console.Write("Enter book title: ");
            var title = Console.ReadLine() ?? "";

            // Select or add author
            Console.WriteLine("Choose an author or type a new one:");
            authorManager.ListAuthors();
            Console.Write("Author name: ");
            var authorName = Console.ReadLine() ?? "";
            var author = authorManager.AddAuthor(authorName);

            // Select or add genre
            Console.WriteLine("Choose a genre or type a new one:");
            genreManager.ListGenres();
            Console.Write("Genre name: ");
            var genreName = Console.ReadLine() ?? "";
            var genre = genreManager.AddGenre(genreName);

            Console.Write("Enter rating (1-5): ");
            int.TryParse(Console.ReadLine(), out var rating);

            Console.Write("Enter date read (yyyy-MM-dd) or leave blank: ");
            DateTime? dateRead = null;
            var dateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out var parsedDate))
                dateRead = parsedDate;

            var book = new Book
            {
                Title = title,
                AuthorPK = author.AuthorPK,
                GenrePK = genre.GenrePK,
                Rating = rating,
                DateRead = dateRead
            };

            BookManager.AddBook(book);
            Console.WriteLine("Book added!");
        }

        /// <summary>
        /// Edits the book.
        /// </summary>
        static void EditBook()
        {
            Console.Write("Enter book ID to edit: ");
            if (!Guid.TryParse(Console.ReadLine(), out var bookPK)) return;

            BookManager.EditBook(bookPK, book =>
            {
                Console.WriteLine($"Editing Book: {book.Title}");

                Console.Write($"New title (leave blank to keep '{book.Title}'): ");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle))
                    book.Title = newTitle;

                // Edit author
                Console.WriteLine("Choose an author or type a new one:");
                authorManager.ListAuthors();
                Console.Write($"New author (leave blank to keep current): ");
                var newAuthorName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAuthorName))
                {
                    var newAuthor = authorManager.AddAuthor(newAuthorName);
                    book.AuthorPK = newAuthor.AuthorPK;
                }

                // Edit genre
                Console.WriteLine("Choose a genre or type a new one:");
                genreManager.ListGenres();
                Console.Write($"New genre (leave blank to keep current): ");
                var newGenreName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newGenreName))
                {
                    var newGenre = genreManager.AddGenre(newGenreName);
                    book.GenrePK = newGenre.GenrePK;
                }

                Console.Write($"New rating (1–5, current {book.Rating}): ");
                if (int.TryParse(Console.ReadLine(), out var newRating))
                    book.Rating = newRating;

                Console.Write($"New date read (current {book.DateRead?.ToString("yyyy-MM-dd") ?? "none"}, format yyyy-MM-dd, leave blank to keep): ");
                var newDateStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDateStr) && DateTime.TryParse(newDateStr, out var newDate))
                    book.DateRead = newDate;
            });

            Console.WriteLine("Book updated.");
        }

        /// <summary>
        /// Deletes the book.
        /// </summary>
        static void DeleteBook()
        {
            Console.Write("Enter book ID to delete: ");
            if (Guid.TryParse(Console.ReadLine(), out var bookPK))
            {
                if (BookManager.FindBookById(bookPK) != null)
                {
                    BookManager.DeleteBook(bookPK);
                    Console.WriteLine("Book deleted.");
                }
            }

            Console.WriteLine("Book is not present in library.");
        }

        /// <summary>
        /// Shows the books read in year.
        /// </summary>
        static void ShowBooksReadInYear()
        {
            Console.Write("Enter year (e.g. 2025): ");
            if (int.TryParse(Console.ReadLine(), out var year))
            {
                int count = BookManager.CountBooksByYear(year);
                Console.WriteLine($"You have read {count} books in {year}.");
            }
            else
            {
                Console.WriteLine("Invalid year.");
            }
        }

        ///// <summary>
        ///// Exports to database.
        ///// </summary>
        //static void ExportToDatabase()
        //{
        //    var books = BookManager.GetAllBooks(authorManager, genreManager);
        //    var dbImporter = new BookDbImporter("your-connection-string-here");
        //    dbImporter.InsertBooks(books);
        //    Console.WriteLine("Exported to database successfully.");
        //}

        /// <summary>
        /// Counts the books by author.
        /// </summary>
        static void CountBooksByAuthor()
        {
            Console.Write("Enter author name: ");
            var authorName = Console.ReadLine() ?? "";

            int count = BookManager.CountBooksByAuthor(authorName, authorManager);
            Console.WriteLine($"You have read {count} books by {authorName}.");
        }

        /// <summary>
        /// Counts the books by genre.
        /// </summary>
        static void CountBooksByGenre()
        {
            Console.Write("Enter genre: ");
            var genreName = Console.ReadLine() ?? "";

            int count = BookManager.CountBooksByGenre(genreName, genreManager);
            Console.WriteLine($"You have read {count} books in the {genreName} genre.");
        }

        /// <summary>
        /// Adds the author.
        /// </summary>
        static void AddAuthor()
        {
            Console.Write("Enter author name: ");
            var name = Console.ReadLine() ?? "";
            var author = authorManager.AddAuthor(name);
            Console.WriteLine($"Author '{author.Name}' added with ID {author.AuthorPK}");
        }

        /// <summary>
        /// Adds the genre.
        /// </summary>
        static void AddGenre()
        {
            Console.Write("Enter genre name: ");
            var name = Console.ReadLine() ?? "";
            var genre = genreManager.AddGenre(name);
            Console.WriteLine($"Genre '{genre.Name}' added with ID {genre.GenrePK}");
        }
    }
}
