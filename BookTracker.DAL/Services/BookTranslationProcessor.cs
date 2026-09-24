using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Languages;
using BookTracker.DAL.Entities.Translations;
using BookTracker.DAL.Models;

using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.Services
{
	public class BookTranslationProcessor(
		IDbContextFactory<BooksDbContext> contextFactory,
		ITextTranslator textTranslator) : IBookTranslationProcessor
	{
		public async Task ProcessTranslationAsync(BookTranslationJob job)
		{
			var translatedTitle = await textTranslator.TranslateAsync(job.Title, job.TargetLanguage);
			var translatedAuthorName = await textTranslator.TranslateAsync(job.AuthorName, job.TargetLanguage);
			var translatedGenreName = await textTranslator.TranslateAsync(job.Genre, job.TargetLanguage);

			for (var attempt = 1; attempt <= 2; attempt++)
			{
				await using var context = await contextFactory.CreateDbContextAsync();
				var targetLanguage = await GetOrCreateLanguageAsync(context, job.TargetLanguage);

				await EnsureBookTranslationAsync(context, job.BookPk, targetLanguage.LanguagePk, translatedTitle);
				await EnsureAuthorTranslationAsync(context, job.AuthorPk, targetLanguage.LanguagePk, translatedAuthorName);
				await EnsureGenreTranslationAsync(context, job.GenrePk, targetLanguage.LanguagePk, translatedGenreName);

				await context.SaveChangesAsync();
			}
		}

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

		private static async Task EnsureBookTranslationAsync(
			BooksDbContext context,
			Guid bookPk,
			byte languagePk,
			string translatedTitle)
		{
			var existingTranslation = await context.BookTranslations
				.FirstOrDefaultAsync(t => t.BookPk == bookPk && t.LanguagePk == languagePk);

			if (existingTranslation != null)
			{
				existingTranslation.Title = translatedTitle;
				return;
			}

			await context.BookTranslations.AddAsync(new BookTranslation
			{
				BookPk = bookPk,
				LanguagePk = languagePk,
				Title = translatedTitle
			});
		}

		private static async Task EnsureAuthorTranslationAsync(
			BooksDbContext context,
			Guid authorPk,
			byte languagePk,
			string translatedName)
		{
			var existingTranslation = await context.AuthorTranslations
				.FirstOrDefaultAsync(t => t.AuthorPk == authorPk && t.LanguagePk == languagePk);

			if (existingTranslation != null)
			{
				existingTranslation.Name = translatedName;
				return;
			}

			await context.AuthorTranslations.AddAsync(new AuthorTranslation
			{
				AuthorPk = authorPk,
				LanguagePk = languagePk,
				Name = translatedName
			});
		}

		private static async Task EnsureGenreTranslationAsync(
			BooksDbContext context,
			Guid genrePk,
			byte languagePk,
			string translatedName)
		{
			var existingTranslation = await context.GenreTranslations
				.FirstOrDefaultAsync(t => t.GenrePk == genrePk && t.LanguagePk == languagePk);

			if (existingTranslation != null)
			{
				existingTranslation.Name = translatedName;
				return;
			}

			await context.GenreTranslations.AddAsync(new GenreTranslation
			{
				GenrePk = genrePk,
				LanguagePk = languagePk,
				Name = translatedName
			});
		}
	}
}
