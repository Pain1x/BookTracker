using System.Collections.Generic;
using System.Threading.Tasks;
using BookTracker.BLL.Models;

namespace BookTracker.BLL.Abstractions;

public interface IGenreService
{
    Task<IEnumerable<GenreModel>> GetGenresForDropdownAsync(string? searchTerm);
}