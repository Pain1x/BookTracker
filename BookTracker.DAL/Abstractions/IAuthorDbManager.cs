using BookTracker.DAL.Entities.Authors;

namespace BookTracker.DAL.Abstractions
{
    public interface IAuthorDbManager
    {
        Task<IEnumerable<Author>> GetAuthorsForSearchableDropdown(string? searchTerm);
    }
}