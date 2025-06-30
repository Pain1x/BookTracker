using BookTracker.Entities;
using BookTracker.Helpers;

namespace BookTracker
{
    internal class Program
    {
        /// <summary>
        /// The manager
        /// </summary>
        static BookManager BookManager = new();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

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
                Console.WriteLine("7. Exit");
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
                    case "7": return;
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
            var books = BookManager.GetAllBooks();
            if (books.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine($"{book.Id}: {book.Title} in genre {book.Genre} by {book.Author} ({book.Rating}/5)");
            }
        }

        /// <summary>
        /// Adds the book.
        /// </summary>
        static void AddBook()
        {
            Console.Write("Title: ");
            var title = Console.ReadLine() ?? "";
            Console.Write("Author: ");
            var author = Console.ReadLine() ?? "";
            Console.Write("Genre: ");
            var genre = Console.ReadLine() ?? "";
            Console.Write("Rating (1-5): ");
            var rating = int.TryParse(Console.ReadLine(), out var r) ? r : 0;

            BookManager.AddBook(new Book
            {
                Title = title,
                Author = author,
                Genre = genre,
                DateRead = DateTime.Now,
                Rating = rating
            });

            Console.WriteLine("Book added!");
        }

        /// <summary>
        /// Edits the book.
        /// </summary>
        static void EditBook()
        {
            Console.Write("Enter book ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) return;

            BookManager.EditBook(id, book =>
            {
                Console.Write($"New title (leave blank to keep '{book.Title}'): ");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle)) book.Title = newTitle;

                Console.Write($"New genre (leave blank to keep '{book.Genre}'): ");
                var newGenre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newGenre)) book.Genre = newGenre;

                Console.Write($"New rating (1–5, leave blank to keep {book.Rating}): ");
                if (int.TryParse(Console.ReadLine(), out var newRating)) book.Rating = newRating;

                Console.Write($"New date read (current {book.DateRead?.ToString("yyyy-MM-dd") ?? "none"}, format yyyy-MM-dd, leave blank to keep): ");
                var newDateStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDateStr) && DateTime.TryParse(newDateStr, out var newDate))
                    book.DateRead = newDate;
            });

            Console.WriteLine("Book is updated.");
        }

        /// <summary>
        /// Deletes the book.
        /// </summary>
        static void DeleteBook()
        {
            Console.Write("Enter book ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out var id))
            {
                if (BookManager.FindBookById(id) != null)
                {
                    BookManager.DeleteBook(id);
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

        /// <summary>
        /// Exports to database.
        /// </summary>
        static void ExportToDatabase()
        {
            var books = BookManager.GetAllBooks();
            var dbImporter = new BookDbImporter("your-connection-string-here");
            dbImporter.InsertBooks(books);
            Console.WriteLine("Exported to database successfully.");
        }
    }
}
