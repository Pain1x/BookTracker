using BookTracker.Entities;
using System.Text.Json;

namespace BookTracker.Repositories
{
    internal class GenreRepository
    {
        /// <summary>
        /// The file path
        /// </summary>
        private const string FilePath = "genres.json";

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public static List<Genre> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Genre>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Genre>>(json) ?? new List<Genre>();
        }

        /// <summary>
        /// Saves the specified genres.
        /// </summary>
        /// <param name="genres">The genres.</param>
        public static void Save(List<Genre> genres)
        {
            var json = JsonSerializer.Serialize(genres, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(FilePath, json);
        }
    }
}
