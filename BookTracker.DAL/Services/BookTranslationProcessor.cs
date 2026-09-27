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
            var translatedTitle = await textTranslator.TranslateAsync(job.Title, job.TargetLanguage);
            var translatedAuthorName = await textTranslator.TranslateAsync(job.AuthorName, job.TargetLanguage);
            var translatedGenreName = await textTranslator.TranslateAsync(job.Genre, job.TargetLanguage);

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                await using var context = await contextFactory.CreateDbContextAsync();
                var targetLanguage = await GetOrCreateLanguageAsync(context, job.TargetLanguage);

                // Use the generic helper for all three types
                await EnsureTranslationAsync(
                    context,
                    job.BookPk,
                    targetLanguage.LanguagePk,
                    translatedTitle,
                    () => new BookTranslation { BookPk = job.BookPk, LanguagePk = targetLanguage.LanguagePk }
                );

                await EnsureTranslationAsync(
                    context,
                    job.AuthorPk,
                    targetLanguage.LanguagePk,
                    translatedAuthorName,
                    () => new AuthorTranslation { AuthorPk = job.AuthorPk, LanguagePk = targetLanguage.LanguagePk }
                );

                await EnsureTranslationAsync(
                    context,
                    job.GenrePk,
                    targetLanguage.LanguagePk,
                    translatedGenreName,
                    () => new GenreTranslation { GenrePk = job.GenrePk, LanguagePk = targetLanguage.LanguagePk }
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
        /// <param name="getExistingTranslation">A delegate to check if a translation already exists based on entity PK, language PK, and a partial match on the name/title field (to handle potential updates).</param>
        /// <param name="createEntityFactory">A function that creates a new, unattached instance of TTrans with initial foreign keys.</param>
        private static async Task EnsureTranslationAsync<TTrans>(
            BooksDbContext context,
            Guid entityPk, // Assuming all primary keys are Guid for simplification across types in this refactor scope. Needs review if AuthorPk/GenrePk change type.
            byte languagePk,
            string translatedName,
            Func<TTrans, Guid, byte, string, bool> getExistingTranslation,
            Func<TTrans> createEntityFactory) where TTrans : class
        {
            // Use reflection or a switch statement to dispatch based on the expected translation type
            // and its corresponding DbSet in the context. This keeps the helper generic while respecting EF Core structure.

            switch (typeof(TTrans))
            {
                case BookTranslation bookTranslation:
                    await using(context);
                    var existingTranslation = await context.Set<BookTranslation>()
                        .FirstOrDefaultAsync(t => t.BookPk == entityPk && t.LanguagePk == languagePk);

                    if (existingTranslation != null)
                    {
                        existingTranslation.Title = translatedName;
                        return;
                    }

                    var newTranslation = createEntityFactory();
                    // Note: We must rely on specific casting here as the factory returns an object.
                    var bookTrans = (BookTranslation)(object)newTranslation;
                    bookTrans.BookPk = entityPk;
                    bookTrans.LanguagePk = languagePk;
                    bookTrans.Title = translatedName;
                    await context.BookTranslations.AddAsync(bookTrans);
                    break;

                case AuthorTranslation authorTranslation:
                    await using(context);
                    var existingTranslation = await context.Set<AuthorTranslation>()
                        .FirstOrDefaultAsync(t => t.AuthorPk == entityPk && t.LanguagePk == languagePk);

                    if (existingTranslation != null)
                    {
                        existingTranslation.Name = translatedName;
                        return;
                    }

                    var newTranslation = createEntityFactory();
                    // Note: We must rely on specific casting here as the factory returns an object.
                    var authorTrans = (AuthorTranslation)(object)newTranslation;
                    authorTrans.AuthorPk = entityPk;
                    authorTrans.LanguagePk = languagePk;
                    authorTrans.Name = translatedName;
                    await context.AuthorTranslations.AddAsync(authorTrans);
                    break;

                case GenreTranslation genreTranslation:
                    await using(context);
                    var existingTranslation = await context.Set<GenreTranslation>()
                        .FirstOrDefaultAsync(t => t.GenrePk == entityPk && t.LanguagePk == languagePk);

                    if (existingTranslation != null)
                    {
                        existingTranslation.Name = translatedName;
                        return;
                    }

                    var newTranslation = createEntityFactory();
                    // Note: We must rely on specific casting here as the factory returns an object.
                    var genreTrans = (GenreTranslation)(object)newTranslation;
                    genreTrans.GenrePk = entityPk;
                    genreTrans.LanguagePk = languagePk;
                    genreTrans.Name = translatedName;
                    await context.GenreTranslations.AddAsync(genreTrans);
                    break;

                default:
                    // Handle unknown types gracefully in the future
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
            // Note: Keeping the SaveChanges here to ensure the PK is generated/available immediately.
            await context.SaveChangesAsync();

            return language;
        }
    }
}