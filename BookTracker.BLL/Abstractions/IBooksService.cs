using BookTracker.BLL.Models;

namespace BookTracker.BLL.Abstractions
{
	public interface IBooksService
	{
		/// <summary>
		/// Adds the book.
		/// </summary>
		/// <param name="book">The book.</param>
		/// <returns></returns>
		public Task AddBook(BookModel book);

		/// <summary>
		/// Edits the book.
		/// </summary>
		/// <param name="updatedBook">The updated book.</param>
		/// <returns></returns>
		public Task UpdateBook(BookModel updatedBook);

		/// <summary>
		/// Deletes the book.
		/// </summary>
		/// <param name="bookPk">The book pk.</param>
		/// <returns></returns>
		public Task DeleteBook(Guid bookPk);

		/// <summary>
		/// Gets all books.
		/// </summary>
		/// <returns></returns>
		public Task<List<BookModel>> GetAllBooks();

		/// <summary>
		/// Gets all books localized by language.
		/// </summary>
		/// <param name="languagePk">Language identifier.</param>
		/// <returns></returns>
		public Task<List<BookModel>> GetAllBooksLocalized(byte languagePk);

		/// <summary>
		/// Finds the book by identifier.
		/// </summary>
		/// <param name="bookPk">The book pk.</param>
		/// <returns></returns>
		public Task<BookModel?> FindBookByPk(Guid bookPk);

		/// <summary>
		/// Finds the book by identifier with localized data.
		/// </summary>
		/// <param name="bookPk">The book pk.</param>
		/// <param name="languagePk">Language identifier.</param>
		/// <returns></returns>
		public Task<BookModel?> FindBookByPkLocalized(Guid bookPk, byte languagePk);

		/// <summary>
		/// Counts the books by author.
		/// </summary>
		/// <param name="authorName">Name of the author.</param>
		/// <returns></returns>
		public Task<int> CountBooksByAuthor(string authorName);

		/// <summary>
		/// Counts the books by genre.
		/// </summary>
		/// <param name="genreName">Name of the genre.</param>
		/// <returns></returns>
		public Task<int> CountBooksByGenre(string genreName);

		/// <summary>
		/// Gets the count of books read for each year in the provided list of years.
		/// </summary>
		/// <returns></returns>
		Task<Dictionary<int, int>> CountBooksByYears();
	}
}
