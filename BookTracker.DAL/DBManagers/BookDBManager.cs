using BookTracker.DAL.Abstractions;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities;

using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBManagers
{

	/// <summary>
	/// Initializes a new instance of the <see cref="BookDbManager"/> class.
	/// </summary>
	public class BookDbManager(BooksDbContext booksDbContext) : BaseDbManager(booksDbContext), IBookDbManager
	{
		///<inheritdoc/>
		public async Task AddBook(Book book)
		{
			var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == book.Author.Name);
			if (author == null)
			{
				author = new Author
				{
					AuthorPk = Guid.NewGuid(),
					Name = book.Author.Name
				};

				await BooksDbContext.Authors.AddAsync(author);
				await BooksDbContext.SaveChangesAsync();
			}

			var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == book.Genre.Name);
			if (genre == null)
			{
				genre = new Genre
				{
					GenrePk = Guid.NewGuid(),
					Name = book.Genre.Name
				};

				await BooksDbContext.Genres.AddAsync(genre);
				await BooksDbContext.SaveChangesAsync();
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

			await BooksDbContext.Books.AddAsync(bookToSave);
			await BooksDbContext.SaveChangesAsync();
		}

		///<inheritdoc/>
		public async Task UpdateBook(Book updatedBook)
		{
			var book = await BooksDbContext.Books.FirstOrDefaultAsync(b => b.BookPk == updatedBook.BookPk);
			if (book == null)
			{
				Console.WriteLine("Book not found.");
				return;
			}

			var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == updatedBook.Author.Name);
			if (author == null)
			{
				author = new Author
				{
					AuthorPk = Guid.NewGuid(),
					Name = updatedBook.Author.Name
				};
				await BooksDbContext.Authors.AddAsync(author);
				await BooksDbContext.SaveChangesAsync();
			}

			var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == updatedBook.Genre.Name);
			if (genre == null)
			{
				genre = new Genre
				{
					GenrePk = Guid.NewGuid(),
					Name = updatedBook.Genre.Name
				};
				await BooksDbContext.Genres.AddAsync(genre);
				await BooksDbContext.SaveChangesAsync();
			}

			book.Title = updatedBook.Title;
			book.Author = author;
			book.Genre = genre;
			book.DateRead = updatedBook.DateRead?.ToUniversalTime();
			book.Rating = updatedBook.Rating;
			book.Notes = updatedBook.Notes;

			await BooksDbContext.SaveChangesAsync();
		}

		///<inheritdoc/>
		public async Task DeleteBook(Guid bookPk)
		{
			var book = await BooksDbContext.Books.FirstOrDefaultAsync(b => b.BookPk == bookPk);
			if (book != null)
			{
				BooksDbContext.Books.Remove(book);
				await BooksDbContext.SaveChangesAsync();
			}
		}

		///<inheritdoc/>
		public Task<List<Book>> GetAllBooks() => BooksDbContext.Books
															   .Include(b => b.Author)
															   .Include(b => b.Genre)
															   .OrderBy(b => b.Title)
															   .ToListAsync();

		///<inheritdoc/>
		public Task<Book?> FindBookByPk(Guid bookPk) =>
			BooksDbContext.Books
				.Include(b => b.Author)
				.Include(b => b.Genre)
				.FirstOrDefaultAsync(b => b.BookPk == bookPk);

		///<inheritdoc/>
		public async Task<int> CountBooksByAuthor(string authorName)
		{
			var author = await BooksDbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
			if (author == null)
			{
				return 0;
			}

			return await BooksDbContext.Books.CountAsync(b => b.Author.AuthorPk == author.AuthorPk);
		}

		///<inheritdoc/>
		public async Task<int> CountBooksByGenre(string genreName)
		{
			var genre = await BooksDbContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
			if (genre == null)
			{
				return 0;
			}

			return await BooksDbContext.Books.CountAsync(b => b.Genre.GenrePk == genre.GenrePk);
		}

		///<inheritdoc/>
		public async Task<Dictionary<int, int>> CountBooksByYears()
		{
			var currentYear = DateTime.UtcNow.Year;
			var years = Enumerable.Range(currentYear - 4, 5).Reverse().ToList();
			// Get all books with a DateRead in the given years
			var yearSet = years.ToHashSet();

			var query = await BooksDbContext.Books
				.Where(b => b.DateRead.HasValue && yearSet.Contains(b.DateRead.Value.Year))
				.GroupBy(b => b.DateRead!.Value.Year)
				.Select(g => new { Year = g.Key, Count = g.Count() })
				.ToListAsync();

			var result = years.ToDictionary(y => y, y => 0);
			foreach (var item in query)
			{
				result[item.Year] = item.Count;
			}

			return result;
		}
	}
}