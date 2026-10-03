using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Translations;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookTracker.DAL.Abstractions;

public interface ITranslationsDbManager
{
    Task CreateBookTranslation(BookTranslation bookTranslation, BooksDbContext context, IDbContextTransaction transaction);
    
    Task CreateAuthorTranslation(AuthorTranslation authorTranslation, BooksDbContext context, IDbContextTransaction transaction);
    
    Task CreateGenreTranslation(GenreTranslation genreTranslation, BooksDbContext context, IDbContextTransaction transaction);
}