using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
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
		private const string UkrainianLanguageName = "Ukrainian";

		public async Task ProcessUkrainianTranslationAsync(BookTranslationJob job)
		{
			var translatedTitle = await textTranslator.TranslateToUkrainianAsync(job.Title);
			var translatedAuthorName = await textTranslator.TranslateToUkrainianAsync(job.AuthorName);
			var translatedGenreName = await textTranslator.TranslateToUkrainianAsync(job.GenreName);

			for (var attempt = 1; attempt <= 2; attempt++)
			{
				await using var context = await contextFactory.CreateDbContextAsync();
				var ukrainianLanguage = await GetOrCreateUkrainianLanguageAsync(context);

				await EnsureBookTranslationAsync(context, job.BookPk, ukrainianLanguage.LanguagePk, translatedTitle);
				await EnsureAuthorTranslationAsync(context, job.AuthorPk, ukrainianLanguage.LanguagePk, translatedAuthorName);
				await EnsureGenreTranslationAsync(context, job.GenrePk, ukrainianLanguage.LanguagePk, translatedGenreName);

				await context.SaveChangesAsync();
			}
		}

		private static async Task<Language> GetOrCreateUkrainianLanguageAsync(BooksDbContext context)
		{
			var language = await context.Languages
				.FirstOrDefaultAsync(l => l.LanguageName.ToLower() == UkrainianLanguageName.ToLower());

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
				LanguageName = UkrainianLanguageName
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
