using BookTracker.Entities;
using System.Text.Json;

namespace BookTracker.Repositories
{
    public class BookRepository
    {
        /// <summary>
        /// The file path
        /// </summary>
        private const string FilePath = "books.json";

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static List<Book> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Book>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        /// <summary>
        /// Saves the specified books.
        /// </summary>
        /// <param name="books">The books.</param>
        public static void Save(List<Book> books)
        {
            var json = JsonSerializer.Serialize(books, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(FilePath, json);
        }
    }
}