using System.Collections.Generic;
using System.Threading.Tasks;
using BookTracker.BLL.Models;

namespace BookTracker.BLL.Abstractions
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorModel>> GetAuthorsForDropdownAsync(string? searchTerm);
    }
}