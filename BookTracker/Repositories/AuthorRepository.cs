using BookTracker.Entities;
using System.Text.Json;

namespace BookTracker.Repositories
{
    public class AuthorRepository
    {
        /// <summary>
        /// The file path
        /// </summary>
        private const string FilePath = "authors.json";

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static List<Author> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Author>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Author>>(json) ?? new List<Author>();
        }

        /// <summary>
        /// Saves the specified authors.
        /// </summary>
        /// <param name="authors">The authors.</param>
        public static void Save(List<Author> authors)
        {
            var json = JsonSerializer.Serialize(authors, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(FilePath, json);
        }
    }
}
