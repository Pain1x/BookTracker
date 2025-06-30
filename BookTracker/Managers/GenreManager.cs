using BookTracker.Entities;
using BookTracker.Repositories;

namespace BookTracker.Managers
{
    public class GenreManager
    {
        /// <summary>
        /// The genres
        /// </summary>
        private List<Genre> genres;

        public GenreManager()
        {
            genres = GenreRepository.Load();
        }

        /// <summary>
        /// Adds the genre.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Genre AddGenre(string name)
        {
            var existing = genres.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                return existing;

            var genre = new Genre { Name = name };
            genres.Add(genre);
            GenreRepository.Save(genres);
            return genre;
        }

        /// <summary>
        /// Finds the genre by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Genre? FindGenreById(Guid id)
        {
            return genres.FirstOrDefault(g => g.GenrePK == id);
        }

        /// <summary>
        /// Finds the name of the genre by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Genre? FindGenreByName(string name)
        {
            return genres.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lists the genres.
        /// </summary>
        public void ListGenres()
        {
            Console.WriteLine("Genres:");
            foreach (var genre in genres.OrderBy(x => x.Name))
            {
                Console.WriteLine($"{genre.GenrePK} - {genre.Name}");
            }
        }

        // <summary>
        /// Deletes the name of the author by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public void DeleteGenreByName(string name)
        {
            var genre = genres.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (genre != null)
            {
                genres.Remove(genre);
                GenreRepository.Save(genres);

                Console.WriteLine($"The {genre.Name} is deleted");
            }
            Console.WriteLine("This genre is not present in database");
        }
    }
}
