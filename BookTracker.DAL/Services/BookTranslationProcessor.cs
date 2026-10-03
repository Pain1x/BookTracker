using BookTracker.Common.Extensions;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.Entities.Languages;
using BookTracker.DAL.Entities.Translations;
using BookTracker.Jobs.Abstractions;
using BookTracker.Jobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookTracker.DAL.Services
{
    /// <summary>
    /// Processes and ensures translations exist for various book entities (Book, Author, Genre) in the database.
    /// </summary>
    public class BookTranslationProcessor(
        IDbContextFactory<BooksDbContext> contextFactory,
        ITextTranslator textTranslator,
        ITranslationsDbManager translationsDbManager) : IBookTranslationProcessor
    {
        /// <inheritdoc/>
        public async Task ProcessTranslationAsync(BookTranslationJob job)
        {
            if (job.BookPk == Guid.Empty || job.AuthorPk == Guid.Empty || job.GenrePk == Guid.Empty)
            {
                throw new ArgumentException("One or more primary keys in the translation job are invalid.");
            }

            var translatedTitleTask = textTranslator.TranslateAsync(job.Title, job.TargetLanguage);
            var translatedAuthorNameTask = textTranslator.TranslateAsync(job.AuthorName, job.TargetLanguage);
            var translatedGenreNameTask = textTranslator.TranslateAsync(job.Genre, job.TargetLanguage);

            await Task.WhenAll(translatedTitleTask, translatedAuthorNameTask, translatedGenreNameTask);

            await using var context = await contextFactory.CreateDbContextAsync();

            var language = await context.Set<Language>().FindAsync((byte)job.TargetLanguage.InvertLanguage());

            var book = await context.Set<Book>().FindAsync(job.BookPk);
            var author = await context.Set<Author>().FindAsync(job.AuthorPk);
            var genre = await context.Set<Genre>().FindAsync(job.GenrePk);

            await using var transaction = await context.Database.BeginTransactionAsync();
            {
                await EnsureTranslationAsync(
                    context,
                    new BookTranslation
                    {
                        Book = book,
                        Title = await translatedTitleTask,
                        Language = language
                    },
                    transaction
                );

                await EnsureTranslationAsync(
                    context,
                    new AuthorTranslation
                    {
                        Author = author,
                        Name = await translatedAuthorNameTask,
                        Language = language
                    },
                    transaction
                );

                await EnsureTranslationAsync(
                    context,
                    new GenreTranslation
                    {
                        GenrePk = job.GenrePk,
                        Genre = genre,
                        Name = await translatedGenreNameTask,
                        Language = language
                    },
                    transaction
                );

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        /// Ensures a translation record exists for the given entity and language.
        /// </summary>
        /// <param name="context">The current database context.</param>
        /// <param name="translationEntity">Entity to translate.</param>
        private async Task EnsureTranslationAsync(
            BooksDbContext context,
            TranslationEntity translationEntity,
            IDbContextTransaction transaction)
        {
            switch (translationEntity)
            {
                case BookTranslation bookTranslation:
                    try
                    {
                        await translationsDbManager.CreateBookTranslation(bookTranslation, context, transaction);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                    break;
                case AuthorTranslation authorTranslation:
                    try
                    {
                        await translationsDbManager.CreateAuthorTranslation(authorTranslation, context, transaction);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                    break;
                case GenreTranslation genreTranslation:
                    try
                    {
                        await translationsDbManager.CreateGenreTranslation(genreTranslation, context, transaction);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                    break;

                default:
                    return;
            }
        }
    }
}