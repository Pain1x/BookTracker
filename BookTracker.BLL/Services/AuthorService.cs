using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Models;
using BookTracker.DAL.Abstractions;

namespace BookTracker.BLL.Services;

public class AuthorService(IAuthorDbManager authorDbManager, IMapper mapper) : IAuthorService
{
    public async Task<IEnumerable<AuthorModel>> GetAuthorsForDropdownAsync(string? searchTerm)
    {
        var authors = await authorDbManager.GetAuthorsForSearchableDropdown(searchTerm);
        return mapper.Map<IEnumerable<AuthorModel>>(authors);
    }
}