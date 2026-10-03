using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Translations;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookTracker.DAL.DBManagers;

using Microsoft.EntityFrameworkCore;

public class TranslationsDbManager(IDbContextFactory<BooksDbContext> contextFactory)
    : BaseDbManager(contextFactory), ITranslationsDbManager
{
    public async Task CreateBookTranslation(BookTranslation bookTranslation, BooksDbContext context,
        IDbContextTransaction transaction)
    {
        var existingBookTranslation = await context.Set<BookTranslation>()
            .FirstOrDefaultAsync(t =>
                t.BookPk == bookTranslation.Book.BookPk &&
                t.LanguagePk == bookTranslation.Language.LanguagePk);

        if (existingBookTranslation != null)
        {
            await transaction.RollbackAsync();
            return;
        }

        await context.BookTranslations.AddAsync(bookTranslation);
        await context.SaveChangesAsync();
    }

    public async Task CreateAuthorTranslation(AuthorTranslation authorTranslation, BooksDbContext context,
        IDbContextTransaction transaction)
    {
        var existingBookTranslation = await context.Set<AuthorTranslation>()
            .FirstOrDefaultAsync(a =>
                a.AuthorPk == authorTranslation.Author.AuthorPk &&
                a.LanguagePk == authorTranslation.Language.LanguagePk);

        if (existingBookTranslation != null)
        {
            await transaction.RollbackAsync();
            return;
        }

        await context.AuthorTranslations.AddAsync(authorTranslation);
        await context.SaveChangesAsync();
    }

    public async Task CreateGenreTranslation(GenreTranslation genreTranslation, BooksDbContext context,
        IDbContextTransaction transaction)
    {
        var existingBookTranslation = await context.Set<GenreTranslation>()
            .FirstOrDefaultAsync(g =>
                g.GenrePk == genreTranslation.Genre.GenrePk &&
                g.LanguagePk == genreTranslation.Language.LanguagePk);

        if (existingBookTranslation != null)
        {
            await transaction.RollbackAsync();
            return;
        }

        await context.GenreTranslations.AddAsync(genreTranslation);
        await context.SaveChangesAsync();
    }
}