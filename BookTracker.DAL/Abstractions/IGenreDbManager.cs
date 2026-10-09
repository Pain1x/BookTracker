using BookTracker.DAL.Entities.Genres;

namespace BookTracker.DAL.Abstractions
{
    public interface IGenreDbManager
    {
        Task<IEnumerable<Genre>> GetGenresForSearchableDropdown(string? searchTerm);
    }
}
