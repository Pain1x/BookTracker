using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Genres;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BookTracker.DAL.DBManagers
{
	public class BookDbManager(IDbContextFactory<BooksDbContext> contextFactory) : IBookDbManager
	{
		#region Implementation of IBookDbManager

		///<inheritdoc/>
		public async Task AddBook(Book book)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var author = await context.Authors.FirstOrDefaultAsync(a => a.Name == book.Author.Name);

				if (author == null)
				{
					author = new Author
					{
						AuthorPk = Guid.NewGuid(),
						Name = book.Author.Name
					};

					await context.Authors.AddAsync(author);
					await context.SaveChangesAsync();
				}

				var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == book.Genre.Name);

				if (genre == null)
				{
					genre = new Genre
					{
						GenrePk = Guid.NewGuid(),
						Name = book.Genre.Name
					};

					await context.Genres.AddAsync(genre);
					await context.SaveChangesAsync();
				}

				var bookToSave = new Book
				{
					BookPk = Guid.NewGuid(),
					Title = book.Title,
					Author = author,
					Genre = genre,
					DateRead = book.DateRead?.ToUniversalTime(),
					Rating = book.Rating,
					Notes = book.Notes
				};

				await context.Books.AddAsync(bookToSave);
				await context.SaveChangesAsync();
			}
		}

		///<inheritdoc/>
		public async Task UpdateBook(Book updatedBook)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var book = await context.Books.FirstOrDefaultAsync(b => b.BookPk == updatedBook.BookPk);

				if (book == null)
				{
					return;
				}

				var author = await context.Authors.FirstOrDefaultAsync(a => a.Name == updatedBook.Author.Name);

				if (author == null)
				{
					author = new Author
					{
						AuthorPk = Guid.NewGuid(),
						Name = updatedBook.Author.Name
					};
					await context.Authors.AddAsync(author);
					await context.SaveChangesAsync();
				}

				var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == updatedBook.Genre.Name);

				if (genre == null)
				{
					genre = new Genre
					{
						GenrePk = Guid.NewGuid(),
						Name = updatedBook.Genre.Name
					};
					await context.Genres.AddAsync(genre);
					await context.SaveChangesAsync();
				}

				book.Title = updatedBook.Title;
				book.Author = author;
				book.Genre = genre;
				book.DateRead = updatedBook.DateRead?.ToUniversalTime();
				book.Rating = updatedBook.Rating;
				book.Notes = updatedBook.Notes;

				await context.SaveChangesAsync();
			}
		}

		///<inheritdoc/>
		public async Task DeleteBook(Guid bookPk)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var book = await context.Books.FirstOrDefaultAsync(b => b.BookPk == bookPk);

				if (book != null)
				{
					context.Books.Remove(book);
					await context.SaveChangesAsync();
				}
			}
		}

		///<inheritdoc/>
		public async Task<List<Book>> GetAllBooks()
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var sho = GetAllBooksLocalized(2);

				return await context.Books
					.Include(b => b.Author)
					.Include(b => b.Genre)
					.OrderBy(b => b.Title)
					.ToListAsync();
			}
		}

		public async Task<List<Book>> GetAllBooksLocalized(byte languagePk)
		{
			await using var context = await contextFactory.CreateDbContextAsync();

			var query = context.Books
				.Select(b => new Book
				{
					BookPk = b.BookPk,
					Title = b.Translations
						.Where(t => t.LanguagePk == languagePk)
						.Select(t => t.Title)
						.FirstOrDefault() ?? b.Title,
					Author = new Author
					{
						AuthorPk = b.AuthorPk,
						Name = b.Author.Translations
							.Where(t => t.LanguagePk == languagePk)
							.Select(t => t.Name)
							.FirstOrDefault() ?? b.Author.Name
					},
					Genre = new Genre
					{
						GenrePk = b.GenrePk,
						Name = b.Genre.Translations
							.Where(t => t.LanguagePk == languagePk)
							.Select(t => t.Name)
							.FirstOrDefault() ?? b.Genre.Name
					},
					DateRead = b.DateRead,
					Rating = b.Rating,
					Notes = b.Notes
				})
				.OrderBy(b => b.Title);

			return await query.ToListAsync();
		}

		///<inheritdoc/>
		public async Task<Book?> FindBookByPk(Guid bookPk)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				return await context.Books
					.Include(b => b.Author)
					.Include(b => b.Genre)
					.FirstOrDefaultAsync(b => b.BookPk == bookPk);
			}
		}

		///<inheritdoc/>
		public async Task<int> CountBooksByAuthor(string authorName)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var author = await context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);

				if (author == null)
				{
					return 0;
				}

				return await context.Books.CountAsync(b => b.Author.AuthorPk == author.AuthorPk);
			}
		}

		///<inheritdoc/>
		public async Task<int> CountBooksByGenre(string genreName)
		{
			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);

				if (genre == null)
				{
					return 0;
				}

				return await context.Books.CountAsync(b => b.Genre.GenrePk == genre.GenrePk);
			}
		}

		///<inheritdoc/>
		public async Task<Dictionary<int, int>> CountBooksByYears()
		{
			Dictionary<int, int> result;

			await using (var context = await contextFactory.CreateDbContextAsync())
			{
				var currentYear = DateTime.UtcNow.Year;
				var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();
				// Get all books with a DateRead in the given years
				var yearSet = years.ToHashSet();

				var query = await context.Books
					.Where(b => b.DateRead.HasValue && yearSet.Contains(b.DateRead.Value.Year))
					.GroupBy(b => b.DateRead!.Value.Year)
					.Select(g => new { Year = g.Key, Count = g.Count() })
					.ToListAsync();

				result = years.ToDictionary(y => y, y => 0);

				foreach (var item in query)
				{
					result[item.Year] = item.Count;
				}
			}

			return result;
		}

		#endregion
	}
}