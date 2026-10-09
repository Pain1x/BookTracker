using System.Collections.Generic;
using System.Threading.Tasks;
using BookTracker.BLL.Models;
using BookTracker.BLL.Abstractions;
using BookTracker.DAL.Abstractions;
using AutoMapper;

namespace BookTracker.BLL.Services
{
    public class GenreService(IGenreDbManager genreDbManager, IMapper mapper) : IGenreService
    {
        public async Task<IEnumerable<GenreModel>> GetGenresForDropdownAsync(string? searchTerm)
        {
            var genres = await genreDbManager.GetGenresForSearchableDropdown(searchTerm);
            return mapper.Map<IEnumerable<GenreModel>>(genres);
        }
    }
}