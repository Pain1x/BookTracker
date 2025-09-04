using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities;

namespace BookTracker.Managers
{
    public class GenreDbManager(BooksDbContext booksDbContext) : BaseDbManager(booksDbContext)
    {
        /// <summary>
        /// The genres
        /// </summary>
        private List<Genre> _genres;

        /// <summary>
        /// Adds the genre.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Genre AddGenre(string name)
        {
            var existing = _genres.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                return existing;

            var genre = new Genre { Name = name };
            _genres.Add(genre);
            return genre;
        }

        /// <summary>
        /// Finds the genre by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Genre? FindGenreById(Guid id)
        {
            return _genres.FirstOrDefault(g => g.GenrePk == id);
        }

        /// <summary>
        /// Finds the name of the genre by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Genre? FindGenreByName(string name)
        {
            return _genres.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lists the genres.
        /// </summary>
        public void ListGenres()
        {
            Console.WriteLine("Genres:");
            foreach (var genre in _genres.OrderBy(x => x.Name))
            {
                Console.WriteLine($"{genre.GenrePk} - {genre.Name}");
            }
        }

        // <summary>
        /// Deletes the name of the author by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public void DeleteGenreByName(string name)
        {
            var genre = _genres.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (genre != null)
            {
                _genres.Remove(genre);

                Console.WriteLine($"The {genre.Name} is deleted");
            }
            Console.WriteLine("This genre is not present in database");
        }
    }
}
