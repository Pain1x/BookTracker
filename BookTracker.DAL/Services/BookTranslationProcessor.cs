using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Languages;
using BookTracker.DAL.Entities.Translations;
using BookTracker.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.Services
{
    /// <summary>
    /// Processes and ensures translations exist for various book entities (Book, Author, Genre) in the database.
    /// </summary>
    public class BookTranslationProcessor(
        IDbContextFactory<BooksDbContext> contextFactory,
        ITextTranslator textTranslator) : IBookTranslationProcessor
    {
        /// <inheritdoc/>
        public async Task ProcessTranslationAsync(BookTranslationJob job)
        {
            var translatedTitleTask = textTranslator.TranslateAsync(job.Title, job.TargetLanguage);
            var translatedAuthorNameTask = textTranslator.TranslateAsync(job.AuthorName, job.TargetLanguage);
            var translatedGenreNameTask = textTranslator.TranslateAsync(job.Genre, job.TargetLanguage);

            await Task.WhenAll(translatedTitleTask, translatedAuthorNameTask, translatedGenreNameTask);

            await using var context = await contextFactory.CreateDbContextAsync();

            var bookTranslationTask = EnsureTranslationAsync(
                context,
                new BookTranslation
                {
                    BookPk = job.BookPk,
                    Title = await translatedTitleTask,
                    Language = new Language
                    {
                        LanguagePk = (byte)job.TargetLanguage
                    }
                }
            );
            
            var authorTranslationTask = EnsureTranslationAsync(
                context,
                new AuthorTranslation
                {
                    AuthorPk = job.AuthorPk,
                    Name = await translatedAuthorNameTask,
                    Language = new Language
                    {
                        LanguagePk = (byte)job.TargetLanguage
                    }
                }
            );
            
            var genreTranslationTask = EnsureTranslationAsync(
                context,
                new GenreTranslation
                {
                    GenrePk = job.GenrePk,
                    Name = await translatedGenreNameTask,
                    Language = new Language
                    {
                        LanguagePk = (byte)job.TargetLanguage
                    }
                }
            );

            await Task.WhenAll(bookTranslationTask, authorTranslationTask , genreTranslationTask);
            
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Ensures a translation record exists for the given entity and language.
        /// </summary>
        /// <param name="context">The current database context.</param>
        /// <param name="translationEntity">Entity to translate.</param>
        private async Task EnsureTranslationAsync(
            BooksDbContext context,
            TranslationEntity translationEntity)
        {
            switch (translationEntity)
            {
                case BookTranslation bookTranslation:
                    var existingBookTranslation = await context.Set<BookTranslation>()
                        .FirstOrDefaultAsync(t =>
                            t.BookPk == bookTranslation.BookPk && t.LanguagePk == bookTranslation.Language.LanguagePk);

                    if (existingBookTranslation != null)
                    {
                        return;
                    }

                    await context.BookTranslations.AddAsync(bookTranslation);
                    break;

                case AuthorTranslation authorTranslation:
                    var existingAuthorTranslation = await context.Set<AuthorTranslation>()
                        .FirstOrDefaultAsync(t =>
                            t.AuthorPk == authorTranslation.AuthorPk &&
                            t.LanguagePk == authorTranslation.Language.LanguagePk);

                    if (existingAuthorTranslation != null)
                    {
                        return;
                    }

                    await context.AuthorTranslations.AddAsync(authorTranslation);
                    break;

                case GenreTranslation genreTranslation:
                    var existingGenreTranslation = await context.Set<GenreTranslation>()
                        .FirstOrDefaultAsync(t =>
                            t.GenrePk == genreTranslation.GenrePk &&
                            t.LanguagePk == genreTranslation.Language.LanguagePk);

                    if (existingGenreTranslation != null)
                    {
                        return;
                    }

                    await context.GenreTranslations.AddAsync(genreTranslation);
                    break;

                default:
                    return;
            }
        }
    }
}