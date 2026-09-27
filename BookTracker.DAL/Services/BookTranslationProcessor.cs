using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Enums;
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
            var translatedTitle = await textTranslator.TranslateAsync(job.Title, job.TargetLanguage);
            var translatedAuthorName = await textTranslator.TranslateAsync(job.AuthorName, job.TargetLanguage);
            var translatedGenreName = await textTranslator.TranslateAsync(job.Genre, job.TargetLanguage);

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                await using var context = await contextFactory.CreateDbContextAsync();
                var targetLanguage = await GetOrCreateLanguageAsync(context, job.TargetLanguage);
                
                await EnsureTranslationAsync<BookTranslation>(
                    context,
                    job.BookPk,
                    targetLanguage.LanguagePk,
                    translatedTitle,
                    new BookTranslation { BookPk = job.BookPk, LanguagePk = targetLanguage.LanguagePk }
                );

                await EnsureTranslationAsync<AuthorTranslation>(
                    context,
                    job.AuthorPk,
                    targetLanguage.LanguagePk,
                    translatedAuthorName,
                    new AuthorTranslation { AuthorPk = job.AuthorPk, LanguagePk = targetLanguage.LanguagePk }
                );

                await EnsureTranslationAsync<GenreTranslation>(
                    context,
                    job.GenrePk,
                    targetLanguage.LanguagePk,
                    translatedGenreName,
                    new GenreTranslation { GenrePk = job.GenrePk, LanguagePk = targetLanguage.LanguagePk }
                );

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Ensures a translation record exists for the given entity and language, updating it if necessary or creating it if missing.
        /// </summary>
        /// <typeparam name="TTrans">The type of the translation entity (e.g., BookTranslation, AuthorTranslation).</typeparam>
        /// <param name="context">The current database context.</param>
        /// <param name="entityPk">The primary key of the associated entity (e.g., Guid for Book).</param>
        /// <param name="languagePk">The primary key of the target language.</param>
        /// <param name="translatedName">The new translated string value.</param>
        /// <param name="translationEntity">Entity to translate.</param>
        private static async Task EnsureTranslationAsync<TTrans>(
            BooksDbContext context,
            Guid entityPk,
            byte languagePk,
            string translatedName,
            ITranslationEntity translationEntity) where TTrans : class
        {
            switch (translationEntity)
            {
                case BookTranslation:
                    await using(context);
                    var existingBookTranslation = await context.Set<BookTranslation>()
                        .FirstOrDefaultAsync(t => t.BookPk == entityPk && t.LanguagePk == languagePk);

                    if (existingBookTranslation != null)
                    {
                        existingBookTranslation.Title = translatedName;
                        return;
                    }

                    var newBookTranslation = new BookTranslation
                    {
                        BookPk = entityPk,
                        LanguagePk = languagePk,
                        Title = translatedName
                    };

                    await context.BookTranslations.AddAsync(newBookTranslation);
                    break;

                case AuthorTranslation:
                    await using(context);
                    var existingAuthorTranslation = await context.Set<AuthorTranslation>()
                        .FirstOrDefaultAsync(t => t.AuthorPk == entityPk && t.LanguagePk == languagePk);

                    if (existingAuthorTranslation != null)
                    {
                        existingAuthorTranslation.Name = translatedName;
                        return;
                    }

                    var newAuthorTranslation = new AuthorTranslation
                    {
                        AuthorPk = entityPk,
                        LanguagePk = languagePk,
                        Name = translatedName
                    };
                    await context.AuthorTranslations.AddAsync(newAuthorTranslation);
                    break;

                case GenreTranslation:
                    await using(context);
                    var existingGenreTranslation = await context.Set<GenreTranslation>()
                        .FirstOrDefaultAsync(t => t.GenrePk == entityPk && t.LanguagePk == languagePk);

                    if (existingGenreTranslation != null)
                    {
                        existingGenreTranslation.Name = translatedName;
                        return;
                    }

                    var newGenreTranslation = new GenreTranslation
                    {
                        GenrePk = entityPk,
                        LanguagePk = languagePk,
                        Name = translatedName
                    };
                    await context.GenreTranslations.AddAsync(newGenreTranslation);
                    break;

                default:
                    throw new NotSupportedException($"Translation type {typeof(TTrans).Name} is not supported by this helper method.");
            }
        }

        /// <summary>
        /// Retrieves or creates a Language entry in the database for the target language code.
        /// </summary>
        private static async Task<Language> GetOrCreateLanguageAsync(BooksDbContext context, Languages languageName)
        {
            var language = await context.Languages
                .FirstOrDefaultAsync(l => l.LanguagePk == (byte)languageName);

            if (language != null)
            {
                return language;
            }

            var maxLanguagePk = await context.Languages
                .Select(l => (byte?)l.LanguagePk)
                .MaxAsync() ?? 0;

            language = new Language
            {
                LanguagePk = (byte)(maxLanguagePk + 1),
                LanguageName = languageName.ToString()
            };

            await context.Languages.AddAsync(language);
            await context.SaveChangesAsync();

            return language;
        }
    }
}